
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
	[Activity(Label = "VolleyballApp - Login")]
	public class LogIn : AbstractActivity {

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.LogIn);

			Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

			btnLogin.Click += async (object sender, EventArgs e) => {
				EditText username = FindViewById<EditText>(Resource.Id.usernameText);
				EditText password = FindViewById<EditText>(Resource.Id.passwordText);

				if(await base.login(username.Text, password.Text))
					base.proceedAfterManualLogin();
			};

			FindViewById<TextView>(Resource.Id.registrierenText).Click += (object sender, EventArgs e) => {
				Intent i = new Intent(this, typeof(RegistrationActivity));
				i.AddFlags(ActivityFlags.NoHistory);
				StartActivity(i);
			};
		}
	}
}

