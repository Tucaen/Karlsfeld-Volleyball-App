﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Json;

namespace VolleyballApp {
	class DB_SelectEvent : DB_Select{
		
		public DB_SelectEvent(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		/**
		 * Returns a list with all events for the given userId and state.
		 **/
		public async Task<List<MySqlEvent>> SelectEventsForUser(string host, int idUser, string state) {
			string responseText = await dbCommunicator.makeWebRequest("service/user/load_user.php?id=" + idUser + "&loadAttendences=true", "DB_SelectEvent.SelectEventsForUser()");

			return createEventFromResponse(responseText);
		}

		/**
		 * Creates a MySqlEvent for every row in the response string.
		 **/
		private List<MySqlEvent> createEventFromResponse(string responseText) {
			JsonValue json = JsonArray.Parse(responseText);
			List<MySqlEvent> listEvent = new List<MySqlEvent>();

			if(json["data"][0]["User"].ContainsKey("attendences")) {
				foreach(JsonValue e in json["data"][0]["User"]["attendences"]) {
					JsonValue jsonEvent = e["Attendence"]["eventObj"]["Event"];
					Console.WriteLine("createEventFromResponse- creating event - " + e["Attendence"].ToString());
						listEvent.Add(new MySqlEvent(
						dbCommunicator.convertAndInitializeToInt(dbCommunicator.containsKey(jsonEvent, "id", DB_Communicator.JSON_TYPE_INT)),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(jsonEvent, "name", DB_Communicator.JSON_TYPE_STRING)),
						dbCommunicator.convertAndInitializeToDateTime(dbCommunicator.containsKey(jsonEvent, "startDate", DB_Communicator.JSON_TYPE_DATE)),
						dbCommunicator.convertAndInitializeToDateTime(dbCommunicator.containsKey(jsonEvent, "endDate", DB_Communicator.JSON_TYPE_DATE)),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(jsonEvent, "location", DB_Communicator.JSON_TYPE_STRING)),
						dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(e["Attendence"], "state", DB_Communicator.JSON_TYPE_STRING))));
				}
			}
			return listEvent;
		}
	}
}

