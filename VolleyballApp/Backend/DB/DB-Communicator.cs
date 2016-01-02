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
			return await dbSelectUser.validateLogin(username, password).ConfigureAwait(continueOnCapturedContext:false);
		}

		public async Task<JsonValue> logout() {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.logout();
		}

		public async Task<JsonValue> register(string email, string password) {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.register(email, password).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Provides you with list of all events for a specific user and state.
		 *If state is null all states will be selected.
		 **/
		public async Task<JsonValue> SelectUpcomingEventsForUser(int idUser, string state) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			return await dbSelectEvent.SelectUpcomingEventsForUser(idUser, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		public async Task<JsonValue> SelectPastEventsForUser(int idUser, string state) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			return await dbSelectEvent.SelectPastEventsForUser(idUser, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		public async Task<JsonValue> SelectAllUser() {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.SelectAllUser().ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Updates a user with the given userId with the given parameters.
		 **/
		public async Task<JsonValue> UpdateUser(string name, string userType) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.UpdateUser(name, userType);
		}
		public async Task<JsonValue> UpdateUser(string name, string role, int number, string position, int teamId) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.UpdateUser(name, role, number, position, teamId);
		}

		public List<VBUser> createUserFromResponse(JsonValue json) {
			return this.createUserFromResponse(json, "");
		}

		public List<VBUser> createUserFromResponse(JsonValue json, string password) {
			DB_SelectUser selectUser = new DB_SelectUser(this);
			return selectUser.createUserFromResponse(json, password);
		}

		public async Task<JsonValue> loadUninvtedUser(int eventId) {
			string service = "service/event/load_uninvited_user.php?eventId=" + eventId;
			return JsonValue.Parse(await this.makeWebRequest(service, "DB_Communicator.loadUninvtedUser"));
		}

		public async Task<string> createUserTypeRequest(int userId, int teamId, string userType) {
			string service = "service/user/create_request.php?userId=" + userId + "&teamId=" + teamId + "&userType=" + userType;
			return await this.makeWebRequest(service, "DB_Communicator.createUserTypeRequest");
		}

		public async Task<string> loadUserTypeRequest(int teamId) {
			string service = "service/user/load_requestedUserTypes.php?teamId=" + teamId;
			return await this.makeWebRequest(service, "DB_Communicator.loadUserTypeRequest");
		}

		public async Task<string> handleUserTypeRequest(VBRequest request, string answer) {
			string service = "service/user/handleRequest.php?teamId=" + request.teamId + "&userId=" + request.userId + "&answer=" + answer +
			                 "&userType=" + request.getUserType().ToString().Substring(0, 1);
			return await this.makeWebRequest(service, "TeamDetailsFragment.handleUserTypeRequest");
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
			return await dbSelectUser.SelectUserForEvent(idEvent, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		public List<VBEvent> createEventFromResponse(JsonValue json) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			return dbSelectEvent.createEventFromResponse(json);
		}

		public async Task<JsonValue> deleteEvent(int id) {
			DB_Delete db = new DB_Delete(this);
			return await db.deleteEvent(id);
		}

		public async Task<JsonValue> createEvent(string name, string location, string start, string end, string info, int teamId) {
			DB_InsertEvent dbInsertEvent = new DB_InsertEvent(this);
			return await dbInsertEvent.createEvent(name, location, start, end, info, teamId);
		}

		public async Task<JsonValue> createEvent(int teamId, string name, string location, string start, string end) {
			DB_InsertEvent dbInsertEvent = new DB_InsertEvent(this);
			return await dbInsertEvent.createEvent(teamId, name, location, start, end);
		}

		public async Task<JsonValue> inviteUserToEvent(int idEvent, string toInvite) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.inviteUserToEvent(idEvent, toInvite);
		}

		public async Task<JsonValue> updateEvent(int idEvent, string name, string location, string start, string end, string info, int teamId) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.updateEvent(idEvent, name, location, start, end, info, teamId);
		}
		#endregion

		#region team
		public async Task<VBTeam> SelectTeam(int idTeam) {
			DB_SelectTeam dbSelectTeam = new DB_SelectTeam(this);
			return await dbSelectTeam.SelectTeam(idTeam);
		}

		public async Task<List<VBTeam>> SelectTeams() {
			DB_SelectTeam dbSelectTeam = new DB_SelectTeam(this);
			return await dbSelectTeam.SelectTeams();
		}

		public async Task<JsonValue> createTeam(string name, string sport, string location, string description) {
			DB_SelectTeam dbSelectTeam = new DB_SelectTeam(this);
			return await dbSelectTeam.createTeam(name, sport, location, description);
		}

		public async Task<string> loadMember(int teamId) {
			string service = "service/team/load_teams.php?id=" + teamId + "&loadMember=true";
			return await this.makeWebRequest(service, "RequestUserTypeDialog.loadMember");
		}

		public async Task<string> deleteTeam(int teamId) {
			string service = "service/team/delete_team.php?id=" + teamId;
			return await this.makeWebRequest(service, "RequestUserTypeDialog.deleteTeam");
		}

		public async Task<string> updateTeam(VBTeam team) {
			string service = "service/team/update_team.php?id=" + team.id + "&name=" + team.name + "&sport=" + team.sport + 
								"&location=" + team.location + "&description=" + team.description;
			return await this.makeWebRequest(service, "RequestUserTypeDialog.updateTeam");
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
						Console.WriteLine(type + " - FATAL ERROR: Error with php-script! Message: " + we.Message);
					responseText = "{\"state\":\"error\",\"code\":\"n\\/a\",\"message\":\"Error at: "+
						type + " Message: " + we.Message + "\",\"data\":{}}";
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
		public bool isAtLeast(UserType ut, UserType userType) {
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
				if(ut.Equals(UserType.Admin) || ut.Equals(UserType.Operator) || ut.Equals(UserType.Coremember) || ut.Equals(UserType.Member) || ut.Equals(UserType.Fan))
					return true;
				return false;
			default :
				return false;
			}
		}

		public List<VBRequest> createReqeuestList(JsonValue json) {
			if(this.wasSuccesful(json)) {
				List<VBRequest> listRequests = new List<VBRequest>();
				foreach(JsonValue request in json["data"]) {
					listRequests.Add(new VBRequest(request["UserTypeRequest"]));
				}
				return listRequests;
			}
			return null;
		}

		public List<VBUser> createMemberList(JsonValue json) {
			if(this.wasSuccesful(json)) {
				List<VBUser> list = new List<VBUser>();
				foreach(JsonValue user in json["data"][0]["Team"]["member"]) {
					list.Add(new VBUser(user["User"]));
				}
				return list;
			}
			return null;
		}
		#endregion
	}
}
