using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VolleyballApp {
	public class DB_SelectUser : DB_Select {
		public DB_SelectUser(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		public async Task<bool> validateLogin(string host, string username, string password) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + "php/validateLogin.php?username=" + username + "&password="  + password);

			string responseText;
			try {
				response = await client.GetAsync(uri).ConfigureAwait(continueOnCapturedContext:false);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext:false);
				if(debug) {
					Console.WriteLine("Login response: " + responseText);
				}
				if(dbCommunicator.wasSuccesful(responseText)) {
					return true;
				}
			} catch(Exception e) {
				Console.WriteLine("Error while loging in: " + e.Message);
				return false;
			}
			return false;
		}


		/**
		 * Concatenate a Uri with the given parameters.
		 * If uri invokation was succesfull a list with all users for the given eventId and state will be created,
		 * which will be stored in the variable listUser.
		 **/
		public async Task<List<MySqlUser>> SelectUserForEvent(string host, int idEvent, string state) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + "php/requestUserForEvent.php?idEvent=" + idEvent + "&state="  + state);

			List<MySqlUser> listUser = null;
			string responseText;
			try {
				response = await client.GetAsync(uri).ConfigureAwait(continueOnCapturedContext:false);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext:false);

				listUser = createUserFromResponse(responseText);
			} catch(Exception e) {
				Console.WriteLine("Error while selecting data from MySQL: " + e.Message);
			}
			return listUser;
		}

		/**
		 * Creates a MySqlUser object for every row in the response string.
		 **/
		private List<MySqlUser> createUserFromResponse(string response) {
			if(base.dbCommunicator.wasSuccesful(response)) {
				string[] userInfo = response.Split('|');
				List<MySqlUser> listUser = new List<MySqlUser>();

				int i = 0;
				do {
					if(debug) {
						Console.WriteLine("Creating User: " + userInfo[i] + " " + userInfo[i + 1] + " " + userInfo[i + 2] + " " + userInfo[i + 3] + " " + userInfo[i + 4] + " " + userInfo[i + 5]);
					}
					listUser.Add(new MySqlUser(Convert.ToInt32(userInfo[i]), userInfo[i + 1], userInfo[i + 2], userInfo[i + 3], Convert.ToInt32(userInfo[i + 4]), userInfo[i + 5]));
					i += 6;
				} while(!userInfo[i].Equals("<endoffile>")) ;

				return listUser;
			} else {
				Console.WriteLine("Invalid response for creating user!");
				return null;
			}
		}
	}
}

