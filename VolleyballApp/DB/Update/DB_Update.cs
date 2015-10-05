using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Json;

namespace VolleyballApp {
	public class DB_Update {
		public DB_Communicator dbCommunicator { get; set; }
		public HttpClient client { get; set; }
		public bool debug { get; set; }

		public DB_Update(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.client = dbCommunicator.client;
			this.debug = dbCommunicator.debug;
		}

		/**
		 * Updates a user with the given id with the given parameters.
		 * You can check if the insert was succesful in the succes variable.
		 **/
		public async Task<JsonValue> UpdateUser(string host, string name, string role, int number, string position) {
			Uri uri = new Uri(host + "service/user/update_userinfo.php" + "?name=" + name + "&role=" + role + "&number=" + number + "&position=" + position);
			if(debug) 
				Console.WriteLine("DB_Update.UpdateUser() - uri: " + uri);
			
			HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(continueOnCapturedContext:false);
			response.EnsureSuccessStatusCode();
			string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext:false);

			return JsonValue.Parse(responseText);
		}
	}
}

