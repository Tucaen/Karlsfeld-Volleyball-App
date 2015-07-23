
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

			DB_Communicator db = new DB_Communicator();
			listUser = db.SelectUserForEvent(Convert.ToInt32(this.Intent.Extras.Get("idEvent")), null).Result;

			listView.Adapter = new ListUserAdapter(this, listUser);
		}
	}
}

