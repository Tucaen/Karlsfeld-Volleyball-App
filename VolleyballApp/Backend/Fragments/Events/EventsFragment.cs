
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
using System.Threading;

namespace VolleyballApp {
	public class EventsFragment : Fragment {
		ListView listView;
		public List<MySqlEvent> listEvents { get; set; }
		View view;

//		public EventsFragment() {
//		}

		public EventsFragment(List<MySqlEvent> listEvents) {
			this.listEvents = listEvents;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			//Get all events for the logged in user
//			listEvents = MySqlEvent.GetListEventsFromPreferences();

			view = inflater.Inflate(Resource.Layout.EventsFragment, container, false);
			if(listEvents.Count == 0) {
				//display text that there are currently no events and hide list with events
				view.FindViewById(Resource.Id.listEvents).Visibility = ViewStates.Gone;
			} else {
				//display list with events and hide the text
				view.FindViewById(Resource.Id.noEvents).Visibility = ViewStates.Gone;

				listView = view.FindViewById<ListView>(Resource.Id.listEvents);
				listView.Adapter = new ListEventsAdapter(this, listEvents);
				listView.ItemClick += OnListItemClick;
			}

			view.FindViewById<Button>(Resource.Id.btnAddEvent).Click += (object sender, EventArgs e) => {
				MainActivity mainActivity = (MainActivity) this.Activity;
				mainActivity.switchFragment(MainActivity.UPCOMING_EVENTS_FRAGMENT, MainActivity.ADD_EVENT_FRAGMENT, new AddEventFragment());
			};

			return view;
		}

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			MainActivity mainActivity = (MainActivity) this.Activity;
			mainActivity.OnListEventClicked(e, listEvents);
		}
	}
}

