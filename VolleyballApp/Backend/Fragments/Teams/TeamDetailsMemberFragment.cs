
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace VolleyballApp {
	public class TeamDetailsMemberFragment : Fragment {
		private List<VBUser> listMember;

		public TeamDetailsMemberFragment(List<VBUser> listMember) {
			this.listMember = listMember;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.TeamDetailsMemberFragment, container, false);

			if(this.listMember != null && this.listMember.Count > 0) {
				ListView listView = view.FindViewById<ListView>(Resource.Id.teamDetailsMemberListMember);

				view.FindViewById<TextView>(Resource.Id.teamDetailsMemberNoMember).Visibility = ViewStates.Gone;
				listView.Visibility = ViewStates.Visible;

				listView.Adapter = new ListUserAdapter(this, listMember);
			} else {
				view.FindViewById<TextView>(Resource.Id.teamDetailsMemberNoMember).Visibility = ViewStates.Visible;
				view.FindViewById<ListView>(Resource.Id.teamDetailsMemberListMember).Visibility = ViewStates.Gone;
			}

			return view;
		}
	}
}

