using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Json;
using System.Net;
using System.IO;
using System.Linq;
using Android.Widget;

namespace VolleyballApp {
	public class DB_SelectUser : DB_Select {
		public DB_SelectUser(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		public async Task<JsonValue> SelectAllUser() {
			string responseText = await dbCommunicator.makeWebRequest("service/user/load_user.php", "DB_SelectUser.SelectAllUser()");
			return JsonValue.Parse(responseText);
		}

		public async Task<JsonValue> logout() {
			string responseText = await dbCommunicator.makeWebRequest("service/user/logout.php", "DB_SelectUser.logout()");
			return JsonValue.Parse(responseText);
		}

		public async Task<JsonValue> register(string host, string email, string password) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/register.php?email=" + email + "&password="  + password, "DB_SelectUser.register");
			
			return JsonValue.Parse(responseText);
		}

		public async Task<MySqlUser> validateLogin(string host, string username, string password) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/login.php?email=" + username + "&password=" + password, "DB_SelectUser.validateLogin");
//			try {
				MySqlUser user  = createUserFromResponse(JsonValue.Parse(responseText), password)[0];
				if(debug)
					Console.WriteLine("DB_SelectUser.validateLogin - user = " + user);
				return user;
//			} catch (Exception) {
//				return null;
//			}
			
		}

		/**
		 *Returns a list with all users for the given eventId and state
		 **/
		public async Task<List<MySqlUser>> SelectUserForEvent(string host, int idEvent, string state) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/load_event.php?id=" + idEvent + "&loadAttendences=true", "DB_SelectUser.SelectUserForEvent");

			return createUserAttendanceListFromResponse(responseText);
		}

		/**
		 * Creates a List<MySqlUser> object from a JsonValue.
		 * And saves the given password for automatic relogin.
		 **/
		public List<MySqlUser> createUserFromResponse(JsonValue json, string password) {
			List<MySqlUser> listUser = new List<MySqlUser>();
			if(dbCommunicator.wasSuccesful(json)) {
				try {
					foreach (JsonValue u in json["data"]) {
						JsonValue user = u["User"];
						JsonValue teamrole = (user["teamroles"].Count > 0) ? user["teamroles"][0]["TeamRole"] : null;
						listUser.Add( new MySqlUser(
							dbCommunicator.convertAndInitializeToInt(dbCommunicator.containsKey(user, "id", DB_Communicator.JSON_TYPE_INT)),
							dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "name", DB_Communicator.JSON_TYPE_STRING)),
							dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "email", DB_Communicator.JSON_TYPE_STRING)),
							dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "state", DB_Communicator.JSON_TYPE_STRING)),
							password,
							createTeamrole(teamrole)
						));
					}
				} catch(Exception) { //es wurde nur ein User und kein Array zurückgegeben
					JsonValue user = json["data"]["User"];
					JsonValue teamrole;
					if(user["teamroles"] is JsonObject) {
						teamrole = user["teamroles"]["TeamRole"];
					} else {
						teamrole = (user["teamroles"].Count > 0) ? user["teamroles"][0]["TeamRole"] : null;
					}
					listUser.Add(new MySqlUser(
						dbCommunicator.convertAndInitializeToInt(dbCommunicator.containsKey(user, "id", DB_Communicator.JSON_TYPE_INT)),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "name", DB_Communicator.JSON_TYPE_STRING)),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "email", DB_Communicator.JSON_TYPE_STRING)),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "state", DB_Communicator.JSON_TYPE_STRING)),
						password,
						createTeamrole(teamrole)
					));
				}

			}
			List<MySqlUser> sortedList = listUser.OrderBy(u => u.name).ToList();
			return sortedList;
		}

		/**Creates a list of user who attend an event*/
		private List<MySqlUser> createUserAttendanceListFromResponse(string response) {
			JsonValue json = JsonValue.Parse(response);
			List<MySqlUser> listUser = new List<MySqlUser>();
			if(dbCommunicator.wasSuccesful(json)) {
				JsonValue attendences = json["data"][0]["Event"]["attendences"];
				
				foreach (JsonValue u in attendences) {
					JsonValue user = u["Attendence"]["userObj"]["User"];
					JsonValue teamrole = (user["teamroles"].Count > 0) ? user["teamroles"][0]["TeamRole"] : null;
					listUser.Add(new MySqlUser(
						dbCommunicator.convertAndInitializeToInt(dbCommunicator.containsKey(user, "id", DB_Communicator.JSON_TYPE_INT)),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "name", DB_Communicator.JSON_TYPE_STRING)),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "email", DB_Communicator.JSON_TYPE_STRING)),
						"", //state e.g. "FILLDATA" or "FINAL"; isn't send
						"", //password; isn't send
						createTeamrole(teamrole),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(u["Attendence"], "state", DB_Communicator.JSON_TYPE_STRING))));
				}
			}
			return listUser.OrderBy(u => u.eventState).ToList();
		}

		private MySqlTeamrole createTeamrole(JsonValue json) {
			return new MySqlTeamrole(
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(json, "userType", DB_Communicator.JSON_TYPE_STRING)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(json, "role", DB_Communicator.JSON_TYPE_STRING)),
				dbCommunicator.convertAndInitializeToInt(dbCommunicator.containsKey(json, "number", DB_Communicator.JSON_TYPE_INT)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(json, "position", DB_Communicator.JSON_TYPE_STRING)) );
		}
	}
}

