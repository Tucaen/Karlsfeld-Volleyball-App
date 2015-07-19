using System;
using System.Net.Http;

namespace VolleyballApp {
	public class DB_Update {
		public DB_Communicator dbCommunicator { get; set; }
		public HttpClient client { get; set; }
		public bool debug { get; set; }
		public bool success { get; set; }

		public DB_Update(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.client = dbCommunicator.client;
			this.debug = dbCommunicator.debug;
			this.success = false;
		}

		/**
		 * Updates a user with the given id with the given parameters.
		 * You can check if the insert was succesful in the succes variable.
		 **/
		public async void UpdateUser(string host, int idUser, string name, string role, string password, int number, string position) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + "php/updateUser.php" + "?idUser=" + idUser + "&name=" + name + "&role=" + role + "&password=" + password + "&number=" + number + "&position=" + position);

			string responseText;
			try {
				response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync();
				this.success = true;

				if(debug) {
					Console.WriteLine("Update response: " + responseText);
				}
			} catch(Exception e) {
				Console.WriteLine("Error while updating data from MySQL: " + e.Message);
				this.success = false;
			}
		}
	}
}

