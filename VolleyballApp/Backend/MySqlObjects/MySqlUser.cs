using System;
using Android.Content;
using Android.Preferences;
using Android.OS;
using Java.Interop;
using System.Collections.Generic;
using System.Linq;

namespace VolleyballApp {
	public class MySqlUser : MySqlObject {
		public int idUser{ get; set; }
		public string name{ get; set; }
		public string email{ get; set; }
		public string state{ get; set; }
		public string role{ get; set; }
		public string password{ get; set; }
		public int number{ get; set; }
		public string position{ get; set; }
		public string eventState{ get; set; }

		private static Intent intent;
		private static readonly string LIST_USER = "listUser";

		public MySqlUser() {}

		public MySqlUser(int idUser, string name, string email, string state, string role, string password, int number, string position)
			: this(idUser, name, email, state, role, password, number, position, "") {
		}

		public MySqlUser(int idUser, string name, string email, string state, string role, string password, int number, string position, string eventState) {
			this.idUser = idUser;
			this.name = name;
			this.email = email;
			this.state = state;
			this.role = role;
			this.password = password;
			this.number = number;
			this.position = position;

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
			ISharedPreferences prefs = context.GetSharedPreferences("userinformation", FileCreationMode.Private);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutInt("idUser", user.idUser);
			editor.PutString("name", user.name);
			editor.PutString("email", user.email);
			editor.PutString("state", user.state);
			editor.PutString("role", user.role);
			editor.PutString("password", user.password);
			editor.PutInt("number", user.number);
			editor.PutString("position", user.position);
			editor.Commit();
		}

		public static void DeleteUserFromPreferences(Context context) {
			context.GetSharedPreferences("userinformation", FileCreationMode.Private).Edit().Clear().Commit();
		}

		public static MySqlUser GetUserFromPreferences(Context context) {
			ISharedPreferences prefs = context.GetSharedPreferences("userinformation", FileCreationMode.Private);

			if(prefs.Contains("idUser")) {
				return new MySqlUser(prefs.GetInt("idUser", 0),
					prefs.GetString("name", ""),
					prefs.GetString("email", ""),
					prefs.GetString("state", ""),
					prefs.GetString("role", ""),
					prefs.GetString("password", ""),
					prefs.GetInt("number", 0),
					prefs.GetString("position", ""));
			} else {
				return null;
			}
		}

		public static void StoreUserListInPreferences(Intent intent, List<MySqlUser> listUser) {
			MySqlUser.intent = intent;
			MySqlUser[] array = new MySqlUser[listUser.Count];
			for(int i = 0; i < array.Length; i++) {
				array[i] = listUser[i];
			}
			intent.PutParcelableArrayListExtra(LIST_USER, array);
			MySqlUser[] tempArray = intent.GetParcelableArrayListExtra(LIST_USER).Cast<MySqlUser>().ToArray();
		}

		public static List<MySqlUser> GetListUserFromPreferences() {
			List<MySqlUser> listUser = new List<MySqlUser>();
			MySqlUser[] array = MySqlUser.intent.GetParcelableArrayListExtra(LIST_USER).Cast<MySqlUser>().ToArray();
			for(int i = 0; i < array.Length; i++) {
				listUser.Add(array[i]);
			}
			return listUser;
		}

		public override String ToString() {
			return "Id: " + idUser + ", Name: " + name + ", Email: " + email + ", State: " + state + ", Role: " + role + ", Number: " + number + ", Position: " + position;
		}

		#region ParcelableImplementation
		public MySqlUser(Parcel p) {
			this.idUser = p.ReadInt();
			this.name = p.ReadString();
			this.email = p.ReadString();
			this.state = p.ReadString();
			this.role = p.ReadString();
			this.password = p.ReadString();
			this.number = p.ReadInt();
			this.position = p.ReadString();
		}

		public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags) {
			dest.WriteInt(idUser);
			dest.WriteString(name);
			dest.WriteString(email);
			dest.WriteString(state);
			dest.WriteString(role);
			dest.WriteString(password);
			dest.WriteInt(number);
			dest.WriteString(position);
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

