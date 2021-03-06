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
	[Activity (Label = "VolleyballApp - Events", MainLauncher = false)]			
	public class ListEventsActivity : Activity {
		ListView listView;
		List<MySqlEvent> listEvents;
		MySqlUser user;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.ListEvents);

			//Get all events for the logged in user
			DB_Communicator db = new DB_Communicator();
			user = MySqlUser.GetUserFromPreferences(this);
			listEvents = db.SelectEventsForUser(user.idUser, null).Result;

			if(listEvents.Count == 0) {
				//display text that there are currently no events
			} else {
				listView = FindViewById<ListView>(Resource.Id.listEvents);
				listView.Adapter = new ListEventsAdapter(this, listEvents);
				listView.ItemClick += OnListItemClick;
			}
		}

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			ListView listView = sender as ListView;
			Intent i = new Intent(this, typeof(EventDetails));
			i.PutExtra("idEvent", listEvents[e.Position].idEvent);
			StartActivity(i);
		}
	}
}

