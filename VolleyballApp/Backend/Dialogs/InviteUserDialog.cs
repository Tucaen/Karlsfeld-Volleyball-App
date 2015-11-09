
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

		public InviteUserDialog (Bundle bundle, MySqlEvent _event) {
			this.Arguments = bundle;
			this._event = _event;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.InviteUserDialog, container, false);

			List<MySqlUser> listUser = MySqlUser.GetListUserFromPreferences();
			ListView listView = view.FindViewById<ListView>(Resource.Id.InviteUserDialog_ListUser);
			listView.Adapter = new InviteUserDialogListAdapter(this, listUser);

			view.FindViewById<Button>(Resource.Id.InviteUserDialog_btnEinladen).Click += async delegate {
				this.Dismiss();
			};

			view.FindViewById<Button>(Resource.Id.InviteUserDialog_btnAbbrechen).Click += async delegate {
				this.Dismiss();
			};

			return view;
		}
	}

	class InviteUserDialogListAdapter : BaseAdapter<MySqlUser> {
		List<MySqlUser> listUser;
		Fragment context;

		public InviteUserDialogListAdapter(Fragment context, List<MySqlUser> listUser) : base() {
			this.context = context;
			this.listUser = listUser;
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
			var item = listUser[position];
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.Activity.LayoutInflater.Inflate(Resource.Layout.InviteUserDialogListView, null);
			view.FindViewById<TextView>(Resource.Id.InviteUserDialogUserName).Text = item.name;
//			view.FindViewById<CheckBox>(Resource.Id.InviteUserDialogCheckbox);
			return view;
		}
	}
}
