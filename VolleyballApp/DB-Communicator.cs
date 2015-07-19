using System;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Android.App;
using System.Net.Http;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Client.Methods;
using System.Collections.Generic;


namespace VolleyballApp {
	public class DB_Communicator {
		private bool debug { get; set; }
		private HttpClient client;
		static string host = "http://10.0.3.2/";
		static string requestEventsForUser = "php/requestEventsForUser.php";
		static string requestUserForEvent = "php/requestUserForEvent.php";

		public DB_Communicator() {
			client = new HttpClient();
			debug = false;
		}

		public async void selectUserForEvent() {
//			HttpGet httpGet = new HttpGet(requestEventsForUser);
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + requestUserForEvent);

			string responseText;
			try {
				response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync();

				createUserFromResponse(responseText);
			} catch(Exception e) {
				Console.WriteLine("Error while selecting data from MySQL: " + e.Message);
			}
		}

		private List<MySqlUser> createUserFromResponse(string response) {
			if(wasSuccesful(response)) {
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

		private bool wasSuccesful(string response) {
			return !response.Contains("FAILED");
		}
	}
}
