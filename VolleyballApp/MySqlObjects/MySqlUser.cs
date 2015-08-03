using System;
using Android.Content;
using Android.Preferences;
using System.Collections.Generic;

namespace VolleyballApp {
	public class MySqlUser : IComparable<MySqlUser> {
		public int idUser{ get; set; }
		public string name{ get; set; }
		public string role{ get; set; }
		public string password{ get; set; }
		public int number{ get; set; }
		public string position{ get; set; }
		public string state { get; set; }

		public MySqlUser(int idUser, string name, string role, string password, int number, string position, string state) {
			this.idUser = idUser;
			this.name = name;
			this.role = role;
			this.password = password;
			this.number = number;
			this.position = position;
			this.state = state;
		}

		public void StoreUserInPreferences(Context context, MySqlUser user) {
			
			ISharedPreferences prefs = context.GetSharedPreferences("userinformation", FileCreationMode.Private);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutInt("idUser", user.idUser);
			editor.PutString("name", user.name);
			editor.PutString("role", user.role);
			editor.PutString("password", user.password);
			editor.PutInt("number", user.number);
			editor.PutString("position", user.position);
			editor.PutString("state", user.state);
			editor.Commit();
		}

		public static MySqlUser GetUserFromPreferences(Context context) {
			ISharedPreferences prefs = context.GetSharedPreferences("userinformation", FileCreationMode.Private);
			return new MySqlUser(prefs.GetInt("idUser", 0),
				prefs.GetString("name", ""),
				prefs.GetString("role", ""),
				prefs.GetString("password", ""),
				prefs.GetInt("number", 0),
				prefs.GetString("position", ""),
				prefs.GetString("state", ""));
		}

		public static MySqlUser GetUserFromList(int idUser, List<MySqlUser> listUser) {
			for(int i = 0; i < listUser.Count; i++) {
				if(listUser[i].idUser == idUser)
					return listUser[i];
			}
			return null;
		}

		/**
		 * Sorts the user for state (accepted, denied, invited).
		 * If they have the same state the are sorted alphabetically after name.
		 **/
		public int CompareTo(MySqlUser other) {
			if(state.Equals(other.state))
				return name.CompareTo(other.name);

			if(state.Equals(DB_Communicator.State.Accepted))
				return -1;
				
			if(state.Equals(DB_Communicator.State.Denied) && other.state.Equals(DB_Communicator.State.Invited))
				return -1;

			return 1;
		}
	}
}

