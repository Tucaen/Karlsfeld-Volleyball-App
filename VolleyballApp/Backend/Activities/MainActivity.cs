
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
			startApp();
		}

		private async void startApp() {
			try {
				ProgressDialog dialog = base.createProgressDialog("Please Wait!", "Checking login data...");
				ViewController.getInstance().mainActivity = this;
				
				VBUser.context = this;
				VBUser user = VBUser.GetUserFromPreferences();
				if(user == null) {
					Intent i = new Intent(this, typeof(LogIn));
					i.AddFlags(ActivityFlags.NoHistory).AddFlags(ActivityFlags.ClearTop);
					StartActivity(i);
				} else {
					//log in, if the session timed out
					dialog.SetMessage("Log in...");
					await DB_Communicator.getInstance().refreshLogin();
					
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
							VBEvent e = await ViewController.getInstance().refreshDataForEvent(ViewController.getInstance().pushEventId);
							List<VBUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(ViewController.getInstance().pushEventId, "");
							this.initalizeFragment(ViewController.EVENT_DETAILS_FRAGMENT, new EventDetailsFragment(e, listUser));
							break;
							
						default:
							dialog.SetMessage("Load events...");
							List<VBEvent> listEvents = await ViewController.getInstance().loadEvents(user, EventType.Upcoming);
							this.initalizeFragment(ViewController.UPCOMING_EVENTS_FRAGMENT, new EventsFragment(listEvents));
							break;
						}
					}
					
					#region Slide Menu
					menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
					FindViewById (Resource.Id.MenuButton).Click += (sender, e) => {
						menu.AnimatedOpened = !menu.AnimatedOpened;
					};
					
					FindViewById(Resource.Id.menuProfile).SetOnClickListener(new SlideMenuClickListener(SlideMenuClickListener.ON_PROFILE, this));
					
					FindViewById<LinearLayout>(Resource.Id.menuTeam).SetOnClickListener(new SlideMenuClickListener(SlideMenuClickListener.ON_TEAM, this));
					
					FindViewById(Resource.Id.menuEventsUpcoming).SetOnClickListener(new SlideMenuClickListener(SlideMenuClickListener.ON_UPCOMING_EVENTS, this));
					
					FindViewById(Resource.Id.menuEventsPast).SetOnClickListener(new SlideMenuClickListener(SlideMenuClickListener.ON_PAST_EVENTS, this));
					
					FindViewById(Resource.Id.menuLogout).SetOnClickListener(new SlideMenuClickListener(SlideMenuClickListener.ON_LOGOUT, this));
					#endregion
				}
				dialog.Dismiss();
			} catch (System.Exception e) {
				Toast.MakeText(this, "Error while trying to resume the app! " + e.Message, ToastLength.Long);
			}
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

		public async void OnListEventClicked(AdapterView.ItemClickEventArgs e, List<VBEvent> listEvents) {
			ProgressDialog dialog = base.createProgressDialog("Please wait!", "Loading...");
			this.eventPosition = e.Position;
			VBEvent clickedEvent = listEvents[eventPosition];

			List<VBUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(clickedEvent.idEvent, "");

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
	
		class SlideMenuClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
			public const string ON_PROFILE = "onProfile", ON_TEAM = "onTeam", ON_UPCOMING_EVENTS = "onUpcomingEvents",
								ON_PAST_EVENTS = "onPastEvents", ON_LOGOUT = "onLogout";
			private string source;
			private MainActivity t;

			public SlideMenuClickListener(string source, MainActivity t) {
				this.source = source;
				this.t = t;
			}

			public void OnClick(View view) {
				switch(this.source) {
				case ON_PROFILE:
					this.onProfile();
					break;
				case ON_TEAM:
					this.onTeam();
					break;
				case ON_UPCOMING_EVENTS:
					this.onUpcomingEvents();
					break;
				case ON_PAST_EVENTS:
					this.onPastEvents();
					break;
				case ON_LOGOUT:
					this.onLogout();
					break;
				}
			}

			private void onProfile() {
				ProgressDialog d = t.createProgressDialog("Please Wait!", "");
				t.switchFragment(t.activeFragment, ViewController.PROFILE_FRAGMENT, new ProfileFragment());
				d.Dismiss();
			}


			private async void onTeam() {
				ProgressDialog d = t.createProgressDialog("Please Wait!", "");
				List<VBTeam> list = await DB_Communicator.getInstance().SelectTeams();
				t.switchFragment(t.activeFragment, ViewController.TEAMS_FRAGMENT, new TeamsFragment(list));
				d.Dismiss();
			}

			private async void onUpcomingEvents() {
				ProgressDialog d = t.createProgressDialog("Please Wait!", "Loading...");
				List<VBEvent> listEvents = await ViewController.getInstance().loadEvents(VBUser.GetUserFromPreferences(), EventType.Upcoming);
				t.switchFragment(t.activeFragment, ViewController.UPCOMING_EVENTS_FRAGMENT, new EventsFragment(listEvents));
				d.Dismiss();
			}

			private async void onPastEvents() {
				ProgressDialog d = t.createProgressDialog("Please Wait!", "Loading...");
				List<VBEvent> listEvents = await ViewController.getInstance().loadEvents(VBUser.GetUserFromPreferences(), EventType.Past);
				t.switchFragment(t.activeFragment, ViewController.PAST_EVENTS_FRAGMENT, new EventsFragment(listEvents));
				d.Dismiss();
			}

			private void onLogout() {
				ProgressDialog d = t.createProgressDialog("Please Wait!", "");
				t.logout();
				d.Dismiss();
			}
		}
	}
}

