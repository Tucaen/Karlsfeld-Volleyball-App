
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
	[Activity(Label = "Volleyball App - EventDetails")]			
	public class EventDetails : Activity {
		ListView listView;
		List<MySqlUser> listUser;
		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.EventDetails);
			listView = FindViewById<ListView>(Resource.Id.EventDetails_ListUser);

			FindViewById<TextView>(Resource.Id.EventDetails_eventLocation).Text = Convert.ToString(this.Intent.Extras.Get(MySqlEvent.location_string));
			FindViewById<TextView>(Resource.Id.EventDetails_eventState).Text = "(" + Convert.ToString(this.Intent.Extras.Get(MySqlUser.GetUserFromPreferences(this).state) + ")");
			FindViewById<TextView>(Resource.Id.EventDetails_eventTitle).Text = Convert.ToString(this.Intent.Extras.Get(MySqlEvent.name_string));
			Console.WriteLine("startDate = " + this.Intent.Extras.Get(MySqlEvent.startDate_string));
			DateTime startDate  = Convert.ToDateTime(Convert.ToString(this.Intent.Extras.Get(MySqlEvent.startDate_string)));
			DateTime endDate  = Convert.ToDateTime(Convert.ToString(this.Intent.Extras.Get(MySqlEvent.endDate_string)));
			FindViewById<TextView>(Resource.Id.EventDetails_eventTime).Text = MySqlEvent.convertDateToString(startDate, endDate);

			DB_Communicator db = new DB_Communicator();
			listUser = db.SelectUserForEvent(Convert.ToInt32(this.Intent.Extras.Get(MySqlEvent.idEvent_string)), null).Result;

			listView.Adapter = new ListUserAdapter(this, listUser);
		}
	}
}

