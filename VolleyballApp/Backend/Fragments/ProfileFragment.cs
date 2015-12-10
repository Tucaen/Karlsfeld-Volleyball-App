
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;

using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace VolleyballApp {
	public class ProfileFragment : Fragment {
		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.ProfileFragment, container, false);

			MySqlUser user = MySqlUser.GetUserFromPreferences();
			TextView userType = view.FindViewById<TextView>(Resource.Id.profileUserTypeValue);
			EditText name = view.FindViewById<EditText>(Resource.Id.profileNameValue);
			Spinner position = view.FindViewById<Spinner>(Resource.Id.profilePositionValue);
			EditText number = view.FindViewById<EditText>(Resource.Id.profileNumberValue);
			EditText team = view.FindViewById<EditText>(Resource.Id.profileTeamValue);
			EditText password = view.FindViewById<EditText>(Resource.Id.profilePasswordValue);

			ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.positions, Resource.Layout.SpinnerTextView);
			adapter.SetDropDownViewResource(Resource.Layout.SpinnerCheckedLayout);
			position.Adapter = adapter;

			if(user != null) {
				//UserType
				userType.Text = user.teamRole.getUserType().ToString();

				//Name
				view.FindViewById(Resource.Id.profileNameLine).Visibility = ViewStates.Visible;
				name.Text = user.name;

				//Position
				if(DB_Communicator.getInstance().isAtLeast(user, UserType.Coremember)) {
					view.FindViewById(Resource.Id.profilePositionLine).Visibility = ViewStates.Visible;
					position.SetSelection(getIdOfPosition(user.teamRole.position));
				}

				//Number
				if(DB_Communicator.getInstance().isAtLeast(user, UserType.Coremember)) {
					view.FindViewById(Resource.Id.profileNumberLine).Visibility = ViewStates.Visible;
					number.Text = user.teamRole.number.ToString();
				}

//				view.FindViewById(Resource.Id.profileTeamLine).Visibility = ViewStates.Visible;
//				team.Text = user.team;
//				view.FindViewById(Resource.Id.profilePasswordLine).Visibility = ViewStates.Visible;
//				password.Text = user.password;
				
				
				view.FindViewById<Button>(Resource.Id.profileBtnSave).Click += async delegate {
					DB_Communicator db = DB_Communicator.getInstance();
					JsonValue json = await db.UpdateUser(name.Text, user.teamRole.role, Convert.ToInt32(number.Text), position.SelectedItem.ToString(), 1);
					
					//ändernungen im user speichern
					//TODO Update-Skript muss noch angepasst werden. Liefert im Moment nur den Namen zurück
					List<MySqlUser> list = db.createUserFromResponse(json, user.password);
					if(list.Count > 0) {
						MySqlUser updatedUser = db.createUserFromResponse(json, user.password)[0]; //TODO user.password durch password.Text ersetzen
						updatedUser.StoreUserInPreferences(this.Activity, updatedUser);
					}
					
					Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long).Show();
				};
			} else {
				view.FindViewById(Resource.Id.profileErrorLine).Visibility = ViewStates.Visible;
				view.FindViewById<EditText>(Resource.Id.profileErrorValue).Text = "There was an error loading your profile information! " +
																					" Server may be down!";
			}

			return view;
		}

		private int getIdOfPosition(string position) {
			switch(position) {
			case "Außenangreifer":
				return 1;
			case "Diagonalangreifer":
				return 2;
			case "Libero":
				return 3;
			case "Mittelblocker":
				return 4;
			case "Steller":
				return 5;
			default:
				return 0;
			}
		}
	}
}

