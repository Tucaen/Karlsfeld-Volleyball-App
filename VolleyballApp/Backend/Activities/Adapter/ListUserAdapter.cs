using System;
using Android.Widget;
using System.Collections.Generic;
using Android.App;
using Android.Views;

namespace VolleyballApp {
	public class ListUserAdapter : BaseAdapter<VBUser> {
		List<VBUser> listUser;
		Fragment context;

		public ListUserAdapter(Fragment context, List<VBUser> listUser) : base() {
			this.context = context;
			this.listUser = listUser;
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
			var item = listUser[position];
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.Activity.LayoutInflater.Inflate(Resource.Layout.UserListView, null);
			
			view.FindViewById<TextView>(Resource.Id.UserListViewName).Text = item.name;

			if(item.listTeamRole != null && item.listTeamRole.Count > 0 && item.listTeamRole[0].position != null && 
				!item.listTeamRole[0].position.Equals("") && !item.listTeamRole[0].position.Equals("Keine")) {

				view.FindViewById<TextView>(Resource.Id.UserListViewPosition).Text = "(" + item.listTeamRole[0].position + ")";
			} else {
				view.FindViewById<TextView>(Resource.Id.UserListViewPosition).Text = "";
			}
			return view;
		}
	}
}

