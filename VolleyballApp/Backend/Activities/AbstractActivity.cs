
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
using System.Json;

namespace VolleyballApp {
	[Activity(Label = "AbstractLogin")]			
	public abstract class AbstractActivity : Activity {
		DB_Communicator db;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			db = DB_Communicator.getInstance();
		}

		public async Task<bool> login(string username, string password) {
			ProgressDialog dialog = this.createProgressDialog("Please wait!", "Loading...");
			bool wasSuccessful = false;

			MySqlUser user = await db.login(username, password);

			if(user != null) { //login successful
				//storing user information for usage in other activities
				user.StoreUserInPreferences(this, user);

				dialog.Dismiss();
				Toast.MakeText(this, "Login successful!", ToastLength.Short).Show();
				wasSuccessful = true;
			} else { //login failed
				dialog.Dismiss();
				Toast.MakeText(this, "Login failed!", ToastLength.Long).Show();
				wasSuccessful = false;
			}

			return wasSuccessful;
		}

		/*
		 *Starts the FillDataActivity if the state of the user equals 'FILLDATA'
		 *else starts MainActivity.
		 *Also calls Finish().
		 */
		public void proceedAfterManualLogin() {
			MySqlUser user = MySqlUser.GetUserFromPreferences();
			Intent i = null;
			if(user.state.Equals("\"FILLDATA\"") || user.state.Equals("FILLDATA")) {
				i = new Intent(this, typeof(FillDataActivity));
			} else {
				i = new Intent(this, typeof(MainActivity));	
			}
			StartActivity(i);
			Finish();
		}

		/**
		 * Ends the php-session and deletes the logged-in-user out of the preferences.
		 *Than switches to LogIn-Activity.
		 **/
		public async void logout() {
			await db.logout();
			MySqlUser.DeleteUserFromPreferences();
			Intent i = new Intent(this, typeof(LogIn));
			i.AddFlags(ActivityFlags.NoHistory).AddFlags(ActivityFlags.ClearTop);
			StartActivity(i);
		}

//		/**Loads all events for the given user.
//		 **/
//		public async Task<List<MySqlEvent>> loadEvents(MySqlUser user, EventType eventType) {
//			List<MySqlEvent> listEvents = new List<MySqlEvent>();
//			JsonValue json;
//			string alternativeMessage = "";
//
//			if(eventType == EventType.Past) {
//				json = await db.SelectPastEventsForUser(user.idUser, null);
//				alternativeMessage = "Error while loading past events!";
//			} else {
//				json = await db.SelectUpcomingEventsForUser(user.idUser, null);
//				alternativeMessage = "Error while loading upcoming events!";
//			}
//			
//			if(db.wasSuccesful(json)) {
//				listEvents = db.createEventFromResponse(json);
//			} else {
//				this.toastJson(this, json, ToastLength.Long, alternativeMessage);
//			}
////			MySqlEvent.StoreEventListInPreferences(Intent, listEvents);
//			return listEvents;
//		}

		/** Creates a uncancaleable, indeterminate ProgressDialog.
		 *Use to wait till loading data finshed.
		 **/
		public ProgressDialog createProgressDialog(string title, string text) {
			ProgressDialog dialog = ProgressDialog.Show(this, title, text);
			dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
			dialog.SetCancelable(false);
			dialog.Indeterminate = true;
			return dialog;
		}

//		public void toastJson(Context context, JsonValue json, ToastLength length, string alternativeMessage) {
//			string message = (json.ContainsKey("message")) ? json["message"].ToString() : alternativeMessage;
//			Toast.MakeText(context, message, length).Show();
//		}
	}
}

