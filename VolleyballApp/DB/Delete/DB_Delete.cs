using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VolleyballApp {
	public class DB_Delete {
		public DB_Communicator dbCommunicator { get; set; }
		public HttpClient client { get; set; }
		public bool debug { get; set; }

		public DB_Delete(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.client = dbCommunicator.client;
			this.debug = dbCommunicator.debug;
		}

		/**
		 * Delets a user with the given userId.
		 * You can check if the insert was succesful in the succes variable.
		 **/
		public async Task<bool> DeleteUser(string host, int idUser) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + "php/deleteUser.php" + "?idUser=" + idUser);

			string responseText;
			try {
				response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync();

				if(dbCommunicator.wasSuccesful(responseText)) {
					return true;
				}

				if(debug) {
					Console.WriteLine("Delete response: " + responseText);
				}
			} catch(Exception e) {
				Console.WriteLine("Error while selecting data from MySQL: " + e.Message);
				return false;
			}
			return false;
		}
	}
}

