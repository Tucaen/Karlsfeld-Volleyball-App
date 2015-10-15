
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

namespace VolleyballApp {
	public class EventDetailsFragment : Fragment {
		ListView listView;
		List<MySqlUser> listUser;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			listUser = MySqlUser.GetListUserFromPreferences();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.EventDetails, container, false);
			listView = view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser);
			listView.Adapter = new ListUserAdapter(this, listUser);
			return view;
		}
	}
}

