
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
using Java.Lang;

namespace VolleyballApp {
	public class TeamDetailsFragment : Fragment {
		private VBTeam team;
		private VBTeamrole teamrole;
		public static Spinner position;
		public static EditText number;

		public TeamDetailsFragment(VBTeam team, VBTeamrole teamrole) {
			this.team = team;
			this.teamrole = teamrole;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			VBUser user = VBUser.GetUserFromPreferences();

			View view = inflater.Inflate(Resource.Layout.TeamDetailsFragment, container, false);

			view.FindViewById<TextView>(Resource.Id.teamDetailsName).Text = team.name;
			view.FindViewById<TextView>(Resource.Id.teamDetailsSport).Text = team.sport;
			if(!team.location.Equals("") && !team.sport.Equals("")) {
				view.FindViewById<TextView>(Resource.Id.teamDetailsLocation).Text = team.location + " - ";
			} else {
				view.FindViewById<TextView>(Resource.Id.teamDetailsLocation).Text = team.location;
			}

			#region initialize buttons
			Button btnSave = view.FindViewById<Button>(Resource.Id.teamDetailsBtnSave);
			Button btnRequestRank = view.FindViewById<Button>(Resource.Id.teamDetailsBtnRequestRank);
			Button btnJoin = view.FindViewById<Button>(Resource.Id.teamDetailsBtnJoin);
			Button btnFollow = view.FindViewById<Button>(Resource.Id.teamDetailsBtnFollow);
			Button btnLeave = view.FindViewById<Button>(Resource.Id.teamDetailsBtnLeave);

			btnSave.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_SAVE, this));
			btnRequestRank.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_REQUEST_RANK, this));
			btnJoin.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_JOIN, this));
			btnFollow.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_FOLLOW, this));
			btnLeave.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_LEAVE, this));

			btnLeave.Visibility = ViewStates.Gone;
			btnRequestRank.Visibility = ViewStates.Gone;
			#endregion

			view.FindViewById<LinearLayout>(Resource.Id.teamDetailsUserTypeLine).Visibility = ViewStates.Gone;
			view.FindViewById<LinearLayout>(Resource.Id.teamDetailsPositionLine).Visibility = ViewStates.Gone;
			view.FindViewById<LinearLayout>(Resource.Id.teamDetailsNumberLine).Visibility = ViewStates.Gone;

			if(teamrole != null ) {
				btnSave.Visibility = ViewStates.Visible;

				//userTyp
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsUserTypeLine).Visibility = ViewStates.Visible;
				view.FindViewById<TextView>(Resource.Id.teamDetailsUserTypeValue).Text = teamrole.getUserType().ToString();

				if(DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Member)) {
					btnRequestRank.Visibility = ViewStates.Visible;
				} else {
					btnRequestRank.Visibility = ViewStates.Gone;
				}

				if(DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Coremember)) {
					//Position
					view.FindViewById<LinearLayout>(Resource.Id.teamDetailsPositionLine).Visibility = ViewStates.Visible;
					position = view.FindViewById<Spinner>(Resource.Id.teamDetailsPositionValue);

					ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.positions, Resource.Layout.SpinnerTextView);
					adapter.SetDropDownViewResource(Resource.Layout.SpinnerCheckedLayout);
					position.Adapter = adapter;

					position.SetSelection(getIdOfPosition(teamrole.position));

					//Number
					view.FindViewById<LinearLayout>(Resource.Id.teamDetailsNumberLine).Visibility = ViewStates.Visible;
					number = view.FindViewById<EditText>(Resource.Id.teamDetailsNumberValue);

					number.Text = teamrole.number.ToString();
					
				} else {
					btnSave.Visibility = ViewStates.Gone;
				}
				
				if(DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Member)) {
					btnJoin.Visibility = ViewStates.Gone;
					btnFollow.Visibility = ViewStates.Gone;
					btnLeave.Visibility = ViewStates.Visible;
				} else if(DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Fan)) {
					btnJoin.Visibility = ViewStates.Visible;
					btnFollow.Visibility = ViewStates.Gone;
					btnLeave.Visibility = ViewStates.Visible;
				} else {
					btnJoin.Visibility = ViewStates.Visible;
					btnFollow.Visibility = ViewStates.Visible;
					btnLeave.Visibility = ViewStates.Gone;
				}
			} else {
				view.FindViewById<TextView>(Resource.Id.teamDetailsUserTypeValue).Visibility = ViewStates.Gone;
				btnSave.Visibility = ViewStates.Gone;
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

		class TeamDetailsClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
			public const string ON_SAVE = "onSave", ON_JOIN = "onJoin", ON_FOLLOW = "onFollow", ON_LEAVE = "onLeave", ON_REQUEST_RANK="onRequestRank";
			private string source;
			private TeamDetailsFragment t;
			private VBUser user;

			public TeamDetailsClickListener(string source, TeamDetailsFragment t) {
				this.source = source;
				this.t = t;
				this.user = VBUser.GetUserFromPreferences();
			}

			public void OnClick(View view) {
				switch(this.source) {
				case ON_SAVE:
					this.onSave();
					break;
				case ON_JOIN:
					this.onJoin(t.team.id);
					break;
				case ON_FOLLOW:
					this.onFollow(t.team.id);
					break;
				case ON_LEAVE:
					this.onLeave();
					break;
				case ON_REQUEST_RANK:
					this.onRequestRank();
					break;
				}
			}

			private async void onSave() {
				DB_Communicator db = DB_Communicator.getInstance();
				JsonValue json = await db.UpdateUser(user.name, teamrole.role, Convert.ToInt32(number.Text), 
					position.SelectedItem.ToString(), teamrole.teamId);

				//ändernungen im user speichern
				List<VBUser> list = db.createUserFromResponse(json, user.password);
				if(list.Count > 0) {
					VBUser updatedUser = db.createUserFromResponse(json, user.password)[0];
					updatedUser.StoreUserInPreferences(ViewController.getInstance().mainActivity, updatedUser);
				}

				Toast.MakeText(ViewController.getInstance().mainActivity, json["message"].ToString(), ToastLength.Long).Show();
			}
		
			private async void onJoin(int teamId) {
				string response = await DB_Communicator.getInstance().makeWebRequest("service/team/join_team.php?id=" + teamId + "&type=M",
																													"TeamDetailsFragment.onJoin");
					
				this.updateTeamrole(response);
				this.refreshTeamDetailsFragment();
			}

			private async void onFollow(int teamId) {
				string response = await DB_Communicator.getInstance().makeWebRequest("service/team/follow_team.php?id=" + teamId,
					"TeamDetailsFragment.onFollow");

				this.updateTeamrole(response);
				this.refreshTeamDetailsFragment();
			}

			private async void onLeave() {
				string response = await DB_Communicator.getInstance().makeWebRequest("service/team/leave_team.php" +
					"?userId=" + user.idUser +"&teamId=" + t.team.id, 
					"TeamDetailsFragment.onLeave");

				user.removeTeamrole(teamrole.teamId);
				user.StoreUserInPreferences(ViewController.getInstance().mainActivity, user);
				this.refreshTeamDetailsFragment();
			}

			private void onRequestRank() {
				RequestUserTypeDialog d = new RequestUserTypeDialog(user.idUser, team.id);
				d.Show(ViewController.getInstance().mainActivity.FragmentManager, "REQUEST_USERTYPE_DIALOG");

			}

			private void updateTeamrole(string response) {
				JsonValue json = JsonValue.Parse(response);

				if(DB_Communicator.getInstance().wasSuccesful(json)) {
					//createTeamroleFromResponse
					VBTeamrole newTeamrole = new VBTeamrole(json["data"]["TeamRole"]);
					user.listTeamRole.Add(newTeamrole);
					
					//StoreUserInPreferencess
					user.StoreUserInPreferences(ViewController.getInstance().mainActivity, user);

					ViewController vc = ViewController.getInstance();
					vc.toastJson(vc.mainActivity, json,ToastLength.Long, "Successfully updated teamrole");
				}
			}

			private void refreshTeamDetailsFragment() {
				ViewController vc = ViewController.getInstance();
				string tag = ViewController.TEAM_DETAILS_FRAGMENT;

				TeamDetailsFragment frag = vc.mainActivity.FindFragmentByTag(tag) as TeamDetailsFragment;
				frag.teamrole = user.getTeamroleForTeam(team.id);

				ViewController.getInstance().refreshFragment(ViewController.TEAM_DETAILS_FRAGMENT);
			}
		}
	}
}

