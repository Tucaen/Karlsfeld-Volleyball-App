
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
	[Activity(Label = "Login", MainLauncher = false, Icon = "@drawable/icon")]	
	public class LogIn : Activity {
		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.LogIn);

			DB_Communicator db = new DB_Communicator();

			Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

			btnLogin.Click += async (object sender, EventArgs e) => {
				EditText username = FindViewById<EditText>(Resource.Id.usernameText);
				EditText password = FindViewById<EditText>(Resource.Id.passwordText);

				if(await db.login(username.Text, password.Text)) {
					Toast.MakeText(this, "Login successful!", ToastLength.Short).Show();
					Intent i = new Intent(this, typeof(ListEventsActivity));
					StartActivity(i);
				} else {
					Toast.MakeText(this, "Login failed!", ToastLength.Long).Show();
				}
			};


		}
	}
}

