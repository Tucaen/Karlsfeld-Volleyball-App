using System;
using Android.Content;
using System.Collections.Generic;
using Java.Interop;
using Android.OS;
using System.Linq;

namespace VolleyballApp {
	public class VBEvent {
		public int idEvent { get; set; }
		public string name { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public string location { get; set; }
		public string state { get; set; }
		public string description { get; set; }
//		private static Intent intent;

		/** Constructor for Parcelable **/
		public VBEvent(){}

		public VBEvent(int idEvent, string name, DateTime startDate, DateTime endDate, string location, string state, string description) {
			this.idEvent = idEvent;
			this.name = name;
			this.startDate = startDate;
			this.endDate = endDate;
			this.location = location;
			this.description = description;

			DB_Communicator db = DB_Communicator.getInstance();
			switch(state) {
			case "G":
				this.state = DB_Communicator.State.Accepted;
				break;
			case "M":
				this.state = "Vielleicht";
				break;
			case "D":
				this.state = DB_Communicator.State.Denied;
				break;
			default:
				this.state = DB_Communicator.State.Invited;
				break;
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

