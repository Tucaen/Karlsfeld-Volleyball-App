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
			listView = FindViewById<ListView>(Resource.Id.listEvents);

			user = MySqlUser.GetUserFromPreferences(this);

			DB_Communicator db = new DB_Communicator();
			listEvents = db.SelectEventsForUser(user.idUser, null).Result;
			listView.Adapter = new ListEventsAdapter(this, listEvents);

			listView.ItemClick += OnListItemClick;
		}

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			ListView listView = sender as ListView;
			Intent i = new Intent(this, typeof(EventDetails));
			i.PutExtra(MySqlEvent.idEvent_string, listEvents[e.Position].idEvent);
			i.PutExtra(MySqlEvent.name_string, listEvents[e.Position].name);
			i.PutExtra(MySqlEvent.location_string, listEvents[e.Position].location);
			i.PutExtra(MySqlEvent.startDate_string, Convert.ToString(listEvents[e.Position].startDate));
			i.PutExtra(MySqlEvent.endDate_string, Convert.ToString(listEvents[e.Position].endDate));
			StartActivity(i);
		}
		
		protected override void OnRestart() {
			base.OnRestart();
			Finish();
			StartActivity(new Intent(this, typeof(ListEventsActivity)));
		}
	}
}

