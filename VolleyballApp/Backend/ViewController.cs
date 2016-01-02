using System;
using System.Threading.Tasks;
using Android.App;
using System.Collections.Generic;
using System.Json;
using Android.Widget;
using Android.Content;
using Android.Gms.Common;
using Java.Lang;
using Android.Views.InputMethods;
using System.Linq;

namespace VolleyballApp {
	public class ViewController {
		private static ViewController Instance;
		public MainActivity mainActivity { get; set; }

		public int pushEventId { get; set; }
		public string token { get; set; }

		public static readonly string UPCOMING_EVENTS_FRAGMENT = "UpcomingEventsFragment", EVENT_DETAILS_FRAGMENT = "EventDetailsFragment",
			ADD_EVENT_FRAGMENT="AddEventFragment", NO_EVENTS_FOUND_FRAGMENT = "NoEventsFoundFragment", PROFILE_FRAGMENT="ProfileFragment",
			EDIT_EVENT_FRAGMENT = "EditEventFragment", PAST_EVENTS_FRAGMENT="PastEventsFragment", TEAMS_FRAGMENT="TeamsFragment",
			ADD_TEAM_FRAGMENT="AddTeamFragment", TEAM_DETAILS_FRAGMENT="TeamDetailsFragment", EDIT_TEAM_FRAGMENT = "EditTeamFragment";

		private ViewController() {
		}

		public static ViewController getInstance() {
			if(Instance == null) {
				Instance = new ViewController();
			}
			return Instance;
		}

		public async Task<List<VBEvent>> refreshEvents() {
			EventType eventType = this.GetEventTypeFromBackstack();
			Fragment eventsFragment = null;
			List<VBEvent> listEvents = new List<VBEvent>();

			if(eventType != EventType.Unknown) {
				listEvents = await loadEvents(VBUser.GetUserFromPreferences(), eventType);

				if(eventType == EventType.Upcoming)
					eventsFragment = mainActivity.FindFragmentByTag(UPCOMING_EVENTS_FRAGMENT);
				if(eventType == EventType.Past)
					eventsFragment = mainActivity.FindFragmentByTag(PAST_EVENTS_FRAGMENT);

				if(eventsFragment != null)
					(eventsFragment as EventsFragment).listEvents = listEvents;
			} else {
				listEvents = await loadEvents(VBUser.GetUserFromPreferences(), EventType.Upcoming);
			}

			return listEvents;
		}

		private EventType GetEventTypeFromBackstack() {
			int i = mainActivity.FragmentManager.BackStackEntryCount - 1;
			EventType eventType = EventType.Unknown;

			if(i >= 0) {
				try {
					string name = mainActivity.FragmentManager.GetBackStackEntryAt(i).Name;
					switch(name) {
					case "UpcomingEventsFragment":
						eventType = EventType.Upcoming;
						break;
					case "PastEventsFragment":
						eventType = EventType.Past;
						break;
					}
				} catch (NullPointerException) {
					return eventType;
				}
			}

			return eventType;
		}

		/**Loads all events for the given user.
		 **/
		public async Task<List<VBEvent>> loadEvents(VBUser user, EventType eventType) {
			DB_Communicator db = DB_Communicator.getInstance();
			List<VBEvent> listEvents = new List<VBEvent>();
			JsonValue json;
			string alternativeMessage = "";

			if(eventType == EventType.Past) {
				json = await db.SelectPastEventsForUser(user.idUser, null);
				alternativeMessage = "Error while loading past events!";
			} else {
				json = await db.SelectUpcomingEventsForUser(user.idUser, null);
				alternativeMessage = "Error while loading upcoming events!";
			}

			if(db.wasSuccesful(json)) {
				listEvents = db.createEventFromResponse(json);
			} else {
				this.toastJson(mainActivity, json, ToastLength.Long, alternativeMessage);
			}

			return listEvents;
		}

		public void toastJson(Context context, JsonValue json, ToastLength length, string alternativeMessage) {
			Context c = (context != null) ? context : mainActivity;
			string message = (json.ContainsKey("message")) ? json["message"].ToString() : alternativeMessage;
			Toast.MakeText(c, message, length).Show();
		}

		/**
		 * ONLY use this method when fragment is not switched!
		 **/
		public void  refreshFragment(string fragmentTag) {
			Fragment frag = mainActivity.FindFragmentByTag(fragmentTag);
			FragmentTransaction trans = mainActivity.FragmentManager.BeginTransaction();
			trans.Detach(frag);
			trans.Attach(frag);
			trans.Commit();
		}

		public async Task<VBEvent> refreshDataForEvent(int idEvent) {
			//macht keinen sinn mehr UserList wird nicht mher in den Preferences gespeichert, sondern direkt an EventDetailsFragment übergeben
//			List<MySqlUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(idEvent, "");
//			MySqlUser.StoreUserListInPreferences(mainActivity.Intent, listUser);

			List<VBEvent> listEvents = await this.refreshEvents();
			foreach(VBEvent e in listEvents) {
				if(e.idEvent == idEvent) {
					return e;
				}
			}

			return null;
		}

		/*
		 * Formates a dd.MM.yyyy string to yyyy-MM-dd
		 */
		public string convertDateForDb(string date) {
			string[] temp = date.Split('.');
			return temp[2] + "-" + temp[1] + "-" + temp[0];
		}

		public void hideSoftKeyboard() {
			InputMethodManager inputMethodManager = (InputMethodManager)  this.mainActivity.GetSystemService(Activity.InputMethodService);
			inputMethodManager.HideSoftInputFromWindow(this.mainActivity.CurrentFocus.WindowToken, 0);
		}

		public bool IsPlayServicesAvailable () {
			string type = "IsPlayServicesAvailable()";
			GoogleApiAvailability gaa = GoogleApiAvailability.Instance;
			int resultCode = gaa.IsGooglePlayServicesAvailable(mainActivity);

			if (resultCode != ConnectionResult.Success) {
				if(gaa.IsUserResolvableError(resultCode)) {
					Console.WriteLine(type + " - " + gaa.GetErrorString(resultCode));
					gaa.GetErrorDialog(mainActivity, resultCode, 0).Show();
				} else {
					Console.WriteLine(type + " - Sorry, this device is not supported");
//					Finish ();
				}

				return false;
			}
			else {
				Console.WriteLine(type + " - Google Play Services is available.");
				return true;
			}
		}
	
		public List<VBUser> sortUserlistForTeam(List<VBUser> list, int teamId) {
			List<VBUser> sortedList = list.OrderBy(u => u.getTeamroleForTeam(teamId).position.Equals("Keine") || u.getTeamroleForTeam(teamId).position.Equals("")).
				ThenBy(u => u.getTeamroleForTeam(teamId).position.Equals("Steller")).
				ThenBy(u => u.getTeamroleForTeam(teamId).position.Equals("Mittelblocker")).
				ThenBy(u => u.getTeamroleForTeam(teamId).position.Equals("Libero")).
				ThenBy(u => u.getTeamroleForTeam(teamId).position.Equals("Diagonalangreifer")).
				ThenBy(u => u.getTeamroleForTeam(teamId).position.Equals("Außenangreifer")).
				ThenBy(u => u.name). 
				ToList();
			return sortedList;
		}
	}
}

