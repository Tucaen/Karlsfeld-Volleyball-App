
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using VolleyballAp;

namespace VolleyballApp {
	[Activity(Label = "VolleyballApp", Icon="@drawable/VolleyballApp_Logo", MainLauncher = true,
		Theme = "@android:style/Theme.Holo.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]			
	public class MainActivity : AbstractActivity {
		private FlyOutContainer menu;
		private int eventPosition;
		public FragmentTransaction trans { get; private set; }
		private string activeFragment;

		protected override void OnCreate(Bundle bundle) {
			// You may use ServicePointManager here
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			base.OnCreate(bundle);

			SetContentView(Resource.Layout.FlyOutContainer);
		}

		protected override void OnResume() {
			base.OnResume();
			NetworkInfo activeConnection = ((ConnectivityManager) GetSystemService(ConnectivityService)).ActiveNetworkInfo;
			bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
			DB_Communicator.getInstance().IsOnline = isOnline;
			startApp();
		}

		private async void startApp() {
			ProgressDialog dialog = base.createProgressDialog("Please Wait!", "Checking login data...");
			ViewController.getInstance().mainActivity = this;

			MySqlUser.context = this;
			MySqlUser user = MySqlUser.GetUserFromPreferences();
			if(user == null) {
				Intent i = new Intent(this, typeof(LogIn));
				i.AddFlags(ActivityFlags.NoHistory).AddFlags(ActivityFlags.ClearTop);
				StartActivity(i);
			} else {
				//log in, if the session timed out
				if(DB_Communicator.getInstance().cookieContainer.Count == 0) {
					dialog.SetMessage("Log in...");
					if(!await base.login(user.email, user.password)) {
						Intent i = new Intent(this, typeof(LogIn));
						i.AddFlags(ActivityFlags.NoHistory).AddFlags(ActivityFlags.ClearTop);
						StartActivity(i);
					}
				}

				//set up push-notifications
				dialog.SetMessage("Check services...");
				if (ViewController.getInstance().IsPlayServicesAvailable ()) {
					var intent = new Intent (this, typeof (RegistrationIntentService));
					StartService (intent);
				}

				if(activeFragment == null) {
					switch(this.Intent.Action) {
					case MyGcmListenerService.PUSH_EVENT_UPDATE:
						//do same as for PUSH_INVITE
					case MyGcmListenerService.PUSH_INVITE:
						dialog.SetMessage("Load event details...");
						MySqlEvent e = await ViewController.getInstance().refreshDataForEvent(ViewController.getInstance().pushEventId);
						List<MySqlUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(ViewController.getInstance().pushEventId, "");
						this.initalizeFragment(ViewController.EVENT_DETAILS_FRAGMENT, new EventDetailsFragment(e, listUser));
						break;

					default:
						dialog.SetMessage("Load events...");
						List<MySqlEvent> listEvents = await ViewController.getInstance().loadEvents(user, EventType.Upcoming);
						this.initalizeFragment(ViewController.UPCOMING_EVENTS_FRAGMENT, new EventsFragment(listEvents));
						break;
					}
				}

				#region Slide Menu
				menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
				FindViewById (Resource.Id.MenuButton).Click += (sender, e) => {
					menu.AnimatedOpened = !menu.AnimatedOpened;
				};

				FindViewById(Resource.Id.menuProfile).Click += (sender, e) => {
					ProgressDialog d = base.createProgressDialog("Please Wait!", "");
					switchFragment(activeFragment, ViewController.PROFILE_FRAGMENT, new ProfileFragment());
					d.Dismiss();
				};

				FindViewById(Resource.Id.menuEventsUpcoming).Click += async (sender, e) => {
					ProgressDialog d = base.createProgressDialog("Please Wait!", "Loading...");
					List<MySqlEvent> listEvents = await ViewController.getInstance().loadEvents(user, EventType.Upcoming);
					switchFragment(activeFragment, ViewController.UPCOMING_EVENTS_FRAGMENT, new EventsFragment(listEvents));
					d.Dismiss();
				};

				FindViewById(Resource.Id.menuEventsPast).Click += async (sender, e) => {
					ProgressDialog d = base.createProgressDialog("Please Wait!", "Loading...");
					List<MySqlEvent> listEvents = await ViewController.getInstance().loadEvents(user, EventType.Past);
					switchFragment(activeFragment, ViewController.PAST_EVENTS_FRAGMENT, new EventsFragment(listEvents));
					d.Dismiss();
				};

				FindViewById(Resource.Id.menuLogout).Click += (sender, e) => {
					ProgressDialog d = base.createProgressDialog("Please Wait!", "");
					base.logout();
					d.Dismiss();

				};
				#endregion
			}
			dialog.Dismiss();
		}

		private void initalizeFragment(string activeFragmentTag, Fragment frag) {
			activeFragment = activeFragmentTag;
			trans = FragmentManager.BeginTransaction();
			trans.Add(Resource.Id.fragmentContainer, frag, activeFragmentTag);
			trans.AddToBackStack(activeFragmentTag);
			trans.CommitAllowingStateLoss();
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
			Fragment oldFragment = FragmentManager.FindFragmentByTag(oldFragmentTag);

			if(addToBackStack && oldFragment != null) //des is null obwohls nicht null sein sollte, weil EventFragemnt dse oldFragment nicht akutalliesiert
				trans.AddToBackStack(oldFragmentTag);

			if(oldFragment != null)
				trans.Remove(oldFragment);
			trans.Add(Resource.Id.fragmentContainer, newFragment, newFragmentTag);
			trans.Commit();
		}

		public void popBackstack() {
			if(FragmentManager.BackStackEntryCount > 0)
				activeFragment = FragmentManager.GetBackStackEntryAt(FragmentManager.BackStackEntryCount - 1).Name;
			else
				activeFragment = null;
			
			FragmentManager.PopBackStackImmediate();
		}

		public async void OnListEventClicked(AdapterView.ItemClickEventArgs e, List<MySqlEvent> listEvents) {
			ProgressDialog dialog = base.createProgressDialog("Please wait!", "Loading...");
			this.eventPosition = e.Position;
			MySqlEvent clickedEvent = listEvents[eventPosition];

			List<MySqlUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(clickedEvent.idEvent, "");
//			MySqlUser.StoreUserListInPreferences(this.Intent, listUser);

			this.switchFragment(ViewController.UPCOMING_EVENTS_FRAGMENT, ViewController.EVENT_DETAILS_FRAGMENT, new EventDetailsFragment(clickedEvent, listUser));

			dialog.Dismiss();
		}

		public override void OnBackPressed() {
			if(FragmentManager.BackStackEntryCount > 0)
				base.OnBackPressed();
			else
				Finish();
		}

		public Fragment FindFragmentByTag(string tag) {
			return FragmentManager.FindFragmentByTag(tag);
		}
	}
}

