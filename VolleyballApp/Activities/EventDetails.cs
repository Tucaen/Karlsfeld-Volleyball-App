
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
		MySqlUser user;
		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.EventDetails);
			listView = FindViewById<ListView>(Resource.Id.EventDetails_ListUser);

			DB_Communicator db = new DB_Communicator();
			listUser = db.SelectUserForEvent(Convert.ToInt32(this.Intent.Extras.Get(MySqlEvent.idEvent_string)), null).Result;

			user = MySqlUser.GetUserFromPreferences(this);
			user.state = MySqlUser.GetUserFromList(user.idUser, listUser).state;

			FindViewById<TextView>(Resource.Id.EventDetails_eventLocation).Text = Convert.ToString(this.Intent.Extras.Get(MySqlEvent.location_string));
			FindViewById<TextView>(Resource.Id.EventDetails_eventState).Text = "(" + user.state + ")";
			FindViewById<TextView>(Resource.Id.EventDetails_eventTitle).Text = Convert.ToString(this.Intent.Extras.Get(MySqlEvent.name_string));
			DateTime startDate  = Convert.ToDateTime(Convert.ToString(this.Intent.Extras.Get(MySqlEvent.startDate_string)));
			DateTime endDate  = Convert.ToDateTime(Convert.ToString(this.Intent.Extras.Get(MySqlEvent.endDate_string)));
			FindViewById<TextView>(Resource.Id.EventDetails_eventTime).Text = MySqlEvent.convertDateToString(startDate, endDate);

			listView.Adapter = new ListUserAdapter(this, listUser);

			FindViewById<Button>(Resource.Id.EventDetails_eventZusagen).Click += async (object sender, EventArgs e) => {
				if(!user.state.Equals(DB_Communicator.State.Accepted)) {
					await db.UpdateState(user.idUser, Convert.ToInt32(this.Intent.Extras.Get(MySqlEvent.idEvent_string)), DB_Communicator.State.Accepted);
					Intent i = new Intent(this, typeof(EventDetails));
					i.PutExtra(MySqlEvent.idEvent_string, Convert.ToInt32(this.Intent.Extras.Get(MySqlEvent.idEvent_string)));
					i.PutExtra(MySqlEvent.name_string, Convert.ToString(this.Intent.Extras.Get(MySqlEvent.name_string)));
					i.PutExtra(MySqlEvent.location_string, Convert.ToString(this.Intent.Extras.Get(MySqlEvent.location_string)));
					i.PutExtra(MySqlEvent.startDate_string, Convert.ToString(this.Intent.Extras.Get(MySqlEvent.startDate_string)));
					i.PutExtra(MySqlEvent.endDate_string, Convert.ToString(this.Intent.Extras.Get(MySqlEvent.endDate_string)));
					StartActivity(i);
				}
			};

			FindViewById<Button>(Resource.Id.EventDetails_eventAbsagen).Click += async (object sender, EventArgs e) => {
				if(!user.state.Equals(DB_Communicator.State.Denied)) {
					await db.UpdateState(user.idUser, Convert.ToInt32(this.Intent.Extras.Get(MySqlEvent.idEvent_string)), DB_Communicator.State.Denied);
					Intent i = new Intent(this, typeof(EventDetails));
					i.PutExtra(MySqlEvent.idEvent_string, Convert.ToInt32(this.Intent.Extras.Get(MySqlEvent.idEvent_string)));
					i.PutExtra(MySqlEvent.name_string, Convert.ToString(this.Intent.Extras.Get(MySqlEvent.name_string)));
					i.PutExtra(MySqlEvent.location_string, Convert.ToString(this.Intent.Extras.Get(MySqlEvent.location_string)));
					i.PutExtra(MySqlEvent.startDate_string, Convert.ToString(this.Intent.Extras.Get(MySqlEvent.startDate_string)));
					i.PutExtra(MySqlEvent.endDate_string, Convert.ToString(this.Intent.Extras.Get(MySqlEvent.endDate_string)));
					StartActivity(i);
				}
			};
		}
	}
}

