using System;

namespace VolleyballApp {
	public class MySqlEvent {
		public int idEvent { get; set; }
		public string name { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public string location { get; set; }

		public MySqlEvent(int idEvent, string name, DateTime startDate, DateTime endDate, string location) {
			this.idEvent = idEvent;
			this.name = name;
			this.startDate = startDate;
			this.endDate = endDate;
			this.location = location;
		}
	}
}

