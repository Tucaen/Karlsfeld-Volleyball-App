using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Json;

namespace VolleyballApp {
	public class DB_SelectUser : DB_Select {
		public DB_SelectUser(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		public async Task<JsonValue> register(string host, string email, string password) {
//			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + "service/user/register.php?email=" + email + "&password="  + password);
			if(debug) 
				Console.WriteLine("Registration uri: " + uri);

			HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(continueOnCapturedContext:false);
			response.EnsureSuccessStatusCode();
			string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext:false);

			if(debug) 
				Console.WriteLine("DB_SelectUser.register() - response: " + responseText);
			
			return JsonValue.Parse(responseText);
		}

		public async Task<MySqlUser> validateLogin(string host, string username, string password) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + "service/user/login.php?email=" + username + "&password="  + password);
			if(debug) 
				Console.WriteLine("Login uri: " + uri);

			MySqlUser user = null;
			string responseText;
			try {
				response = await client.GetAsync(uri).ConfigureAwait(continueOnCapturedContext:false);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext:false);
				
				if(debug) 
					Console.WriteLine("Login response: " + responseText);
				
				user  = createUserFromResponse(responseText)[0];
			} catch(Exception e) {
				Console.WriteLine("Error while loging in: " + e.Message + "\n Source: "  + e.InnerException + " | " + e.StackTrace);
			}
			return user;
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
			JsonValue json = JsonValue.Parse(response);
			List<MySqlUser> listUser = new List<MySqlUser>();
			JsonValue user = json["data"]["User"];
			Console.WriteLine("DB_SelectUser.createUserFromResponse() - json data: " + json["data"].ToString());

			listUser.Add(new MySqlUser(dbCommunicator.convertAndInitializeToInt(user["id"]),
				dbCommunicator.convertAndInitializeToString(user["name"]),
				dbCommunicator.convertAndInitializeToString(user["email"]),
				dbCommunicator.convertAndInitializeToString(user["state"]),
				dbCommunicator.convertAndInitializeToString(user["role"]),
				"",
				dbCommunicator.convertAndInitializeToInt(user["number"]),
				dbCommunicator.convertAndInitializeToString(user["position"])));

			return listUser;
		}
	}
}

