using System;

namespace VolleyballApp {
	public class MySqlUser {
		private int idUser{ get; set; }
		private string name{ get; set; }
		private string role{ get; set; }
		private string password{ get; set; }
		private int number{ get; set; }
		private string position{ get; set; }

		public MySqlUser(int idUser, string name, string role, string password, int number, string position) {
			this.idUser = idUser;
			this.name = name;
			this.role = role;
			this.password = password;
			this.number = number;
			this.position = position;
		}
	}
}

