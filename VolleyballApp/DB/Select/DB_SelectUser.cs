using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Json;
using System.Net;
using System.IO;

namespace VolleyballApp {
	public class DB_SelectUser : DB_Select {
		public DB_SelectUser(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		public async Task<JsonValue> register(string host, string email, string password) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/register.php?email=" + email + "&password="  + password, "DB_SelectUser.register");
			
			return JsonValue.Parse(responseText);
		}

		public async Task<MySqlUser> validateLogin(string host, string username, string password) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/login.php?email=" + username + "&password=" + password, "DB_SelectUser.validateLogin");
			
			MySqlUser user  = createUserFromResponse(responseText);
			return user;
		}


		/**
		 * Concatenate a Uri with the given parameters.
		 * If uri invokation was succesfull a list with all users for the given eventId and state will be created,
		 * which will be stored in the variable listUser.
		 **/
		public async Task<List<MySqlUser>> SelectUserForEvent(string host, int idEvent, string state) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/load_event.php?id=" + idEvent + "&loadAttendences=true", "DB_SelectUser.SelectUserForEvent");

			return createUserListFromResponse(responseText);
		}

		/**
		 * Creates a MySqlUser object for the logged in user.
		 **/
		private MySqlUser createUserFromResponse(string response) {
			JsonValue json = JsonValue.Parse(response);
			JsonValue user = json["data"]["User"];

			return new MySqlUser(dbCommunicator.convertAndInitializeToInt(
				dbCommunicator.containsKey(user, "id", DB_Communicator.JSON_TYPE_INT)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "name", DB_Communicator.JSON_TYPE_STRING)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "email", DB_Communicator.JSON_TYPE_STRING)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "state", DB_Communicator.JSON_TYPE_STRING)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "role", DB_Communicator.JSON_TYPE_STRING)),
				"",
				dbCommunicator.convertAndInitializeToInt(dbCommunicator.containsKey(user, "number", DB_Communicator.JSON_TYPE_INT)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "position", DB_Communicator.JSON_TYPE_STRING)));
		}


		/**Creates a list of user who attend an event*/
		private List<MySqlUser> createUserListFromResponse(string response) {
			JsonValue json = JsonValue.Parse(response);
			List<MySqlUser> listUser = new List<MySqlUser>();
			JsonValue attendences = json["data"][0]["Event"]["attendences"];

			foreach (JsonValue u in attendences) {
				JsonValue user = u["Attendence"]["userObj"]["User"];
				listUser.Add(new MySqlUser(dbCommunicator.convertAndInitializeToInt(
					dbCommunicator.containsKey(user, "id", DB_Communicator.JSON_TYPE_INT)),
					dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "name", DB_Communicator.JSON_TYPE_STRING)),
					dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "email", DB_Communicator.JSON_TYPE_STRING)),
					dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "state", DB_Communicator.JSON_TYPE_STRING)),
					dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "role", DB_Communicator.JSON_TYPE_STRING)),
					"",
					dbCommunicator.convertAndInitializeToInt(dbCommunicator.containsKey(user, "number", DB_Communicator.JSON_TYPE_INT)),
					dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(user, "position", DB_Communicator.JSON_TYPE_STRING))));
			}

			return listUser;
		}
	}
}

