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
	}
}

