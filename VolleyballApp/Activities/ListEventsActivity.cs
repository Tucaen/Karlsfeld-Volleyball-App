
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
		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.ListEvents);
			listView = FindViewById<ListView>(Resource.Id.listEvents);

			DB_Communicator db = new DB_Communicator();
			Console.WriteLine("Created DB_Communicator");
  			listEvents = db.SelectEventsForUser(1, null).Result;
			Console.WriteLine("Passed terrifying point of selecting all events for this user");
			listView.Adapter = new ListEventsAdapter(this, listEvents);

			listView.ItemClick += OnListItemClick;
		}

		void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			ListView listView = sender as ListView;
			Console.WriteLine("Item was clicked");
		}
	}
}

