using System;
using Android.Content;
using Android.Preferences;

namespace VolleyballApp {
	public class MySqlUser {
		public int idUser{ get; set; }
		public string name{ get; set; }
		public string role{ get; set; }
		public string password{ get; set; }
		public int number{ get; set; }
		public string position{ get; set; }

		public MySqlUser(int idUser, string name, string role, string password, int number, string position) {
			this.idUser = idUser;
			this.name = name;
			this.role = role;
			this.password = password;
			this.number = number;
			this.position = position;
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
			editor.Commit();
		}

		public static MySqlUser GetUserFromPreferences(Context context) {
			ISharedPreferences prefs = context.GetSharedPreferences("userinformation", FileCreationMode.Private);
			return new MySqlUser(prefs.GetInt("idUser", 0),
				prefs.GetString("name", ""),
				prefs.GetString("role", ""),
				prefs.GetString("password", ""),
				prefs.GetInt("number", 0),
				prefs.GetString("position", ""));
		}
	}
}

