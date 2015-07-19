using System;

namespace VolleyballApp {
	public class MySqlEvent {
		private int idEvent { get; set; }
		private string name { get; set; }
		private DateTime startDate { get; set; }
		private DateTime endDate { get; set; }
		private string location { get; set; }

		public MySqlEvent(int idEvent, string name, DateTime startDate, DateTime endDate, string location) {
			this.idEvent = idEvent;
			this.name = name;
			this.startDate = startDate;
			this.endDate = endDate;
			this.location = location;
		}
	}
}

