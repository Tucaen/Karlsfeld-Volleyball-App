using System;
using System.Net.Http;
using System.Collections.Generic;

namespace VolleyballApp {
	public class DB_SelectUser : DB_Select {
		public List<MySqlUser> listUser { get; set; }

		public DB_SelectUser(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		public async void SelectUserForEvent(string host, string requestUserForEvent, int idEvent, string state) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + requestUserForEvent + "?idEvent=" + idEvent + "&state="  + state);

			string responseText;
			try {
				response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync();

				listUser = createUserFromResponse(responseText);
			} catch(Exception e) {
				Console.WriteLine("Error while selecting data from MySQL: " + e.Message);
			}
		}

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

