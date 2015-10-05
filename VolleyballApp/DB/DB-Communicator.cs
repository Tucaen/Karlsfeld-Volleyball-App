using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Java.Security.Cert;
using Android.App;
using Javax.Net.Ssl;
using Java.Security;
using System.Json;


namespace VolleyballApp {
	public class DB_Communicator {
		public bool debug { get; set; }
		public HttpClient client { get; set; }
		static string host = "https://psymax.onthewifi.com:10815/"; 
//		static string host = "http://10.0.3.2/";

		public class State {
			public static string Invited = "eingeladen";
			public static string Accepted = "zugesagt";
			public static string Denied = "abgesagt";
		}

		public DB_Communicator() {
			client = new HttpClient();
			debug = true;
		}

		public async Task<MySqlUser> login(string username, string password) {
			DB_SelectUser dbUser = new DB_SelectUser(this);
			return await dbUser.validateLogin(host, username, password).ConfigureAwait(continueOnCapturedContext:false);
		}

		public async Task<JsonValue> register(string email, string password) {
			DB_SelectUser dbUser = new DB_SelectUser(this);
			return await dbUser.register(host, email, password).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Provides you with list of all events for a specific user and state.
		 * If state is null all states will be selected.
		 **/
		public async Task<List<MySqlEvent>> SelectEventsForUser(int idUser, string state) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			return await dbSelectEvent.SelectEventsForUser(host, idUser, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Provides you with list of all users for a specific event and state.
		 * If state is null all states will be selected.
		 **/
		public async Task<List<MySqlUser>> SelectUserForEvent(int idEvent, string state) {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.SelectUserForEvent(host, idEvent, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Inserts a user with the given parameters and userId = currentHighestId + 1
		 **/
		public async Task<bool> InsertUser(string name, string role, string password, int number, string position) {
			DB_Insert dbInsert = new DB_Insert(this);
			return await dbInsert.InsertUser(host, name, role, password, number, position);
		}

		/**
		 * Deletes a user with the given userId.
		 **/
		public async Task<bool> DeleteUser(int idUser) {
			DB_Delete dbDelete = new DB_Delete(this);
			return await dbDelete.DeleteUser(host, idUser);

		}

		/**
		 * Updates a user with the given userId with the given parameters.
		 **/
		public async Task<JsonValue> UpdateUser(string name) {
			return await UpdateUser(name, "", 0, "");
		}
		public async Task<JsonValue> UpdateUser(string name, string role, int number, string position) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.UpdateUser(host, name, role, number, position);
		}
//		public async void UpdateUser(int idUser, string name, string role, string password, int number, string position) {
//			DB_Update dbUpdate = new DB_Update(this);
//			dbUpdate.UpdateUser(host, idUser, name, role, password, number, position);
////			return dbUpdate.UpdateUser(host, idUser, name, role, password, number, position);
//		}

		/**
		 * Returns true if the mySQL-Statement was succesfully invoked else false.
		 **/
		public bool wasSuccesful(string response) {
			return !response.Contains("FAILED");
		}

		public string convertAndInitializeToString(JsonValue value) {
			return (value == null) ? "" : value.ToString();
		}

		public int convertAndInitializeToInt(JsonValue value) {
			return (value == null) ? 0 : Convert.ToInt32(value.ToString());
		}
	}
}
