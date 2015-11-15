using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Json;

namespace VolleyballApp {
	public class DB_Update {
		public DB_Communicator dbCommunicator { get; set; }
		public bool debug { get; set; }

		public DB_Update(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.debug = dbCommunicator.debug;
		}

		/**
		 * Updates a user with the given id with the given parameters.
		 * You can check if the insert was succesful in the state variable.
		 **/
		public async Task<JsonValue> UpdateUser(string host, string name, string role, int number, string position) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/update_userinfo.php" + "?name=" + name 
				+ "&role=" + role + "&number=" + number + "&position=" + position, "DB_Update.UpdateUser()");

			return JsonValue.Parse(responseText);
		}

		public async Task<JsonValue> updateEventState(int id, string state) {
			string responseText = await dbCommunicator.makeWebRequest(
				"service/event/attend_event.php" + "?id=" + id + "&state=" + state, "DB_Update.updateEventState()");

			return JsonValue.Parse(responseText);
		}

		public async Task<JsonValue> inviteUserToEvent(int idEvent, string toInvite) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/invite.php" +
											"?type=users&eventId="+idEvent+"&userIds="+toInvite, "DB_Update.inviteUserToEvent");

			return JsonValue.Parse(responseText);
		}

		public async Task<JsonValue> updateEvent (int idEvent, string name, string location, string start, string end) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/update_event.php" +
				"?idEvent=" + idEvent + "&name=" + name + "&startDate=" + start + "&endDate=" + end + "&location="+ location,
				"DB_Update.updateEvent");

			return JsonValue.Parse(responseText);
		}
	}
}

