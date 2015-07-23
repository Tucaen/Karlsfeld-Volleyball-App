
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

			FindViewById<TextView>(Resource.Id.EventDetails_eventLocation).Text = this.Intent.Extras.Get(MySqlEvent.location_string);
			FindViewById<TextView>(Resource.Id.EventDetails_eventState).Text = "(" + this.Intent.Extras.Get(MySqlEvent.state_string) + ")";
			FindViewById<TextView>(Resource.Id.EventDetails_eventTitle).Text = this.Intent.Extras.Get(MySqlEvent.name_string);
			DateTime startDate  = this.Intent.Extras.Get(MySqlEvent.startDate_string);
			DateTime endDate  = this.Intent.Extras.Get(MySqlEvent.endDate_string);

			if(startDate.Day == endDate.Day) {
				FindViewById<TextView>(Resource.Id.EventDetails_eventTime).Text = startDate.ToString("dd.MM.yy HH:MM") + " - " + endDate.ToString("HH:MM");
			} else {
				FindViewById<TextView>(Resource.Id.EventDetails_eventTime).Text = startDate.ToString("dd.MM.yy HH:MM") + " - " + endDate.ToString("dd.MM.yy HH:MM");
			}

			DB_Communicator db = new DB_Communicator();
			listUser = db.SelectUserForEvent(Convert.ToInt32(this.Intent.Extras.Get(MySqlEvent.idEvent_string)), null).Result;

			listView.Adapter = new ListUserAdapter(this, listUser);
		}
	}
}

