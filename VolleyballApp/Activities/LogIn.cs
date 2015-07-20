
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
	[Activity(Label = "LogIn", MainLauncher = true, Icon = "@drawable/icon")]	
	public class LogIn : Activity {
		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.LogIn);

			Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

			btnLogin.Click += async (object sender, EventArgs e) => {
				EditText username = FindViewById<EditText>(Resource.Id.usernameText);
				EditText password = FindViewById<EditText>(Resource.Id.passwordText);

				DB_Communicator db = new DB_Communicator();
				if(await db.login(username.Text, password.Text)) {
					
				} else {
					
				}
			};


		}
	}
}

