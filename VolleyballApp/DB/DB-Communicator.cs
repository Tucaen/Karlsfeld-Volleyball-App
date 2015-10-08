using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Java.Security.Cert;
using Android.App;
using Javax.Net.Ssl;
using Java.Security;
using System.Json;


namespace VolleyballApp {
	public class DB_Communicator {
		public static int JSON_TYPE_INT = 0, JSON_TYPE_STRING = 1;
		public static DB_Communicator db;
		public bool debug { get; set; }
		public HttpClient client { get; set; }
		public CookieContainer cookieContainer { get; set; }
		static string host = "https://psymax.onthewifi.com:10815/"; 
//		static string host = "http://10.0.3.2/";

		public class State {
			public static string Invited = "eingeladen";
			public static string Accepted = "zugesagt";
			public static string Denied = "abgesagt";
		}

		public DB_Communicator() {
			client = new HttpClient();
			cookieContainer = new CookieContainer();
			debug = true;
		}

		public static DB_Communicator getInstance() {
			if(db == null)
				db = new DB_Communicator();
			return db;
		}

		public async Task<MySqlUser> login(string username, string password) {
			DB_SelectUser dbUser = new DB_SelectUser(this);
			return await dbUser.validateLogin(host, username, password).ConfigureAwait(continueOnCapturedContext:false);
		}

		public async Task<JsonValue> register(string email, string password) {
			DB_SelectUser dbUser = new DB_SelectUser(this);
			return await dbUser.register(host, email, password).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Provides you with list of all events for a specific user and state.
		 * If state is null all states will be selected.
		 **/
		public async Task<List<MySqlEvent>> SelectEventsForUser(int idUser, string state) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			return await dbSelectEvent.SelectEventsForUser(host, idUser, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Provides you with list of all users for a specific event and state.
		 * If state is null all states will be selected.
		 **/
		public async Task<List<MySqlUser>> SelectUserForEvent(int idEvent, string state) {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			return await dbSelectUser.SelectUserForEvent(host, idEvent, state).ConfigureAwait(continueOnCapturedContext:false);
		}

		/**
		 * Updates a user with the given userId with the given parameters.
		 **/
		public async Task<JsonValue> UpdateUser(string name) {
			return await UpdateUser(name, "", 0, "");
		}
		public async Task<JsonValue> UpdateUser(string name, string role, int number, string position) {
			DB_Update dbUpdate = new DB_Update(this);
			return await dbUpdate.UpdateUser(host, name, role, number, position);
		}

		/**
		 * Returns true if the mySQL-Statement was succesfully invoked else false.
		 **/
		public bool wasSuccesful(JsonValue json) {
			return json["state"].ToString().Equals("\"ok\"");
		}

		public string convertAndInitializeToString(JsonValue value) {
			return (value == null) ? "" : value.ToString();
		}

		public int convertAndInitializeToInt(JsonValue value) {
			return (value == null) ? 0 : Convert.ToInt32(value.ToString());
		}

		public JsonValue containsKey(JsonValue value, string key, int type) {
			JsonValue nullValue = (type == JSON_TYPE_STRING) ? new JsonPrimitive("") : new JsonPrimitive(0);
			return (value.ContainsKey(key)) ? value[key] : nullValue;
		}

		public async Task<string> makeWebRequest(string phpService, string type) {
			Uri uri = new Uri(host + phpService);
			if(debug) 
				Console.WriteLine(type + " - uri: " + uri);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.CookieContainer = cookieContainer;
			Console.WriteLine("CookieContainer.Count before GetResponse: " + request.CookieContainer.Count + " | " + cookieContainer.Count);

			WebResponse response = await request.GetResponseAsync().ConfigureAwait(continueOnCapturedContext:false);
			Console.WriteLine("CookieContainer.Count after GetResponse: " + request.CookieContainer.Count + " | " + cookieContainer.Count);
			StreamReader sr = new StreamReader(response.GetResponseStream());
			string responseText = sr.ReadToEnd();

			if(debug) 
				Console.WriteLine(type + " - response: " + responseText);
			
			return responseText;
		}
	}
}
