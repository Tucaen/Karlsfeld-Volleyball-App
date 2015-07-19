using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace VolleyballApp
{
	[Activity (Label = "VolleyballApp", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			DB_Communicator db = new DB_Communicator();
//			db.SelectEventsForUser(2, DB_Communicator.State.Invited);
//			db.SelectEventsForUser(2, null);
//			db.InsertUser("test test", "Spieler", "spieler", 0, "?");
//			db.DeleteUser(8);
//			db.InsertUser("Lukas Hoffmann", "Spieler", "spieler", 0, "?");
			db.UpdateUser(7, null, null, null, 0, null);
		}
	}
}


