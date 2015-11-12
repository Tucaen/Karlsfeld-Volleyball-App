
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Json;

namespace VolleyballApp {

	class InviteUserDialog : DialogFragment {
		private MySqlEvent _event;
		private int position;

		public InviteUserDialog (Bundle bundle, MySqlEvent _event, int position) {
			this.Arguments = bundle;
			this._event = _event;
			this.position = position;
			this.SetStyle(DialogFragmentStyle.Normal, 4);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.InviteUserDialog, container, false);

			List<MySqlUser> listUser = MySqlUser.GetListUserFromPreferences();
			ListView listView = view.FindViewById<ListView>(Resource.Id.InviteUserDialog_ListUser);
			listView.Adapter = new InviteUserDialogListAdapter(this, listUser);

			view.FindViewById<Button>(Resource.Id.InviteUserDialog_btnEinladen).Click += async delegate {
				string toInvite = generateStringForInvatation((listView.Adapter as InviteUserDialogListAdapter).listUserToInvite);
				JsonValue json = await DB_Communicator.getInstance().inviteUserToEvent(_event.idEvent, toInvite);

				Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long).Show();

				MainActivity main = this.Activity as MainActivity;
				main.refreshEventDetailsFragment(MainActivity.EVENT_DETAILS_FRAGMENT, this.position);

				this.Dismiss();
			};

			view.FindViewById<Button>(Resource.Id.InviteUserDialog_btnAbbrechen).Click += delegate {
				this.Dismiss();
			};

			return view;
		}

		private string generateStringForInvatation(List<UserToInvite> list) {
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			foreach(UserToInvite user in list) {
				if(user.isChecked) {
					sb.Append(user.user.idUser);
					sb.Append(",");
				}
			}

			if(sb.Length >= 2)
				sb.Remove(sb.Length - 1, 1);
			
			sb.Append("]");
			return sb.ToString();
		}
	}

	class InviteUserDialogListAdapter : BaseAdapter<MySqlUser> {
		List<MySqlUser> listUser;
		public List<UserToInvite> listUserToInvite { get; private set; }
		Fragment context;
		MySqlUser item;

		public InviteUserDialogListAdapter(Fragment context, List<MySqlUser> listUser) : base() {
			this.context = context;
			this.listUser = listUser;
			this.listUserToInvite = new List<UserToInvite>();
		}

		public override long GetItemId(int position) {
			return position;
		}
		public override MySqlUser this[int position] {
			get { return listUser[position]; }
		}
		public override int Count {
			get { return listUser.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent) {
			item = listUser[position];
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.Activity.LayoutInflater.Inflate(Resource.Layout.InviteUserDialogListView, null);
			
			TextView username = view.FindViewById<TextView>(Resource.Id.InviteUserDialogUserName);
			username.Text = item.name;

			CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.InviteUserDialogCheckbox);

			username.SetOnClickListener(new UserToInviteClickListener(checkBox, item, true, listUserToInvite));
			checkBox.SetOnClickListener(new UserToInviteClickListener(checkBox, item, false, listUserToInvite));

			return view;
		}
	}

	class UserToInvite {
		public MySqlUser user { get; set; }
		public bool isChecked { get; set; }

		public UserToInvite(MySqlUser user, bool isChecked) {
			this.user = user;
			this.isChecked = isChecked;
		}
	}

	class UserToInviteClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
		private CheckBox checkBox;
		private MySqlUser item;
		private bool switchChecked;
		private List<UserToInvite> listUserToInvite;

		public UserToInviteClickListener(CheckBox checkBox, MySqlUser item, bool switchChecked, List<UserToInvite> listUserToInvite) {
			this.checkBox = checkBox;
			this.item = item;
			this.switchChecked = switchChecked;
			this.listUserToInvite = listUserToInvite;
		}

		public void OnClick(View v) {
			if(switchChecked)
				checkBox.Checked = !checkBox.Checked;
			UserToInvite userToInvite = getUserToInvite(listUserToInvite, item.idUser);
			if(userToInvite == null)
				listUserToInvite.Add(new UserToInvite(item, checkBox.Checked));
			else
				updateUserToInvite(listUserToInvite, item.idUser, checkBox.Checked);
		}

		private void updateUserToInvite(List<UserToInvite> list, int id, bool isChecked) {
			foreach (UserToInvite user in list) {
				if(user.user.idUser == id)
					user.isChecked = isChecked;
			}
		}

		private UserToInvite getUserToInvite(List<UserToInvite> list, int id) {
			foreach (UserToInvite user in list) {
				if(user.user.idUser == id)
					return user;
			}
			return null;
		}
	}
}
