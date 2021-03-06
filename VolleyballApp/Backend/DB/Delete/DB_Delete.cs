﻿using System;
using System.Json;
using System.Threading.Tasks;

namespace VolleyballApp {
	public class DB_Delete {
		public DB_Communicator dbCommunicator { get; set; }
		public bool debug { get; set; }

		public DB_Delete(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.debug = dbCommunicator.debug;
		}

		public async Task<JsonValue> deleteEvent(int id) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/delete_event.php" + "?idEvent=" + id
				, "DB_InsertEvent.deleteEvent()");

			return JsonValue.Parse(responseText);
		}

		public async Task<JsonValue> createEvent(int teamId, string name, string location, string start, string end) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/create_event.php" + "?teamId=" + teamId 
				+ "&name=" + name + "&start=" + start + "&end=" + end + "&location="+ location, "DB_InsertEvent.createEvent()");

			return JsonValue.Parse(responseText);
		}
	}
}

