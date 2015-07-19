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

		public class State {
			public static string Invited = "eingeladen";
			public static string Accepted = "zugesagt";
			public static string Denied = "abgesagt";
		}

		public DB_Communicator() {
			client = new HttpClient();
			debug = true;
		}

		public async void SelectEventsForUser(int idUser, string state) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + requestEventsForUser + "?idUser=" + idUser + "&state=" + state);

			string responseText;
			try {
				response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync();

				createEventFromResponse(responseText);
			} catch(Exception e) {
				Console.WriteLine("Error while selecting data from MySQL: " + e.Message);
			}
		}


		public async void SelectUserForEvent(int idEvent, string state) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + requestUserForEvent + "?idEvent=" + idEvent + "&state="  + state);

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

		private List<MySqlEvent> createEventFromResponse(string response) {
			if(wasSuccesful(response)) {
				string[] eventInfo = response.Split('|');
				List<MySqlEvent> listEvent = new List<MySqlEvent>();

				int i = 0;
				do {
					if(debug) {
						Console.WriteLine("Creating Event: " + eventInfo[i] + " " + eventInfo[i + 1] + " " + eventInfo[i + 2] + " " + eventInfo[i + 3] + " " + eventInfo[i + 4]);
					}
					listEvent.Add(new MySqlEvent(Convert.ToInt32(eventInfo[i]), eventInfo[i + 1], Convert.ToDateTime(eventInfo[i + 2]), Convert.ToDateTime(eventInfo[i + 3]), eventInfo[i + 4]));
					i += 5;
				} while(!eventInfo[i].Equals("<endoffile>")) ;

				return listEvent;
			} else {
				Console.WriteLine("Invalid response for creating event!");
				return null;
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
