using System;
using Android.Content;
using Android.Preferences;
using Android.OS;
using Java.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Json;

namespace VolleyballApp {
	public class VBUser {
		public int idUser { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string state { get; set; } //e.g. FILLDATA or FINAL
		public string password { get; set; }
		private string eventState; //e.g. INVITED
		public UserType userType;
		public List<VBTeamrole> listTeamRole { get; set; }
		public static Context context { get; set; }
		private static int count;

		public VBUser() {}

		public VBUser(int idUser, string name, string email, string state, string password, string userType)
			: this(idUser, name, email, state, password, userType, "") {
		}

		public VBUser(int idUser, string name, string email, string state, string password, string userType, string eventState) {
			this.idUser = idUser;
			this.name = name;
			this.email = email;
			this.state = state;
			this.password = password;
			this.setUserType(userType);
			this.listTeamRole = new List<VBTeamrole>();
			this.setEventState(eventState);
		}

		public VBUser(JsonValue json) {
			DB_Communicator db = DB_Communicator.getInstance();
			this.idUser = db.convertAndInitializeToInt(db.containsKey(json, "id", DB_Communicator.JSON_TYPE_INT));
			this.name = db.convertAndInitializeToString(db.containsKey(json, "name", DB_Communicator.JSON_TYPE_STRING));
			this.email = db.convertAndInitializeToString(db.containsKey(json, "email", DB_Communicator.JSON_TYPE_STRING));
			this.state = db.convertAndInitializeToString(db.containsKey(json, "state", DB_Communicator.JSON_TYPE_STRING));
			this.setUserType(db.convertAndInitializeToString(db.containsKey(json, "userType", DB_Communicator.JSON_TYPE_STRING)));
			this.listTeamRole = new List<VBTeamrole>();
			if(json.ContainsKey("teamroles")) {
				if(json["teamroles"] is JsonObject) {
					JsonValue teamrole = json["teamroles"]["TeamRole"];
					this.listTeamRole.Add(new VBTeamrole(teamrole));
				} else {
					foreach(JsonValue teamrole in json["teamroles"]) {
						this.listTeamRole.Add(new VBTeamrole(teamrole["TeamRole"]));
					}
				} 
			}
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

		public void setEventState(string eventState) {
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

		public string getEventState() {
			return this.eventState;
		}

		public VBTeamrole getTeamroleForTeam(int teamId) {
			foreach(VBTeamrole teamrole in listTeamRole) {
				if(teamrole.teamId == teamId)
					return teamrole;
			}
			//return dummy teamrole so sorting won't throw a NullPointerException
			return new VBTeamrole(teamId, UserType.None.ToString(), "", 0, "Keine");
		}

		#region preferences
		public void StoreUserInPreferences(Context context, VBUser user) {
			VBUser.context = context;
			ISharedPreferences prefs = context.GetSharedPreferences("userinformation", FileCreationMode.Private);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutInt("idUser", user.idUser);
			editor.PutString("name", user.name);
			editor.PutString("email", user.email);
			editor.PutString("state", user.state);
			if(user.password != null && !user.password.Equals(""))
				editor.PutString("password", user.password);
			editor.PutString("userType", user.getUserType().ToString().Substring(0,1));
			for(int i = 0; i < listTeamRole.Count; i++) {
				editor.PutInt("teamId"+i, user.listTeamRole[i].teamId);
				editor.PutString("role"+i, user.listTeamRole[i].role);
				editor.PutInt("number"+i, user.listTeamRole[i].number);
				editor.PutString("position"+i, user.listTeamRole[i].position);
				editor.PutString("userType"+i, user.listTeamRole[i].getUserType().ToString().Substring(0,1));
			}
			count = listTeamRole.Count;
			editor.Commit();
		}

		public static void DeleteUserFromPreferences() {
			VBUser.context.GetSharedPreferences("userinformation", FileCreationMode.Private).Edit().Clear().Commit();
		}

		public static VBUser GetUserFromPreferences() {
			ISharedPreferences prefs = VBUser.context.GetSharedPreferences("userinformation", FileCreationMode.Private);
			VBUser vbuser =  new VBUser(prefs.GetInt("idUser", 0),
				prefs.GetString("name", ""),
				prefs.GetString("email", ""),
				prefs.GetString("state", ""),
				prefs.GetString("password", ""),
				prefs.GetString("userType", ""));

			if(prefs.Contains("idUser")) {
				for(int i = 0; i < count; i++) {
					VBTeamrole teamRole = new VBTeamrole(prefs.GetInt("teamId"+i, 0), prefs.GetString("userType"+i, ""), prefs.GetString("role"+i, ""), 
						prefs.GetInt("number"+i, 0),prefs.GetString("position"+i, ""));
					vbuser.listTeamRole.Add(teamRole);
				}


				return vbuser;
			} else {
				return null;
			}
		}
		#endregion

		public void removeTeamrole(int teamId) {
			for(int i = 0; i < listTeamRole.Count; i++) {
				if(listTeamRole[i].teamId == teamId) {
					listTeamRole.RemoveAt(i);
				}
			}
		}

		public override String ToString() {
			return "Id: " + idUser + ", Name: " + name + ", Email: " + email + ", State: " + state + ", Teamrole: " + listTeamRole;
		}
	}
}

