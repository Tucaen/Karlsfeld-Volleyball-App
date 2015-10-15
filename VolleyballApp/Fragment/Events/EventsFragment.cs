
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
		List<MySqlEvent> listEvents;
		View view;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			//Get all events for the logged in user
			listEvents = MySqlEvent.GetListEventsFromPreferences();
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
			MainActivity mainActivity = (MainActivity) this.Activity;
			mainActivity.OnListEventClicked(e, listEvents);
		}
	}
}

