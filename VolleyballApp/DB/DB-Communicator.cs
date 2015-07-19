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
		public bool debug { get; set; }
		public HttpClient client { get; set; }
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

		public List<MySqlEvent> SelectEventsForUser(int idUser, string state) {
			DB_SelectEvent dbSelectEvent = new DB_SelectEvent(this);
			dbSelectEvent.SelectEventsForUser(host, requestEventsForUser, idUser, state);
			return dbSelectEvent.listEvent;
		}


		public async List<MySqlUser> SelectUserForEvent(int idEvent, string state) {
			DB_SelectUser dbSelectUser = new DB_SelectUser(this);
			dbSelectUser.SelectUserForEvent(host, requestEventsForUser, idEvent, state);
			return dbSelectUser.listUser;
		}

		public bool wasSuccesful(string response) {
			return !response.Contains("FAILED");
		}
	}
}
