
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

			VBUser user = VBUser.GetUserFromPreferences();
			TextView userType = view.FindViewById<TextView>(Resource.Id.profileUserTypeValue);
			EditText name = view.FindViewById<EditText>(Resource.Id.profileNameValue);
//			Spinner position = view.FindViewById<Spinner>(Resource.Id.profilePositionValue);
//			EditText number = view.FindViewById<EditText>(Resource.Id.profileNumberValue);
//			EditText team = view.FindViewById<EditText>(Resource.Id.profileTeamValue);
			EditText password = view.FindViewById<EditText>(Resource.Id.profilePasswordValue);

//			ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.positions, Resource.Layout.SpinnerTextView);
//			adapter.SetDropDownViewResource(Resource.Layout.SpinnerCheckedLayout);
//			position.Adapter = adapter;

			if(user != null) {
				//UserType
				userType.Text = user.getUserType().ToString();

				//Name
				view.FindViewById(Resource.Id.profileNameLine).Visibility = ViewStates.Visible;
				name.Text = user.name;

//				//Position
//				if(DB_Communicator.getInstance().isAtLeast(user.listTeamRole[0].getUserType(), UserType.Coremember)) {
//					view.FindViewById(Resource.Id.profilePositionLine).Visibility = ViewStates.Visible;
//					position.SetSelection(getIdOfPosition(user.listTeamRole[0].position));
//				}
//
//				//Number
//				if(DB_Communicator.getInstance().isAtLeast(user.listTeamRole[0].getUserType(), UserType.Coremember)) {
//					view.FindViewById(Resource.Id.profileNumberLine).Visibility = ViewStates.Visible;
//					number.Text = user.listTeamRole[0].number.ToString();
//				}
					
//				view.FindViewById(Resource.Id.profilePasswordLine).Visibility = ViewStates.Visible;
//				password.Text = user.password;
				
				
				view.FindViewById<Button>(Resource.Id.profileBtnSave).Click += async delegate {
					DB_Communicator db = DB_Communicator.getInstance();
					JsonValue json = await db.UpdateUser(name.Text, "");
					
					//ändernungen im user speichern
					List<VBUser> list = db.createUserFromResponse(json, user.password);
					if(list.Count > 0) {
						VBUser updatedUser = db.createUserFromResponse(json, user.password)[0];
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
	}
}

