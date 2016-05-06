using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Json;
using System.Linq;

namespace VolleyballApp {
	class DB_SelectEvent : DB_Select{
		
		public DB_SelectEvent(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		/**
		 * Returns a JsonValue with all events for the given userId and state.
		 **/
		public async Task<JsonValue> SelectUpcomingEventsForUser(int idUser, string state) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/load_user.php?id=" + idUser + 
				"&loadAttendences=true", "DB_SelectEvent.SelectEventsForUser()");

			return JsonValue.Parse(responseText);
		}

		public async Task<JsonValue> SelectPastEventsForUser(int idUser, string state) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/load_user.php?id=" + idUser + 
				"&loadAttendences=true&loadPastEvents=true", "DB_SelectEvent.SelectEventsForUser()");

			return JsonValue.Parse(responseText);
		}

		/**
		 * Creates a MySqlEvent for every row in the response string.
		 **/
		public List<VBEvent> createEventFromResponse(JsonValue json) {
			List<VBEvent> listEvent = new List<VBEvent>();

			if(dbCommunicator.wasSuccesful(json)) {
				if(json["data"].Count > 0) {
					if(json["data"][0]["User"].ContainsKey("attendences")) {
						foreach(JsonValue e in json["data"][0]["User"]["attendences"]) {
							if(e["Attendence"].ContainsKey("eventObj")) {
								JsonValue jsonEvent = e["Attendence"]["eventObj"]["Event"];
								Console.WriteLine("createEventFromResponse- creating event - " + e["Attendence"].ToString());
								listEvent.Add(new VBEvent(e));
							}
						}
					}
				}
			}
			List<VBEvent> sortedList = listEvent.OrderBy(o => o.startDate).ToList();
			return sortedList;
		}
	}
}

