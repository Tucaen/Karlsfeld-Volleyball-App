
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
	[Activity(Label = "VolleyballApp", MainLauncher = true, Icon = "@drawable/icon")]			
	public class MainActivity : AbstractActivity {
		FragmentTransaction trans;
		public static readonly string EVENTS_FRAGMENT = "EventsFragment", EVENT_DETAILS_FRAGMENT = "EventDetailsFragment",
		ADD_EVENT_FRAGMENT="AddEventFragment", NO_EVENTS_FOUND_FRAGMENT = "NoEventsFoundFragment";

		protected override void OnCreate(Bundle bundle) {
			// You may use ServicePointManager here
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

			startApp();

		}

		private async void startApp() {
			ProgressDialog dialog = base.createProgressDialog("Please Wait!", "Loading...");
			MySqlUser user = MySqlUser.GetUserFromPreferences(this);
			if(user == null) {
				StartActivity(new Intent(this, typeof(LogIn)));
			} else {
				await base.loadAndSaveEvents(user, null);
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

