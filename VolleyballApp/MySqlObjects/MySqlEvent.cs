using System;

namespace VolleyballApp {
	public class MySqlEvent {
		public int idEvent { get; set; }
		public string name { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public string location { get; set; }
		/**
		 * State for the event of the user who is logged in
		 **/
		public string state { get; set; }

		public static string idEvent_string = "eventIdEvent";
		public static string name_string = "eventName";
		public static string startDate_string = "eventStartDate";
		public static string endDate_string = "eventEndDate";
		public static string location_string = "eventLocation";
		public static string state_string = "eventState";

		public MySqlEvent(int idEvent, string name, DateTime startDate, DateTime endDate, string location, string state) {
			this.idEvent = idEvent;
			this.name = name;
			this.startDate = startDate;
			this.endDate = endDate;
			this.location = location;
			this.state = state;
		}
	}
}

