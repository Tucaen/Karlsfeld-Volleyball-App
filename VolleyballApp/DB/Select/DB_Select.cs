using System;
using System.Net.Http;
using System.Net;

namespace VolleyballApp {
	abstract public class DB_Select {
		public DB_Communicator dbCommunicator { get; set; }
		public HttpClient client { get; set; }
		public bool debug { get; set; }

		public DB_Select(DB_Communicator dbCommunicator) {
			this.dbCommunicator = dbCommunicator;
			this.client = dbCommunicator.client;
			this.debug = dbCommunicator.debug;
		}
	}
}

