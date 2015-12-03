using System;
using System.Json;
using System.Threading.Tasks;

namespace VolleyballApp {
	public class DB_InsertEvent {
		public DB_Communicator dbCommunicator { get; set; }
		public bool debug { get; set; }

		public DB_InsertEvent(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.debug = dbCommunicator.debug;
		}

		public async Task<JsonValue> createEvent(string name, string location, string start, string end, string description) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/create_event.php" + "?name=" + name + 
				"&start=" + start + "&end=" + end + "&location="+ location + "&desc="+ description, "DB_InsertEvent.createEvent()");

			return JsonValue.Parse(responseText);
		}

		public async Task<JsonValue> createEvent(int teamId, string name, string location, string start, string end) {
			string responseText = await dbCommunicator.makeWebRequest("service/event/create_event.php" + "?teamId=" + teamId 
				+ "&name=" + name + "&start=" + start + "&end=" + end + "&location="+ location, "DB_InsertEvent.createEvent()");

			return JsonValue.Parse(responseText);
		}
	}
}

