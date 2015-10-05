
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
	public class EventsFragment : Fragment {
		ListView listView;
		List<MySqlEvent> listEvents;
		MySqlUser user;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			//Get all events for the logged in user
			DB_Communicator db = new DB_Communicator();
			user = MySqlUser.GetUserFromPreferences(this.Activity);
			listEvents = db.SelectEventsForUser(user.idUser, null).Result;


		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = null;
			if(listEvents.Count == 0) {
				//display text that there are currently no events
				view = inflater.Inflate(Resource.Layout.NoEventsFoundFragment, container, false);
			} else {
				listView = Activity.FindViewById<ListView>(Resource.Id.listEvents);
				listView.Adapter = new ListEventsAdapter(this.Activity, listEvents);
				listView.ItemClick += OnListItemClick;
				view = inflater.Inflate(Resource.Layout.EventsFragment, container, false);
			}
			return view;
		}

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			ListView listView = sender as ListView;
			Intent i = new Intent(this.Activity, typeof(EventDetails));
			i.PutExtra("idEvent", listEvents[e.Position].idEvent);
			StartActivity(i);
		}
	}
}

