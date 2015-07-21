using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace VolleyballApp
{
	[Activity (Label = "VolleyballApp - Events"), MainLauncher = true]
	public class ListEvents : ListActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.ListEvents);

			DB_Communicator db = new DB_Communicator();

			List<MySqlEvent> listEvent = db.SelectEventsForUser(1, null).Result;

			ListView listViewEvents = FindViewById<ListView>(Resource.Id.listEvents);

			ArrayAdapter adapter = new ArrayAdapter<MySqlEvent>(this, Android.Resource.Layout.SimpleListItem1, listEvent);

			listViewEvents.Adapter = adapter;




//			db.SelectEventsForUser(2, DB_Communicator.State.Invited);
//			db.SelectEventsForUser(2, null);
//			db.InsertUser("test test", "Spieler", "spieler", 0, "?");
//			db.DeleteUser(8);
//			db.InsertUser("Lukas Hoffmann", "Spieler", "spieler", 0, "?");
//			db.UpdateUser(7, null, null, null, 99, null);
		}

		protected override void OnListItemClick(ListView l, View v, int position, long id)
		{
			//This is the body
			//insert your code here.
		}
	}
}


