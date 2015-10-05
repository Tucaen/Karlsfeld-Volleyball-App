using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VolleyballApp {
	class DB_SelectEvent : DB_Select{
		
		public DB_SelectEvent(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		/**
		 * Concatenate a Uri with the given parameters.
		 * If uri invokation was succesfull a list with all events for the given userId and state will be created,
		 * which will be stored in the variable listEvent.
		 **/
		public async Task<List<MySqlEvent>> SelectEventsForUser(string host, int idUser, string state) {
			HttpResponseMessage response = new HttpResponseMessage();
//			Uri uri = new Uri(host + "php/requestEventsForUser.php?idUser=" + idUser + "&state=" + state);

			List<MySqlEvent> listEvent = new List<MySqlEvent>();
//			string responseText;
//			try {
//				response = await base.client.GetAsync(uri).ConfigureAwait(continueOnCapturedContext:false);
//				response.EnsureSuccessStatusCode();
//				responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext:false);
//
//				listEvent = createEventFromResponse(responseText);
//			} catch(Exception e) {
//				Console.WriteLine("DB_SelectEvent.SelectEventsForUser() - Error while selecting data from MySQL: " + e.Message);
//			}
			return listEvent;
		}

		/**
		 * Creates a MySqlEvent for every row in the response string.
		 **/
		private List<MySqlEvent> createEventFromResponse(string response) {
			if(base.dbCommunicator.wasSuccesful(response)) {
				string[] eventInfo = response.Split('|');
				List<MySqlEvent> listEvent = new List<MySqlEvent>();

				int i = 0;
				do {
					if(debug) {
						Console.WriteLine("Creating Event: " + eventInfo[i] + " " + eventInfo[i + 1] + " " + eventInfo[i + 2] + " " + eventInfo[i + 3] + " " + eventInfo[i + 4] + " " + eventInfo[i+5]);
					}
					listEvent.Add(new MySqlEvent(Convert.ToInt32(eventInfo[i]), eventInfo[i + 1], Convert.ToDateTime(eventInfo[i + 2]), Convert.ToDateTime(eventInfo[i + 3]), eventInfo[i + 4], eventInfo[i + 5]));
					i += 6;
				} while(!eventInfo[i].Equals("<endoffile>")) ;

				return listEvent;
			} else {
				Console.WriteLine("Invalid response for creating event!");
				return null;
			}
		}
	}
}

