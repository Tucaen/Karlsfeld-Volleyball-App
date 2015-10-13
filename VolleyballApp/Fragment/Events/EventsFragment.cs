
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
//		MySqlUser user;
//		DB_Communicator db;
		View view;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			//Get all events for the logged in user
			Console.WriteLine("Trying to load events from preferences...");
			listEvents = MySqlEvent.GetListEventsFromPreferences(this.Activity.Intent);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			view = null;

			if(listEvents.Count == 0) {
				//display text that there are currently no events
				view = inflater.Inflate(Resource.Layout.NoEventsFoundFragment, container, false);
			} else {
				view = inflater.Inflate(Resource.Layout.EventsFragment, container, false);
				listView = view.FindViewById<ListView>(Resource.Id.listEvents);
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

//		private async Task<List<MySqlEvent>> initializeEventList(List<MySqlEvent> listEvents) {
//			//show loading screen
//			Console.WriteLine("before fetching events");
//			listEvents = await db.SelectEventsForUser(user.idUser, null);
//			Console.WriteLine("after fetching events");
//			//dismiss loading screen
//			return listEvents;
//		}
	}
}

