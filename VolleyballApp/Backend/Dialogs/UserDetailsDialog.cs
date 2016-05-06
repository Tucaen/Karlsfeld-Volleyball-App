
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Json;

namespace VolleyballApp {

	class UserDetailsDialog : DialogFragment {
		public VBUser clickedUser { get; set; }
		public int teamId { get; set; }
		public VBUser user { get; set; }
		public VBEvent _event {get; set;}
		public UserDetailsType type { get; set; }

		public UserDetailsDialog (VBUser clickedUser, int teamId, UserDetailsType type) 
			: this(clickedUser, teamId, null, type) {}

		public UserDetailsDialog (VBUser clickedUser, int teamId, VBEvent _event, UserDetailsType type) {
			this.clickedUser = clickedUser;
			this.teamId = teamId;
			this.user = VBUser.GetUserFromPreferences();
			this._event = _event;
			this.type = type;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.UserDetailsDialog, container, false);

			this.Dialog.SetTitle("User Details");

			TextView name = view.FindViewById<TextView>(Resource.Id.UserDetailsDialog_Name);
			TextView role = view.FindViewById<TextView>(Resource.Id.UserDetailsDialog_Role);
			TextView userType = view.FindViewById<TextView>(Resource.Id.UserDetailsDialog_UserTypeValue);
			TextView number = view.FindViewById<TextView>(Resource.Id.UserDetailsDialog_NumberValue);
			TextView position = view.FindViewById<TextView>(Resource.Id.UserDetailsDialog_PositionValue);

			name.Text = clickedUser.getNameForUI();
			if(teamId != 0) {
				VBTeamrole teamrole = clickedUser.getTeamroleForTeam(teamId);

				if(DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Member)) {
					//wird nur angzeigt, wenn ein Wert vorhanden ist, da kein Label
					if(!teamrole.role.Equals("")) {
						role.Visibility = ViewStates.Visible;
						role.Text = teamrole.role;
					}
				}

				if(DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Coremember)) {
					view.FindViewById<LinearLayout>(Resource.Id.UserDetailsDialog_NumberLine).Visibility = ViewStates.Visible;
					number.Text = (teamrole.number != 0) ? teamrole.number.ToString() : "N/A";

					view.FindViewById<LinearLayout>(Resource.Id.UserDetailsDialog_PositionLine).Visibility = ViewStates.Visible;
					position.Text = (!teamrole.position.Equals("")) ? teamrole.position : "N/A";
				}

				if(DB_Communicator.getInstance().isAtLeast(user.getTeamroleForTeam(teamId).getUserType(), UserType.Admin)) {
					view.FindViewById<LinearLayout>(Resource.Id.UserDetailsDialog_UserTypeLine).Visibility = ViewStates.Visible;
					
					userType.Text = teamrole.getUserType().ToString();
				}
			}

			Button btnRemove = view.FindViewById<Button>(Resource.Id.UserDetailsDialog_BtnRemove);

			if(this.type.Equals(UserDetailsType.Event)) {
				//TODO nur einblenden wenn user host des events ist
				if(_event.isHost) {
					view.FindViewById<LinearLayout>(Resource.Id.UserDetailsDialog_BtnRemoveLine).Visibility = ViewStates.Visible;
				}
			} else if(this.type.Equals(UserDetailsType.Team)) {
				if(DB_Communicator.getInstance().isAtLeast(user.getTeamroleForTeam(teamId).getUserType(), UserType.Admin)) {
					view.FindViewById<LinearLayout>(Resource.Id.UserDetailsDialog_BtnRemoveLine).Visibility = ViewStates.Visible;
				}
			}
			btnRemove.SetOnClickListener(new UserDetailsClickListener(UserDetailsClickListener.ON_REMOVE, view, this));

			return view;
		}
	}

	class  UserDetailsClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
		public const string ON_REMOVE="onRemove";
		private string source;
		private UserDetailsDialog d;
		private View view;

		public UserDetailsClickListener(string source, View view, UserDetailsDialog d) {
			this.source = source;
			this.view = view;
			this.d = d;
		}

		public void OnClick(View view) {
			switch(this.source) {
			case ON_REMOVE:
				this.onRemove();
				break;
			}
		}

		private async void onRemove() {
			if(d.type.Equals(UserDetailsType.Event)) {
				//remove user from event
				string response = await DB_Communicator.getInstance().makeWebRequest("service/event/uninvite_user.php" +
					"?userId=" + d.clickedUser.idUser +"&eventId=" + d._event.idEvent, "UserDetailsDialog.onRemove_event");

				ViewController.getInstance().toastJson(null, JsonValue.Parse(response), ToastLength.Long, "User uninvited");

				d.Dismiss();

				//refresh list of inveted users
				List<VBUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(d._event.idEvent, "");
				EventDetailsFragment frag = ViewController.getInstance().mainActivity.FindFragmentByTag(ViewController.EVENT_DETAILS_FRAGMENT) as EventDetailsFragment;
				frag.listUser = listUser;

				//refresh view
				ViewController.getInstance().refreshFragment(ViewController.EVENT_DETAILS_FRAGMENT);

			} else if(d.type.Equals(UserDetailsType.Team)) {
				//remove user from team
				string response = await DB_Communicator.getInstance().makeWebRequest("service/team/leave_team.php" +
					"?userId=" + d.clickedUser.idUser +"&teamId=" + d.teamId, "UserDetailsDialog.onRemove_team");

				d.clickedUser.removeTeamrole(d.teamId);
				d.Dismiss();

				//refresh the view
				TeamDetailsFragment t = TeamDetailsFragment.findTeamDetailsFragment();
				await t.updateListMember();
				t.refreshTeamDetailsFragment();
			}
		}
	}

	public enum UserDetailsType {
		Event,
		Team
	}
}
