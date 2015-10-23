
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

		public async void login(string username, string password) {
			ProgressDialog dialog = this.createProgressDialog("Please wait!", "Loading...");

			MySqlUser user = await db.login(username, password);

			if(user != null) { //login successful
				//storing user information for usage in other activities
				user.StoreUserInPreferences(this, user);

				Toast.MakeText(this, "Login successful!", ToastLength.Short).Show();

				Intent i = null;
				await this.loadAndSaveEvents(user, null);
				if(user.state.Equals("\"FILLDATA\"") || user.state.Equals("FILLDATA")) {
					i = new Intent(this, typeof(FillDataActivity));
				} else {
					i = new Intent(this, typeof(MainActivity));	
				}
				dialog.Dismiss();
				StartActivity(i);
				Finish();
			} else { //login failed
				dialog.Dismiss();
				Toast.MakeText(this, "Login failed!", ToastLength.Long).Show();
			}
		}

		/**Loads all events for the given user and state.
		 *If state is null, all states are loaded.
		 *This method also saves loaded evetns in preferneces.
		 **/
		public async Task<List<MySqlEvent>> loadAndSaveEvents(MySqlUser user, string state) {
			List<MySqlEvent> listEvents = new List<MySqlEvent>();
			JsonValue json = await db.SelectEventsForUser(user.idUser, state);
			if(db.wasSuccesful(json)) {
				listEvents = db.createEventFromResponse(json);
			} else {
				Toast.MakeText(this, json["message"].ToString(), ToastLength.Long).Show();
			}
			MySqlEvent.StoreEventListInPreferences(Intent, listEvents);
			return listEvents;
		}

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
	}
}

