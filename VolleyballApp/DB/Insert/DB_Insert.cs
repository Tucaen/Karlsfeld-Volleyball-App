using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VolleyballApp {
	public class DB_Insert {
		public DB_Communicator dbCommunicator { get; set; }
		public HttpClient client { get; set; }
		public bool debug { get; set; }

		public DB_Insert(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.client = dbCommunicator.client;
			this.debug = dbCommunicator.debug;
		}

		/**
		 * Inserts a user with the given parameters and userId = currentHighestId + 1.
		 * You can check if the insert was succesful in the succes variable.
		 **/
		public async Task<bool> InsertUser(string host, string name, string role, string password, int number, string position) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + "php/insertUser.php" + "?name=" + name + "&role=" + role + "&password=" + password + "&number=" + number + "&position=" + position);

			string responseText;
			try {
				response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync();

				if(dbCommunicator.wasSuccesful(responseText)) {
					return true;
				}

				if(debug) {
					Console.WriteLine("Insert response: " + responseText);
				}
			} catch(Exception e) {
				Console.WriteLine("Error while selecting data from MySQL: " + e.Message);
				return false;
			}
			return false;
		}
	}
}

