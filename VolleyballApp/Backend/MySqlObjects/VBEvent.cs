using System;
using Android.Content;
using System.Collections.Generic;
using Java.Interop;
using Android.OS;
using System.Linq;
using System.Json;

namespace VolleyballApp {
	public class VBEvent {
		public int idEvent { get; set; }
		public string name { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public string location { get; set; }
		public string state { get; set; }
		public string description { get; set; }
		public int teamId { get; set; }
		public bool isHost { get; set; } //indicates of the currently logged in user is the host of the event

//		public VBEvent(int idEvent, string name, DateTime startDate, DateTime endDate, string location, string state, string description, int teamId) {
//			this.idEvent = idEvent;
//			this.name = name;
//			this.startDate = startDate;
//			this.endDate = endDate;
//			this.location = location;
//			this.description = description;
//			this.teamId = teamId;
//
//			DB_Communicator db = DB_Communicator.getInstance();
//			switch(state) {
//			case "G":
//				this.state = DB_Communicator.State.Accepted;
//				break;
//			case "M":
//				this.state = "Vielleicht";
//				break;
//			case "D":
//				this.state = DB_Communicator.State.Denied;
//				break;
//			default:
//				this.state = DB_Communicator.State.Invited;
//				break;
//			}
//		}

		public VBEvent(JsonValue jsonEventObject) {
			DB_Communicator db = DB_Communicator.getInstance();
			JsonValue jsonEvent = jsonEventObject["Attendence"]["eventObj"]["Event"];

			this.idEvent = db.convertAndInitializeToInt(db.containsKey(jsonEvent, "id", DB_Communicator.JSON_TYPE_INT));
			this.name = db.convertAndInitializeToString(db.containsKey(jsonEvent, "name", DB_Communicator.JSON_TYPE_STRING));
			this.startDate = db.convertAndInitializeToDateTime(db.containsKey(jsonEvent, "startDate", DB_Communicator.JSON_TYPE_DATE));
			this.endDate = db.convertAndInitializeToDateTime(db.containsKey(jsonEvent, "endDate", DB_Communicator.JSON_TYPE_DATE));
			this.location = db.convertAndInitializeToString(db.containsKey(jsonEvent, "location", DB_Communicator.JSON_TYPE_STRING));
			this.state = this.convertStateForUI(db.convertAndInitializeToString(db.containsKey(jsonEventObject["Attendence"], "state", DB_Communicator.JSON_TYPE_STRING)));
			this.description = db.convertAndInitializeToString(db.containsKey(jsonEvent, "description", DB_Communicator.JSON_TYPE_STRING));
			this.teamId = db.convertAndInitializeToInt(db.containsKey(jsonEvent, "teamId", DB_Communicator.JSON_TYPE_INT));
			this.isHost = (db.convertAndInitializeToString(db.containsKey(jsonEventObject["Attendence"], "hostType", DB_Communicator.JSON_TYPE_STRING)).Equals("A"));
		}

		private string convertStateForUI(string jsonState) {
			DB_Communicator db = DB_Communicator.getInstance();
			switch(jsonState) {
			case "G":
				return DB_Communicator.State.Accepted;
			case "M":
				return "Vielleicht";
			case "D":
				return DB_Communicator.State.Denied;
			default:
				return DB_Communicator.State.Invited;
			}
		}

		/** Converts the start and end date of an Event
		 *	If the the dates occur on the same day the output format will be dd.MM.yy HH:mm - HH:mm
		 *	else dd.MM.yy HH:mm - dd.MM.yy HH:mm
		 **/
		public string convertDateForLayout(VBEvent item) {
			if(item.startDate.Day == item.endDate.Day && item.startDate.Month == item.endDate.Month && item.startDate.Year == item.endDate.Year) {
				return item.startDate.ToString("dd.MM.yy") + " (" + item.startDate.ToString("HH:mm") + " - "
														 + item.endDate.ToString("HH:mm") + ")";
//				return item.startDate.ToString("dd.MM.yy HH:mm") + " - " + item.endDate.ToString("HH:mm");
			} else {
				return item.startDate.ToString("dd.MM.yy") + " (" + item.startDate.ToString("HH:mm") + ") - "
					+ item.endDate.ToString("dd.MM.yy") + " (" + item.endDate.ToString("HH:mm") + ")";
//				return item.startDate.ToString("dd.MM.yy HH:mm") + " - " + item.endDate.ToString("dd.MM.yy HH:mm");
			}
		}

//		public static MySqlEvent getEventWithId(int id) {
//			foreach(MySqlEvent e in MySqlEvent.GetListEventsFromPreferences()) {
//				if(e.idEvent == id)
//					return e;
//			}
//			return null;
//		}

		public override string ToString() {
			return "[Event id=" + idEvent + ";name=" + name + ";startDate=" + startDate + ";endDate=" + endDate + ";location=" + location + ";state=" + state + ";description=" + description + "]";
		}
	}
}

