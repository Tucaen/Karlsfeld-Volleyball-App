
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

namespace VolleyballApp {
	[Activity(Label = "VolleyballApp", MainLauncher = true, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class MainActivity : AbstractActivity {
		private FragmentTransaction trans;
		public static readonly string EVENTS_FRAGMENT = "EventsFragment", EVENT_DETAILS_FRAGMENT = "EventDetailsFragment",
									ADD_EVENT_FRAGMENT="AddEventFragment", NO_EVENTS_FOUND_FRAGMENT = "NoEventsFoundFragment",
									PROFILE_FRAGMENT="ProfileFragment";

		private string activeFragment;

		protected override void OnCreate(Bundle bundle) {
			// You may use ServicePointManager here
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			base.OnCreate(bundle);

			SetContentView(Resource.Layout.FlyOutContainer);

			startApp();

		}

		private async void startApp() {
			ProgressDialog dialog = base.createProgressDialog("Please Wait!", "Loading...");
			MySqlUser user = MySqlUser.GetUserFromPreferences(this);
			if(user == null) {
				StartActivity(new Intent(this, typeof(LogIn)));
			} else {
				//log in, if the session timed out
				if(DB_Communicator.getInstance().cookieContainer.Count == 0) {
					base.login(user.email, user.password);
				}

				#region Slide Menu
				var menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
				FindViewById (Resource.Id.MenuButton).Click += (sender, e) => {
					menu.AnimatedOpened = !menu.AnimatedOpened;
				};

				FindViewById(Resource.Id.menuProfile).Click += (sender, e) => {
//					ProgressDialog d = base.createProgressDialog("Please Wait!", "");
					Console.WriteLine("Trying to open profile...");
					switchFragment(activeFragment, PROFILE_FRAGMENT, new ProfileFragment());
//					d.Dismiss();

				};

				FindViewById(Resource.Id.menuLogout).Click += (sender, e) => {
					ProgressDialog d = base.createProgressDialog("Please Wait!", "");
					base.logout();
					Finish();
					d.Dismiss();

				};
				#endregion

				await base.loadAndSaveEvents(user, null);
				activeFragment = EVENTS_FRAGMENT;
				trans = FragmentManager.BeginTransaction();
				trans.Add(Resource.Id.fragmentContainer, new EventsFragment(), EVENTS_FRAGMENT);
				trans.Commit();
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

			this.switchFragment(EVENTS_FRAGMENT, EVENT_DETAILS_FRAGMENT, new EventDetailsFragment(e.Position));

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

