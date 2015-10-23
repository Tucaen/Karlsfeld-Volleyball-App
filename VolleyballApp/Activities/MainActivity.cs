﻿
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
	[Activity(Label = "MainActivity")]			
	public class MainActivity : AbstractActivity {
		FragmentTransaction trans;
		private static readonly string EVENTS_FRAGMENT = "EventsFragment", EVENT_DETAILS_FRAGMENT = "EventDetailsFragment";

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

			trans = FragmentManager.BeginTransaction();
			trans.Add(Resource.Id.fragmentContainer, new EventsFragment(), EVENTS_FRAGMENT);
			trans.Commit();

		}

		public async void OnListEventClicked(AdapterView.ItemClickEventArgs e, List<MySqlEvent> listEvents) {
			ProgressDialog dialog = base.createProgressDialog("Please wait!", "Loading...");

			MySqlEvent clickedEvent = listEvents[e.Position];
			clickedEvent.StoreEventInPreferences(this, clickedEvent);

			List<MySqlUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(clickedEvent.idEvent, "");
			MySqlUser.StoreUserListInPreferences(this.Intent, listUser);

			dialog.Dismiss();

			trans = FragmentManager.BeginTransaction();
			trans.AddToBackStack(EVENTS_FRAGMENT);
			trans.Replace(Resource.Id.fragmentContainer, new EventDetailsFragment());
			trans.Commit();
		}
	}
}

