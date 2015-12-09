using System;
using Android.Content;
using Android.Preferences;
using Android.OS;
using Java.Interop;
using System.Collections.Generic;
using System.Linq;

namespace VolleyballApp {
	public class MySqlUser : MySqlObject {
		public int idUser { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string state { get; set; } //e.g. FILLDATA or FINAL
//		public string role{ get; set; }
		public string password { get; set; }
//		public int number{ get; set; }
//		public string position{ get; set; }
		public string eventState{ get; set; } //e.g. INVITED
		public MySqlTeamrole teamRole { get; set; }
		public static Context context { get; set; }

		public MySqlUser() {}

		public MySqlUser(int idUser, string name, string email, string state, string password, MySqlTeamrole teamRole)
			: this(idUser, name, email, state, password, teamRole, "") {
		}

		public MySqlUser(int idUser, string name, string email, string state, string password, MySqlTeamrole teamRole, string eventState) {
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

		public void StoreUserInPreferences(Context context, MySqlUser user) {
			MySqlUser.context = context;
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
			editor.Commit();
		}

		public static void DeleteUserFromPreferences() {
			MySqlUser.context.GetSharedPreferences("userinformation", FileCreationMode.Private).Edit().Clear().Commit();
		}

		public static MySqlUser GetUserFromPreferences() {
			ISharedPreferences prefs = MySqlUser.context.GetSharedPreferences("userinformation", FileCreationMode.Private);

			if(prefs.Contains("idUser")) {
				MySqlTeamrole teamRole = new MySqlTeamrole("", prefs.GetString("role", ""), 
					prefs.GetInt("number", 0),prefs.GetString("position", ""));

				return new MySqlUser(prefs.GetInt("idUser", 0),
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

		#region ParcelableImplementation
		public MySqlUser(Parcel p) {
			this.idUser = p.ReadInt();
			this.name = p.ReadString();
			this.email = p.ReadString();
			this.state = p.ReadString();
			this.teamRole.role = p.ReadString();
			this.password = p.ReadString();
			this.teamRole.number = p.ReadInt();
			this.teamRole.position = p.ReadString();
		}

		public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags) {
			dest.WriteInt(idUser);
			dest.WriteString(name);
			dest.WriteString(email);
			dest.WriteString(state);
			dest.WriteString(teamRole.role);
			dest.WriteString(password);
			dest.WriteInt(teamRole.number);
			dest.WriteString(teamRole.position);
		}

		public static readonly MyParcelableCreator<MySqlUser> _creator 
		= new MyParcelableCreator<MySqlUser>((parcel) => new MySqlUser(parcel));

		[ExportField ("CREATOR")]
		public static MyParcelableCreator<MySqlUser> InitializeCreator() {
			return _creator;
		}
		#endregion
	}
}

