
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
using Android.Preferences;
using System.Net;
using System.Threading;

namespace VolleyballApp {
	[Activity(Label = "VolleyballApp - Login", MainLauncher = true, Icon = "@drawable/icon")]	
	public class LogIn : AbstractActivity {
//		MySqlUser user;

		protected override void OnCreate(Bundle bundle) {
			// You may use ServicePointManager here
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			base.OnCreate(bundle);

			SetContentView(Resource.Layout.LogIn);

			Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

			btnLogin.Click += (object sender, EventArgs e) => {
				EditText username = FindViewById<EditText>(Resource.Id.usernameText);
				EditText password = FindViewById<EditText>(Resource.Id.passwordText);

				base.login(username.Text, password.Text);
			};

			FindViewById<TextView>(Resource.Id.registrierenText).Click += (object sender, EventArgs e) => {
				Intent i = new Intent(this, typeof(RegistrationActivity));
				StartActivity(i);
			};
		}


	}
}

