
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
		public List<VBEvent> listEvents { get; set; }
		View view;

		public EventsFragment(List<VBEvent> listEvents) {
			this.listEvents = listEvents;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
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

			if(DB_Communicator.getInstance().isAtLeast(VBUser.GetUserFromPreferences().getUserType(), UserType.Coremember)) {
				view.FindViewById<LinearLayout>(Resource.Id.eventsFragmentBtnAddLine).Visibility = ViewStates.Visible;
			} else {
				view.FindViewById<LinearLayout>(Resource.Id.eventsFragmentBtnAddLine).Visibility = ViewStates.Gone;
			}

			view.FindViewById<Button>(Resource.Id.btnAddEvent).Click += (object sender, EventArgs e) => {
				MainActivity mainActivity = (MainActivity) this.Activity;
				mainActivity.switchFragment(ViewController.UPCOMING_EVENTS_FRAGMENT, ViewController.ADD_EVENT_FRAGMENT, new AddEventFragment());
			};

			return view;
		}

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			MainActivity mainActivity = (MainActivity) this.Activity;
			mainActivity.OnListEventClicked(e, listEvents);
		}
	}
}

