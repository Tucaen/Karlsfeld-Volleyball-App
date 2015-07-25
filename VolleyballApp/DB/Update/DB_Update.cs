using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VolleyballApp {
	public class DB_Update {
		public DB_Communicator dbCommunicator { get; set; }
		public HttpClient client { get; set; }
		public bool debug { get; set; }
		public string host;

		public DB_Update(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.client = dbCommunicator.client;
			this.debug = dbCommunicator.debug;
			this.host = DB_Communicator.host;
		}

		/**
		 * Updates a user with the given id with the given parameters.
		 * You can check if the insert was succesful in the succes variable.
		 **/
		public async Task<bool> UpdateUser(int idUser, string name, string role, string password, int number, string position) {
			Uri uri = new Uri(host + "php/updateUser.php" + "?idUser=" + idUser + "&name=" + name + "&role=" + role + "&password=" + password + "&number=" + number + "&position=" + position);

			return await executeUpdate(uri);
		}

		public async Task<bool> UpdateState(int idUser, int idEvent, string state) {
			Uri uri = new Uri(host + "php/updateState.php?idUser=" + idUser + "&idEvent=" + idEvent + "&state=" + state);

			return await executeUpdate(uri);
		}

		private async Task<bool> executeUpdate(Uri uri) {
			HttpResponseMessage response = new HttpResponseMessage();
			string responseText;
			try {
				response = await client.GetAsync(uri).ConfigureAwait(continueOnCapturedContext:false);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext:false);

				if(dbCommunicator.wasSuccesful(responseText)) {
					return true;
				}

				if(debug) {
					Console.WriteLine("Update response: " + responseText);
				}
			} catch(Exception e) {
				Console.WriteLine("Error while updating data from MySQL: " + e.Message);
			}
			return false;
		}
	}
}

