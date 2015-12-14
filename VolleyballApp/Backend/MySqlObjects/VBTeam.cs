using System;

namespace VolleyballApp {
	public class VBTeam {
		public int id { get; set; }
		public string name { get; set; }
		public string sport { get; set; }
		public string location { get; set; }
		public string description { get; set; }

		public VBTeam(int id) : this(id, "", "", "", "") {
		}

		public VBTeam(int id, string name, string sport, string location, string description) {
			this.id = id;
			this.name = name;
			this.sport = sport;
			this.location = location;
			this.description = description;
		}
	}
}

