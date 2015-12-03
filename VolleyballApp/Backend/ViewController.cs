using System;
using System.Threading.Tasks;
using Android.App;
using System.Collections.Generic;
using System.Json;
using Android.Widget;
using Android.Content;
using Android.Gms.Common;
using Java.Lang;

namespace VolleyballApp {
	public class ViewController {
		private static ViewController Instance;
		public MainActivity mainActivity { private get; set; }

		public int pushEventId { get; set; }

		public static readonly string UPCOMING_EVENTS_FRAGMENT = "UpcomingEventsFragment", EVENT_DETAILS_FRAGMENT = "EventDetailsFragment",
										ADD_EVENT_FRAGMENT="AddEventFragment", NO_EVENTS_FOUND_FRAGMENT = "NoEventsFoundFragment",
										PROFILE_FRAGMENT="ProfileFragment", EDIT_EVENT_FRAGMENT = "EditEventFragment",
										PAST_EVENTS_FRAGMENT="PastEventsFragment";


		private ViewController() {
		}

		public static ViewController getInstance() {
			if(Instance == null) {
				Instance = new ViewController();
			}
			return Instance;
		}

		public async Task<List<MySqlEvent>> refreshEvents() {
			EventType eventType = this.GetEventTypeFromBackstack();
			Fragment eventsFragment = null;
			List<MySqlEvent> listEvents = new List<MySqlEvent>();

			if(eventType != EventType.Unknown) {
				listEvents = await loadEvents(MySqlUser.GetUserFromPreferences(), eventType);

				if(eventType == EventType.Upcoming)
					eventsFragment = mainActivity.FindFragmentByTag(UPCOMING_EVENTS_FRAGMENT);
				if(eventType == EventType.Past)
					eventsFragment = mainActivity.FindFragmentByTag(PAST_EVENTS_FRAGMENT);

				if(eventsFragment != null)
					(eventsFragment as EventsFragment).listEvents = listEvents;
			} else {
				listEvents = await loadEvents(MySqlUser.GetUserFromPreferences(), EventType.Upcoming);
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
		public async Task<List<MySqlEvent>> loadEvents(MySqlUser user, EventType eventType) {
			DB_Communicator db = DB_Communicator.getInstance();
			List<MySqlEvent> listEvents = new List<MySqlEvent>();
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
			string message = (json.ContainsKey("message")) ? json["message"].ToString() : alternativeMessage;
			Toast.MakeText(context, message, length).Show();
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

		public async Task<MySqlEvent> refreshDataForEvent(int idEvent) {
			List<MySqlUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(idEvent, "");
			MySqlUser.StoreUserListInPreferences(mainActivity.Intent, listUser);

			List<MySqlEvent> listEvents = await this.refreshEvents();
			foreach(MySqlEvent e in listEvents) {
				if(e.idEvent == idEvent)
					return e;
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
	}
}

