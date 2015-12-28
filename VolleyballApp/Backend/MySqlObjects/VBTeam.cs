using System;
using System.Json;

namespace VolleyballApp {
	public class VBTeam : Java.Lang.Object {
		public int id { get; set; }
		public string name { get; set; }
		public string sport { get; set; }
		public string location { get; set; }
		public string description { get; set; }

		public VBTeam() {
		}

		public VBTeam(int id) : this(id, "", "", "", "") {
		}

		public VBTeam(int id, string name, string sport, string location, string description) {
			this.id = id;
			this.name = name;
			this.sport = sport;
			this.location = location;
			this.description = description;
		}

		public VBTeam(JsonValue json) {
			DB_Communicator db = DB_Communicator.getInstance();
			this.id = db.convertAndInitializeToInt(db.containsKey(json, "id", DB_Communicator.JSON_TYPE_INT));
			this.name = db.convertAndInitializeToString(db.containsKey(json, "name", DB_Communicator.JSON_TYPE_STRING));
			this.sport = db.convertAndInitializeToString(db.containsKey(json, "sport", DB_Communicator.JSON_TYPE_STRING));
			this.location = db.convertAndInitializeToString(db.containsKey(json, "location", DB_Communicator.JSON_TYPE_STRING));
			this.description = db.convertAndInitializeToString(db.containsKey(json, "description", DB_Communicator.JSON_TYPE_STRING));
		}

		public override string ToString() {
			return "id: " + id + " name: " + name + " sport: " + sport + " location: " + location + " description: " + description;
		}
	}
}

