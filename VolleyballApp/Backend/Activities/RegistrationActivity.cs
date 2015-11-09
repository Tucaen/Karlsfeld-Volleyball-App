
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
	public class RegistrationActivity : AbstractActivity {

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Registration);

			FindViewById<Button>(Resource.Id.btnRegistration).Click += async (object sender, EventArgs e) => {
				EditText email = FindViewById<EditText>(Resource.Id.registrationEmailData);
				EditText password = FindViewById<EditText>(Resource.Id.registrationPasswordData);

				DB_Communicator db = DB_Communicator.getInstance();

				JsonValue json = await db.register(email.Text, password.Text);

				if(db.wasSuccesful(json)) {
					Toast.MakeText(this, "Registered successfully! Trying to log in...", ToastLength.Long).Show();

					if(await base.login(email.Text, password.Text))
						base.proceedAfterManualLogin();
					
				} else {
					Toast.MakeText(this, json["message"].ToString(), ToastLength.Long).Show();
				}
			};
		}
	}
}

