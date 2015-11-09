
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Json;

namespace VolleyballApp {
	public class ProfileFragment : Fragment {
		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.ProfileFragment, container, false);

			MySqlUser user = MySqlUser.GetUserFromPreferences(this.Activity);
			EditText name = view.FindViewById<EditText>(Resource.Id.profileFirstnameValue);
			EditText position = view.FindViewById<EditText>(Resource.Id.profilePositionValue);
			EditText number = view.FindViewById<EditText>(Resource.Id.profileNumberValue);
			EditText team = view.FindViewById<EditText>(Resource.Id.profileTeamValue);
			EditText password = view.FindViewById<EditText>(Resource.Id.profilePasswordValue);

			name.Text = user.name;
			position.Text = user.position;
			number.Text = user.number.ToString();
			view.FindViewById(Resource.Id.profileTeamLine).Visibility = ViewStates.Gone;
//			team.Text = user.team;
			view.FindViewById(Resource.Id.profilePasswordLine).Visibility = ViewStates.Gone;
//			password.Text = user.password;


			view.FindViewById<Button>(Resource.Id.profileBtnSave).Click += async delegate {
				DB_Communicator db = DB_Communicator.getInstance();
				JsonValue json = await db.UpdateUser(name.Text, user.role, Convert.ToInt32(number.Text), position.Text);

				//ändernungen im user speichern
				//TODO Update-Skript muss noch angepasst werden. Liefert im Moment nur den Namen zurück
				MySqlUser updatedUser =  db.createUserFromResponse(json, user.password)[0]; //TODO user.password durch password.Text ersetzen
				updatedUser.StoreUserInPreferences(this.Activity, updatedUser);

				Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long).Show();
			};

			return view;
		}
	}
}

