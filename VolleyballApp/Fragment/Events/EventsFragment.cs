
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
using System.Threading.Tasks;

namespace VolleyballApp {
	public class EventsFragment : Fragment {
		ListView listView;
		List<MySqlEvent> listEvents;
		MySqlUser user;
		DB_Communicator db;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			//Get all events for the logged in user
			db = DB_Communicator.getInstance();
			user = MySqlUser.GetUserFromPreferences(this.Activity);
			listEvents = new List<MySqlEvent>();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = null;
			initializeEventList(listEvents);

			if(listEvents.Count == 0) {
				//display text that there are currently no events
				view = inflater.Inflate(Resource.Layout.NoEventsFoundFragment, container, false);
			} else {
				view = inflater.Inflate(Resource.Layout.EventsFragment, container, false);
				listView = view.FindViewById<ListView>(Resource.Id.listEvents);
				Console.WriteLine("EventsFragment.OnCreateView got a listView");
				listView.Adapter = new ListEventsAdapter(this, listEvents);
				listView.ItemClick += OnListItemClick;
			}
			return view;
		}

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			ListView listView = sender as ListView;
			Intent i = new Intent(this.Activity, typeof(EventDetails));
			i.PutExtra("idEvent", listEvents[e.Position].idEvent);
			StartActivity(i);
		}

		private async void initializeEventList(List<MySqlEvent> listEvents) {
			listEvents = await db.SelectEventsForUser(user.idUser, null);
		}
	}
}

