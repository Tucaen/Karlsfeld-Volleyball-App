
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
		private VBEvent _event;
		public bool isShown { get; private set; }
		private List<VBUser> listUserToInvite;

		public InviteUserDialog (Bundle bundle, VBEvent _event, List<VBUser> listUserToInvite) {
			this.Arguments = bundle;
			this._event = _event;
			this.listUserToInvite = listUserToInvite;
			this.SetStyle(DialogFragmentStyle.Normal, 4);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.InviteUserDialog, container, false);

			ListView listView = view.FindViewById<ListView>(Resource.Id.InviteUserDialog_ListUser);
			listView.Adapter = new InviteUserDialogListAdapter(this, listUserToInvite);

			view.FindViewById<Button>(Resource.Id.InviteUserDialog_btnEinladen).Click += async delegate {
				if((listView.Adapter as InviteUserDialogListAdapter).listUserToInvite.Count > 0) {
					string toInvite = generateStringForInvatation((listView.Adapter as InviteUserDialogListAdapter).listUserToInvite);
					JsonValue json = await DB_Communicator.getInstance().inviteUserToEvent(_event.idEvent, toInvite);
					
					Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long).Show();

					//refresh event data | e.g time, location, name
					await ViewController.getInstance().refreshDataForEvent(_event.idEvent);

					//refresh list of inveted users
					List<VBUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(_event.idEvent, "");
					EventDetailsFragment frag = FragmentManager.FindFragmentByTag(ViewController.EVENT_DETAILS_FRAGMENT) as EventDetailsFragment;
					frag.listUser = listUser;

					//refresh view
					ViewController.getInstance().refreshFragment(ViewController.EVENT_DETAILS_FRAGMENT);
					
					this.Dismiss();
				}
			};

			view.FindViewById<Button>(Resource.Id.InviteUserDialog_btnAbbrechen).Click += delegate {
				this.Dismiss();
			};
			this.isShown = true;
			return view;
		}

		public override void Dismiss() {
			base.Dismiss();
			this.isShown = false;
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

	class InviteUserDialogListAdapter : BaseAdapter<VBUser> {
		List<VBUser> listUser;
		public List<UserToInvite> listUserToInvite { get; private set; }
		Fragment context;
		VBUser item;

		public InviteUserDialogListAdapter(Fragment context, List<VBUser> listUser) : base() {
			this.context = context;
			this.listUser = listUser;
			this.listUserToInvite = new List<UserToInvite>();
		}

		public override long GetItemId(int position) {
			return position;
		}
		public override VBUser this[int position] {
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
			username.Text = item.getNameForUI();

			CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.InviteUserDialogCheckbox);

			username.SetOnClickListener(new UserToInviteClickListener(checkBox, item, true, listUserToInvite));
			checkBox.SetOnClickListener(new UserToInviteClickListener(checkBox, item, false, listUserToInvite));

			return view;
		}
	}

	class UserToInvite {
		public VBUser user { get; set; }
		public bool isChecked { get; set; }

		public UserToInvite(VBUser user, bool isChecked) {
			this.user = user;
			this.isChecked = isChecked;
		}
	}

	class UserToInviteClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
		private CheckBox checkBox;
		private VBUser item;
		private bool switchChecked;
		private List<UserToInvite> listUserToInvite;

		public UserToInviteClickListener(CheckBox checkBox, VBUser item, bool switchChecked, List<UserToInvite> listUserToInvite) {
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
