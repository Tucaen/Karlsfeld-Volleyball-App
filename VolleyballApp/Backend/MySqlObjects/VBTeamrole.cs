using System;

namespace VolleyballApp {
	public class VBTeamrole {
		private UserType userType;
		public string role { get; set; }
		public int number { get; set; }
		public string position { get; set; }

		public VBTeamrole(string userType, string role, int number, string position) {
			setUserType(userType);
			this.role = role;
			this.number = number;
			this.position = position;
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
				this.userType = UserType.Fan;
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

