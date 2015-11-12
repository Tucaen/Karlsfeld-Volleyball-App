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
using System.Threading.Tasks;
using System.Net;
using Android.Gms.Common;

namespace VolleyballApp {
	[Activity(Label = "VolleyballApp", MainLauncher = true, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class MainActivity : AbstractActivity {
		private FlyOutContainer menu;
		public FragmentTransaction trans { get; private set; }
		public static readonly string EVENTS_FRAGMENT = "EventsFragment", EVENT_DETAILS_FRAGMENT = "EventDetailsFragment",
									ADD_EVENT_FRAGMENT="AddEventFragment", NO_EVENTS_FOUND_FRAGMENT = "NoEventsFoundFragment",
									PROFILE_FRAGMENT="ProfileFragment";

		private string activeFragment;

		protected override void OnCreate(Bundle bundle) {
			// You may use ServicePointManager here
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			base.OnCreate(bundle);

			SetContentView(Resource.Layout.FlyOutContainer);

			//set up push-notifications | commented out at the moment cause server side is missing
			if (IsPlayServicesAvailable ()) {
				var intent = new Intent (this, typeof (RegistrationIntentService));
				StartService (intent);
			}

			startApp();
		}

		public bool IsPlayServicesAvailable () {
			string type = "PushNotification.IsPlayServicesAvailable()";
			GoogleApiAvailability gaa = GoogleApiAvailability.Instance;
			int resultCode = gaa.IsGooglePlayServicesAvailable(this);

			if (resultCode != ConnectionResult.Success) {
				if(gaa.IsUserResolvableError(resultCode)) {
					Console.WriteLine(type + " - " + gaa.GetErrorString(resultCode));
					gaa.GetErrorDialog(this, resultCode, 0).Show();
				} else {
					Console.WriteLine(type + " - Sorry, this device is not supported");
					Finish ();
				}

				return false;
			}
			else {
				Console.WriteLine(type + " - Google Play Services is available.");
				return true;
			}
		}

		private async void startApp() {
			ProgressDialog dialog = base.createProgressDialog("Please Wait!", "Loading...");
			MySqlUser user = MySqlUser.GetUserFromPreferences(this);
			if(user == null) {
				Intent i = new Intent(this, typeof(LogIn));
				i.AddFlags(ActivityFlags.NoHistory).AddFlags(ActivityFlags.ClearTop);
				StartActivity(i);
			} else {
				//log in, if the session timed out
				Console.WriteLine("cookieContainer.Count = " + DB_Communicator.getInstance().cookieContainer.Count);
				if(DB_Communicator.getInstance().cookieContainer.Count == 0) {
					if(!await base.login(user.email, user.password)) {
						Intent i = new Intent(this, typeof(LogIn));
						i.AddFlags(ActivityFlags.NoHistory).AddFlags(ActivityFlags.ClearTop);
						StartActivity(i);
					}
				}

				await base.loadAndSaveEvents(user, null);

				activeFragment = EVENTS_FRAGMENT;
				trans = FragmentManager.BeginTransaction();
				trans.Add(Resource.Id.fragmentContainer, new EventsFragment(), EVENTS_FRAGMENT);
				trans.Commit();

				#region Slide Menu
				menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
				FindViewById (Resource.Id.MenuButton).Click += (sender, e) => {
					menu.AnimatedOpened = !menu.AnimatedOpened;
				};

				FindViewById(Resource.Id.menuProfile).Click += (sender, e) => {
					ProgressDialog d = base.createProgressDialog("Please Wait!", "");
					switchFragment(activeFragment, PROFILE_FRAGMENT, new ProfileFragment());
					d.Dismiss();
				};

				FindViewById(Resource.Id.menuEvents).Click += async (sender, e) => {
					ProgressDialog d = base.createProgressDialog("Please Wait!", "Loading...");
					await base.loadAndSaveEvents(user, null);
					switchFragment(activeFragment, EVENTS_FRAGMENT, new EventsFragment());
					d.Dismiss();
				};

				FindViewById(Resource.Id.menuLogout).Click += (sender, e) => {
					ProgressDialog d = base.createProgressDialog("Please Wait!", "");
					base.logout();
//					Finish();
					d.Dismiss();

				};
				#endregion
			}
			dialog.Dismiss();
		}

		/**
		 * Replaces the old fragment with the new one and adds the old to the backstack
		 **/
		public void switchFragment(string oldFragmentTag, string newFragmentTag, Fragment newFragment) {
			switchFragment(oldFragmentTag, newFragmentTag, newFragment, true);
		}

		/**
		 * Replaces the old fragment with the new one and adds the old to the backstack,
		 *if bool addToBackStack is set to true
		 **/
		public void switchFragment(string oldFragmentTag, string newFragmentTag, Fragment newFragment, bool addToBackStack) {
			menu.Opened = false;
			activeFragment = newFragmentTag;
			trans = FragmentManager.BeginTransaction();
			if(addToBackStack)
				trans.AddToBackStack(oldFragmentTag);
			trans.Replace(Resource.Id.fragmentContainer, newFragment, newFragmentTag);
			trans.Commit();
		}

		public async void OnListEventClicked(AdapterView.ItemClickEventArgs e, List<MySqlEvent> listEvents) {
			ProgressDialog dialog = base.createProgressDialog("Please wait!", "Loading...");

			MySqlEvent clickedEvent = listEvents[e.Position];

			List<MySqlUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(clickedEvent.idEvent, "");
			MySqlUser.StoreUserListInPreferences(this.Intent, listUser);

			this.switchFragment(EVENTS_FRAGMENT, EVENT_DETAILS_FRAGMENT, new EventDetailsFragment(e.Position, clickedEvent));

			dialog.Dismiss();
		}

		public async void refreshEventDetailsFragment(string fragmentTag, int position) {
			List<MySqlEvent> listEvents = await refreshEvents();
			List<MySqlUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(listEvents[position].idEvent, "");
			MySqlUser.StoreUserListInPreferences(this.Intent, listUser);

			refreshFragment(fragmentTag);
		}

		public async Task<List<MySqlEvent>> refreshEvents() {
			return await base.loadAndSaveEvents(MySqlUser.GetUserFromPreferences(this), "");
		}

		public void refreshFragment(string fragmentTag) {
			Fragment frag = FragmentManager.FindFragmentByTag(fragmentTag);
			trans = FragmentManager.BeginTransaction();
			trans.Detach(frag);
			trans.Attach(frag);
			trans.Commit();
		}
	}
}
