
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
using System.Json;

namespace VolleyballApp {
	[Activity(Label = "RegistrationActivity")]			
	public class RegistrationActivity : Activity {
		MySqlUser user;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Registration);

			FindViewById<Button>(Resource.Id.btnRegistration).Click += async (object sender, EventArgs e) => {
				EditText email = FindViewById<EditText>(Resource.Id.registrationEmailData);
				EditText password = FindViewById<EditText>(Resource.Id.registrationPasswordData);

				DB_Communicator db = new DB_Communicator();

				JsonValue json = await db.register(email.Text, password.Text);

				if(json["state"].ToString().Equals("\"ok\"")) {
					user = await db.login(email.Text, password.Text);
					user.StoreUserInPreferences(this, user);

					Intent i = null;
					if(user.state.Equals("FILLDATA"))
						i = new Intent(this, typeof(FillDataActivity));
					else 
						i = new Intent(this, typeof(MainActivity));	
					StartActivity(i);
				} else {
					Toast.MakeText(this, json["message"].ToString(), ToastLength.Long).Show();
				}
			};
		}
	}
}

