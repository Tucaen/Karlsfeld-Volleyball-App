using System;
using System.Json;

namespace VolleyballApp {
	public class VBRequest {
		public int userId {get;set;}
		public int teamId {get;set;}
		private UserType userType;
		public string state {get;set;}
		public string userName {get;set;}

		public VBRequest(JsonValue json) {
			DB_Communicator db = DB_Communicator.getInstance();
			this.userId = db.convertAndInitializeToInt(db.containsKey(json, "userId", DB_Communicator.JSON_TYPE_INT));
			this.teamId = db.convertAndInitializeToInt(db.containsKey(json, "teamId", DB_Communicator.JSON_TYPE_INT));
			this.setUserType(db.convertAndInitializeToString(db.containsKey(json, "userType", DB_Communicator.JSON_TYPE_STRING)));
			this.state = db.convertAndInitializeToString(db.containsKey(json, "state", DB_Communicator.JSON_TYPE_STRING));
			this.userName = db.convertAndInitializeToString(db.containsKey(json["userObj"]["User"], "name", DB_Communicator.JSON_TYPE_STRING));
		}

		public UserType getUserType() {
			return this.userType;
		}

		public void setUserType(string userType) {
			switch(userType) {
			case "A":
				this.userType = UserType.Admin;
				break;
			case "O":
				this.userType = UserType.Operator;
				break;
			case "C":
				this.userType = UserType.Coremember;
				break;
			case "M":
				this.userType = UserType.Member;
				break;
			case "F":
				this.userType = UserType.Fan;
				break;
			default	:
				this.userType = UserType.None;
				break;
			}
		}
	}
}

