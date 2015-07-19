using System;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Android.App;
using System.Net.Http;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Client.Methods;
using System.Collections.Generic;


namespace VolleyballApp {
	public class DB_Communicator {
		public bool debug { get; set; }
		public HttpClient client { get; set; }
		static string host = "http://10.0.3.2/";
		static string requestEventsForUser = "php/requestEventsForUser.php";
		static string requestUserForEvent = "php/requestUserForEvent.php";

		public class State {
			public static string Invited = "eingeladen";
			public static string Accepted = "zugesagt";
			public static string Denied = "abgesagt";
		}

		public DB_Communicator() {
			client = new HttpClient();
			debug = true;
		}

		/**
		 * Provides you with list of all events for a specific user and state.
		 * If state is null all states will be selected.
		 **/
		public List<MySqlEvent> SelectEventsForUser(int idUser, string state) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			dbSelectEvent.SelectEventsForUser(host, requestEventsForUser, idUser, state);
			return dbSelectEvent.listEvent;
		}

		/**
		 * Provides you with list of all users for a specific event and state.
		 * If state is null all states will be selected.
		 **/
		public List<MySqlUser> SelectUserForEvent(int idEvent, string state) {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			dbSelectUser.SelectUserForEvent(host, requestEventsForUser, idEvent, state);
			return dbSelectUser.listUser;
		}

		/**
		 * Inserts a user with the given parameters and userId = currentHighestId + 1
		 **/
		public bool InsertUser(string name, string role, string password, int number, string position) {
			DB_Insert dbInsert = new DB_Insert(this);
			dbInsert.InsertUser(host, name, role, password, number, position);
			return dbInsert.success;
		}

		/**
		 * Deletes a user with the given userId.
		 **/
		public bool DeleteUser(int idUser) {
			DB_Delete dbDelete = new DB_Delete(this);
			dbDelete.DeleteUser(host, idUser);
			return dbDelete.success;

		}

		/**
		 * Updates a user with the given userId with the given parameters.
		 **/
		public bool UpdateUser(int idUser, string name, string role, string password, int number, string position) {
			DB_Update dbUpdate = new DB_Update(this);
			dbUpdate.UpdateUser(host, idUser, name, role, password, number, position);
			return dbUpdate.success;
		}

		/**
		 * Returns true if the mySQL-Statement was succesfully invoked else false.
		 **/
		public bool wasSuccesful(string response) {
			return !response.Contains("FAILED");
		}
	}
}
