using System;
using System.Json;

namespace VolleyballApp {
	public class VBTeamrole {
		public int teamId { get; set; }
		private UserType userType;
		public string role { get; set; }
		public int number { get; set; }
		public string position { get; set; }

		public VBTeamrole(int teamId, string userType, string role, int number, string position) {
			this.teamId = teamId;
			setUserType(userType);
			this.role = role;
			this.number = number;
			this.position = position;
		}

		public VBTeamrole(JsonValue json) {
			DB_Communicator db = DB_Communicator.getInstance();
			this.teamId = db.convertAndInitializeToInt(db.containsKey(json, "teamId", DB_Communicator.JSON_TYPE_INT));
			setUserType(db.convertAndInitializeToString(db.containsKey(json, "userType", DB_Communicator.JSON_TYPE_STRING)));
			this.role = db.convertAndInitializeToString(db.containsKey(json, "role", DB_Communicator.JSON_TYPE_STRING));
			this.number = db.convertAndInitializeToInt(db.containsKey(json, "number", DB_Communicator.JSON_TYPE_INT));
			this.position = db.convertAndInitializeToString(db.containsKey(json, "position", DB_Communicator.JSON_TYPE_STRING));
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

		public void setUserType(UserType userType) {
			this.userType = userType;
		}

		public override string ToString() {
			return "UserType: " + userType + ", Role: " + role + ", Number: " + number + ", Position: " + position;
		}
	}
}

