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
		public const int JSON_TYPE_INT = 0, JSON_TYPE_STRING = 1, JSON_TYPE_DATE = 2;
		public static DB_Communicator db;
		public bool debug { get; set; }
		public CookieContainer cookieContainer { get; set; }
		static string host = "https://psymax.onthewifi.com:10815/";
		public bool IsOnline {get; set;}
//		static string localhost = "http://10.0.3.2/";

		public static class State {
			public static string Invited = "Eingeladen";
			public static string Accepted = "Zugesagt";
			public static string Denied = "Abgesagt";
		}

		public DB_Communicator() {
			this.cookieContainer = new CookieContainer();
			this.debug = true;
		}

		public static DB_Communicator getInstance() {
			if(db == null)
				db = new DB_Communicator();
			return db;
		}

		#region user
		public async Task<VBUser> login(string username, string password) {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.validateLogin(host, username, password).ConfigureAwait(continueOnCapturedContext:false);
		}

		public async Task<JsonValue> logout() {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.logout();
		}

		public async Task<JsonValue> register(string email, string password) {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.register(host, email, password).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Provides you with list of all events for a specific user and state.
		 *If state is null all states will be selected.
		 **/
		public async Task<JsonValue> SelectUpcomingEventsForUser(int idUser, string state) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			return await dbSelectEvent.SelectUpcomingEventsForUser(host, idUser, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		public async Task<JsonValue> SelectPastEventsForUser(int idUser, string state) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			return await dbSelectEvent.SelectPastEventsForUser(host, idUser, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		public async Task<JsonValue> SelectAllUser() {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.SelectAllUser().ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Updates a user with the given userId with the given parameters.
		 **/
		public async Task<JsonValue> UpdateUser(string name) {
			return await UpdateUser(name, "", 0, "", 1);
		}
		public async Task<JsonValue> UpdateUser(string name, string role, int number, string position, int teamId) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.UpdateUser(host, name, role, number, position, teamId);
		}

		public List<VBUser> createUserFromResponse(JsonValue json) {
			return this.createUserFromResponse(json, "");
		}

		public List<VBUser> createUserFromResponse(JsonValue json, string password) {
			DB_SelectUser selectUser = new DB_SelectUser(this);
			return selectUser.createUserFromResponse(json, password);
		}
		#endregion

		#region event
		public async Task<JsonValue> updateEventState(int idEvent, string state) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.updateEventState(idEvent, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Provides you with list of all users for a specific event and state.
		 *If state is null all states will be selected.
		 **/
		public async Task<List<VBUser>> SelectUserForEvent(int idEvent, string state) {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.SelectUserForEvent(host, idEvent, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		public List<VBEvent> createEventFromResponse(JsonValue json) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			return dbSelectEvent.createEventFromResponse(json);
		}

		public async Task<JsonValue> deleteEvent(int id) {
			DB_Delete db = new DB_Delete(this);
			return await db.deleteEvent(id);
		}

		public async Task<JsonValue> createEvent(string name, string location, string start, string end, string info) {
			DB_InsertEvent dbInsertEvent = new DB_InsertEvent(this);
			return await dbInsertEvent.createEvent(name, location, start, end, info);
		}

		public async Task<JsonValue> createEvent(int teamId, string name, string location, string start, string end) {
			DB_InsertEvent dbInsertEvent = new DB_InsertEvent(this);
			return await dbInsertEvent.createEvent(teamId, name, location, start, end);
		}

		public async Task<JsonValue> inviteUserToEvent(int idEvent, string toInvite) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.inviteUserToEvent(idEvent, toInvite);
		}

		public async Task<JsonValue> updateEvent(int idEvent, string name, string location, string start, string end, string info) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.updateEvent(idEvent, name, location, start, end, info);
		}
		#endregion

		#region team
		public async Task<List<VBTeam>> SelectTeams() {
			DB_SelectTeam dbSelectTeam = new DB_SelectTeam(this);
			return await dbSelectTeam.SelectTeams(host);
		}

		public async Task<List<VBTeam>> SelectTeamsForUser(int idUser) {
			DB_SelectTeam dbSelectTeam = new DB_SelectTeam(this);
			return await dbSelectTeam.SelectTeamsForUser(host, idUser);
		}
		#endregion

		#region general
		/**
		 * Returns true if the mySQL-Statement was succesfully invoked else false.
		 **/
		public bool wasSuccesful(JsonValue json) {
			return json["state"].ToString().Equals("\"ok\"") || json["state"].ToString().Equals("ok")
				|| json["state"].ToString().Equals("\"warning\"") || json["state"].ToString().Equals("warning");
		}

		public string convertAndInitializeToString(JsonValue value) {
			return (value == null) ? "" : value.ToString().Replace("\"", "");
		}

		public int convertAndInitializeToInt(JsonValue value) {
			return (value == null || value.ToString().Replace("\"", "") == "") ? 0 : Convert.ToInt32(value.ToString().Replace("\"", ""));
		}

		public DateTime convertAndInitializeToDateTime(JsonValue value) {
			return (value == null) ? new DateTime() : Convert.ToDateTime(value.ToString().Replace("\"", ""));
		}

		public JsonValue containsKey(JsonValue value, string key, int type) {
			JsonPrimitive nullValue = new JsonPrimitive("");
			switch(type) {
			case JSON_TYPE_INT:
				nullValue = new JsonPrimitive(0);
				break;
			case 1:
				nullValue = new JsonPrimitive("");
				break;
			case 2:
				nullValue = new JsonPrimitive(new DateTime());
				break;
			}
			if(value == null)
				return nullValue;
			return (value.ContainsKey(key)) ? value[key] : nullValue;
		}

		public async Task<string> makeWebRequest(string phpService, string type) {
			string responseText = "";

			if(!IsOnline) {
				responseText = "{\"state\":\"error\",\"message\":\"You are not connected to the internet!.\"}";
			} else {
				Uri uri = new Uri(host + phpService);
				if(debug) 
					Console.WriteLine(type + " - uri: " + uri);
				
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
				request.CookieContainer = cookieContainer;
				
				try {
					WebResponse response = await request.GetResponseAsync().ConfigureAwait(continueOnCapturedContext:false);
					StreamReader sr = new StreamReader(response.GetResponseStream());
					responseText = sr.ReadToEnd();
				} catch (WebException we) {
					if(debug) 
						Console.WriteLine(type + " - FATAL ERROR: Error with php-script! Source: " + we.Source);
					responseText = "{\"state\":\"error\",\"code\":\"n\\/a\",\"message\":\"Error with php-script! "+
						type + "\",\"data\":{}}";
				}
				
				if(debug) 
					Console.WriteLine(type + " - response: " + responseText);
			}

			return responseText;
		}

		/**
		 * Determines if the user has the necessary permission
		 * Admin->Operator->Coremember->Member->Fan
		 */
		public bool isAtLeast(VBUser user, UserType userType) {
			UserType ut = user.teamRole.getUserType();

			switch(userType) {
			case UserType.Admin:
				if(ut.Equals(UserType.Admin))
					return true;
				return false;
			case UserType.Operator:
				if(ut.Equals(UserType.Admin) || ut.Equals(UserType.Operator))
					return true;
				return false;
			case UserType.Coremember:
				if(ut.Equals(UserType.Admin) || ut.Equals(UserType.Operator) || ut.Equals(UserType.Coremember))
					return true;
				return false;
			case UserType.Member:
				if(ut.Equals(UserType.Admin) || ut.Equals(UserType.Operator) || ut.Equals(UserType.Coremember) || ut.Equals(UserType.Member))
					return true;
				return false;
			case UserType.Fan:
				return true;
			default :
				return false;
			}
		}
		#endregion
	}
}
