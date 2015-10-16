﻿
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
	public class LogIn : Activity {
		MySqlUser user;

		protected override void OnCreate(Bundle bundle) {
			// You may use ServicePointManager here
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			base.OnCreate(bundle);

			SetContentView(Resource.Layout.LogIn);

			DB_Communicator db = DB_Communicator.getInstance();

			Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

			btnLogin.Click += async (object sender, EventArgs e) => {
//				new Thread(new ThreadStart(async delegate {
				ProgressDialog dialog = ProgressDialog.Show(this, "Please wait", "Loading...");
				dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
				dialog.SetCancelable(false);
				dialog.Indeterminate = true;

				EditText username = FindViewById<EditText>(Resource.Id.usernameText);
				EditText password = FindViewById<EditText>(Resource.Id.passwordText);
			
				user = await db.login(username.Text, password.Text);

				if(user != null) {
					//storing user information for usage in other activities
					user.StoreUserInPreferences(this, user);

					Toast.MakeText(this, "Login successful!", ToastLength.Short).Show();

					Intent i = null;
					if(user.state.Equals("\"FILLDATA\"") || user.state.Equals("FILLDATA")) {
						i = new Intent(this, typeof(FillDataActivity));
					} else {
						List<MySqlEvent> listEvents = new List<MySqlEvent>();
						listEvents = await db.SelectEventsForUser(user.idUser, null);
						MySqlEvent.StoreEventListInPreferences(Intent, listEvents);
						i = new Intent(this, typeof(MainActivity));	
					}
					dialog.Dismiss();
					StartActivity(i);
					Finish();
				} else {
					dialog.Dismiss();
					Toast.MakeText(this, "Login failed!", ToastLength.Long).Show();
				}
			};

			FindViewById<TextView>(Resource.Id.registrierenText).Click += (object sender, EventArgs e) => {
				Intent i = new Intent(this, typeof(RegistrationActivity));
				StartActivity(i);
			};
		}


	}
}

