using System;
using Android.Content;
using Android.Preferences;
using Android.OS;
using Java.Interop;
using System.Collections.Generic;
using System.Linq;

namespace VolleyballApp {
	public class VBUser {
		public int idUser { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string state { get; set; } //e.g. FILLDATA or FINAL
//		public string role{ get; set; }
		public string password { get; set; }
//		public int number{ get; set; }
//		public string position{ get; set; }
		public string eventState{ get; set; } //e.g. INVITED
		public VBTeamrole teamRole { get; set; }
		public static Context context { get; set; }

		public VBUser() {}

		public VBUser(int idUser, string name, string email, string state, string password, VBTeamrole teamRole)
			: this(idUser, name, email, state, password, teamRole, "") {
		}

		public VBUser(int idUser, string name, string email, string state, string password, VBTeamrole teamRole, string eventState) {
			this.idUser = idUser;
			this.name = name;
			this.email = email;
			this.state = state;
			this.password = password;
			this.teamRole = teamRole;

			switch(eventState) {
			case "G":
				this.eventState = "Zugesagt";
				break;
			case "M":
				this.eventState = "Vielleicht";
				break;
			case "D":
				this.eventState = "Abgesagt";
				break;
			default:
				this.eventState = "Eingeladen";
				break;
			}
		}

		public void StoreUserInPreferences(Context context, VBUser user) {
			VBUser.context = context;
			ISharedPreferences prefs = context.GetSharedPreferences("userinformation", FileCreationMode.Private);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutInt("idUser", user.idUser);
			editor.PutString("name", user.name);
			editor.PutString("email", user.email);
			editor.PutString("state", user.state);
			editor.PutString("role", user.teamRole.role);
			editor.PutString("password", user.password);
			editor.PutInt("number", user.teamRole.number);
			editor.PutString("position", user.teamRole.position);
			editor.PutString("userType", user.teamRole.getUserType().ToString().Substring(0,1));
			editor.Commit();
		}

		public static void DeleteUserFromPreferences() {
			VBUser.context.GetSharedPreferences("userinformation", FileCreationMode.Private).Edit().Clear().Commit();
		}

		public static VBUser GetUserFromPreferences() {
			ISharedPreferences prefs = VBUser.context.GetSharedPreferences("userinformation", FileCreationMode.Private);

			if(prefs.Contains("idUser")) {
				VBTeamrole teamRole = new VBTeamrole(prefs.GetString("userType", ""), prefs.GetString("role", ""), 
					prefs.GetInt("number", 0),prefs.GetString("position", ""));

				return new VBUser(prefs.GetInt("idUser", 0),
					prefs.GetString("name", ""),
					prefs.GetString("email", ""),
					prefs.GetString("state", ""),
					prefs.GetString("password", ""),
					teamRole);
			} else {
				return null;
			}
		}

		public override String ToString() {
			return "Id: " + idUser + ", Name: " + name + ", Email: " + email + ", State: " + state + ", Teamrole: " + teamRole;
		}
	}
}

