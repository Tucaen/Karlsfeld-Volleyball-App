using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Widget;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;

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

		public async Task<JsonValue> register(string email, string password) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/register.php?email=" + email + "&password="  + password, "DB_SelectUser.register");
			
			return JsonValue.Parse(responseText);
		}

		public async Task<VBUser> validateLogin(string username, string password) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/login.php?email=" + username + "&password=" + password, "DB_SelectUser.validateLogin");
			List<VBUser> listUser = createUserFromResponse(JsonValue.Parse(responseText), password);
			if(listUser.Count > 0) {
				if(debug)
					Console.WriteLine("DB_SelectUser.validateLogin - user = " + listUser[0]);
				return listUser[0];
			} else {
				return null;
			}
			
		}

		/**
		 *Returns a list with all users for the given eventId and state
		 **/
		public async Task<List<VBUser>> SelectUserForEvent(int idEvent, string state) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/load_event.php?id=" + idEvent + "&loadAttendences=true", "DB_SelectUser.SelectUserForEvent");

			return createUserAttendanceListFromResponse(responseText);
		}

		/**
		 * Creates a List<MySqlUser> object from a JsonValue.
		 * And saves the given password for automatic relogin.
		 **/
		public List<VBUser> createUserFromResponse(JsonValue json, string password) {
			List<VBUser> listUser = new List<VBUser>();
			if(dbCommunicator.wasSuccesful(json)) {
				if(json["data"] is JsonArray) {
					foreach (JsonValue u in json["data"]) {
						JsonValue user = u["User"];
					
						listUser.Add(createUserFromJson(user, password));
					}
				} else { //es wurde nur ein User und kein Array zurückgegeben
					JsonValue user = json["data"]["User"];

					listUser.Add(createUserFromJson(user, password));
				}

			}
			List<VBUser> sortedList = listUser.OrderBy(u => u.name).ToList();
			return sortedList;
		}

		/**Creates a list of user who attend an event*/
		private List<VBUser> createUserAttendanceListFromResponse(string response) {
			JsonValue json = JsonValue.Parse(response);
			List<VBUser> listUser = new List<VBUser>();
			if(dbCommunicator.wasSuccesful(json)) {
				JsonValue attendences = json["data"][0]["Event"]["attendences"];
				
				foreach (JsonValue u in attendences) {
					JsonValue user = u["Attendence"]["userObj"]["User"];
					VBUser temp = createUserFromJson(user, "");
					temp.setEventState(u["Attendence"]["state"]);
					listUser.Add(temp);
				}
			}
			return listUser.OrderBy(u => u.getEventState()).ToList();
		}

		private VBUser createUserFromJson(JsonValue json, string password) {
			VBUser vbuser = new VBUser(json);
			vbuser.password = password;

			return vbuser;
		}
	}
}

