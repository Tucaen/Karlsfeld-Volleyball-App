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
		 **/
		public async Task<bool> DeleteUser(int idUser) {
			Uri uri = new Uri(DB_Communicator.host + "php/deleteUser.php" + "?idUser=" + idUser);
			return await executeDelete(uri);
		}

		public async Task<bool> DeleteEvent(int idEvent) {
			Uri uri = new Uri(DB_Communicator.host + "php/deleteEvent.php" + "?idEvent=" + idEvent);
			return await executeDelete(uri);
		}

		private async Task<bool> executeDelete(Uri uri) {
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
					Console.WriteLine("Delete response: " + responseText);
				}
			} catch(Exception e) {
				Console.WriteLine("Error while deleting data from MySQL: " + e.Message);
				return false;
			}
			return false;
		}
	}
}

