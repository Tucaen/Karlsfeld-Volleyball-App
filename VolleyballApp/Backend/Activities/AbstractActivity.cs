
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
using Android.Util;

namespace VolleyballApp {
	[Activity(Label = "AbstractLogin")]			
	public abstract class AbstractActivity : Activity {
		DB_Communicator db;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			db = DB_Communicator.getInstance();
		}

		public async Task<bool> login(string username, string password) {
//			ProgressDialog dialog = this.createProgressDialog("Please wait!", "Loading...");
			bool wasSuccessful = false;

			VBUser user = await db.login(username, password);

			if(user != null) { //login successful
				//storing user information for usage in other activities
				user.StoreUserInPreferences(this, user);

//				dialog.Dismiss();
				Toast.MakeText(this, "Login successful!", ToastLength.Short).Show();
				wasSuccessful = true;
			} else { //login failed
//				dialog.Dismiss();
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
			VBUser user = VBUser.GetUserFromPreferences();
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
			await this.deleteToken();
			await db.logout();
			VBUser.DeleteUserFromPreferences();
			Intent i = new Intent(this, typeof(LogIn));
			i.AddFlags(ActivityFlags.NoHistory).AddFlags(ActivityFlags.ClearTop);
			StartActivity(i);
		}

		public async Task<JsonValue> deleteToken() {
			//delete token from database
			int userId = VBUser.GetUserFromPreferences().idUser;
			JsonValue json = JsonValue.Parse(await DB_Communicator.getInstance().makeWebRequest("service/user/delete_token.php?userId=" + userId + "&token=" + ViewController.getInstance().token, "AbstractActivity.deleteToken()"));
			Log.Info("AbstractActivity.deleteToken", "json message: " + json["message"].ToString());
			return json;
			//token should be deleted from GCM automatically
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

