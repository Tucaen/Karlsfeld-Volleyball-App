
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
		MySqlUser user;
		MySqlEvent _event;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			listUser = MySqlUser.GetListUserFromPreferences();
			user = MySqlUser.GetUserFromPreferences(this.Activity);
			_event = MySqlEvent.GetEventFromPreferences(this.Activity);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.EventDetails, container, false);
			MainActivity main = (MainActivity)this.Activity;

			view.FindViewById<TextView>(Resource.Id.eventTitle).Text = _event.name;
			view.FindViewById<TextView>(Resource.Id.eventState).Text = "(" + getLoggedInUser(user.idUser).eventState + ")";
			view.FindViewById<TextView>(Resource.Id.eventLocation).Text = _event.location;
			view.FindViewById<TextView>(Resource.Id.eventTime).Text = main.convertDateForLayout(_event);

			listView = view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser);
			listView.Adapter = new ListUserAdapter(this, listUser);

			return view;
		}

		private MySqlUser getLoggedInUser(int id) {
			foreach(MySqlUser u in listUser) {
				if(u.idUser == id)
					return u;
			}
			return null;
		}
	}
}

