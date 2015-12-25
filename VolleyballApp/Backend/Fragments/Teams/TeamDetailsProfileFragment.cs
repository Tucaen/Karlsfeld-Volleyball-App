
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
	public class TeamDetailsProfileFragment : Fragment {
		public VBTeam team { get; set; }
		private VBTeamrole teamrole;
		public Spinner position;
		public EditText number;
		public List<VBRequest> listRequests { get; set; }

		public TeamDetailsProfileFragment(VBTeam team, VBTeamrole teamrole, List<VBRequest> listRequests) {
			this.team = team;
			this.teamrole = teamrole;
			this.listRequests = listRequests;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			VBUser user = VBUser.GetUserFromPreferences();

			View view = inflater.Inflate(Resource.Layout.TeamDetailsProfileFragment, container, false);

			#region buttons
			Button btnRequestRank = view.FindViewById<Button>(Resource.Id.teamDetailsBtnRequestRank);
			Button btnSave = view.FindViewById<Button>(Resource.Id.teamDetailsBtnSave);

			btnRequestRank.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_REQUEST_RANK, this));
			btnSave.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_SAVE, this));
			#endregion

			#region requests
			if(this.listRequests.Count > 0 && DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Admin)) {
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsRequestsLayout).Visibility = ViewStates.Visible;
				this.initialzeListRequests(view.FindViewById<LinearLayout>(Resource.Id.teamDetailsRequestList), this.listRequests, inflater);
			} else {
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsRequestsLayout).Visibility = ViewStates.Gone;
			}
			#endregion

			#region teamrole
			view.FindViewById<LinearLayout>(Resource.Id.teamDetailsUserTypeLine).Visibility = ViewStates.Gone;
			view.FindViewById<LinearLayout>(Resource.Id.teamDetailsPositionLine).Visibility = ViewStates.Gone;
			view.FindViewById<LinearLayout>(Resource.Id.teamDetailsNumberLine).Visibility = ViewStates.Gone;

			if(teamrole != null ) {
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsProfileLayout).Visibility = ViewStates.Visible;
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
			} else {
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsProfileLayout).Visibility = ViewStates.Gone;
				view.FindViewById<TextView>(Resource.Id.teamDetailsUserTypeValue).Visibility = ViewStates.Gone;
				btnSave.Visibility = ViewStates.Gone;
			}
			#endregion
			return view;
		}

		private void initialzeListRequests(LinearLayout listView, List<VBRequest> list, LayoutInflater inflater) {
			foreach(VBRequest request in list) {
				View row = inflater.Inflate(Resource.Layout.RequestListView, null);
				row.FindViewById<TextView>(Resource.Id.requestListViewName).Text = request.userName;
				row.FindViewById<TextView>(Resource.Id.requestListViewUserType).Text = request.getUserType().ToString();

				ImageView btnAccept = row.FindViewById<ImageView>(Resource.Id.requestListViewBtnAccept);
				ImageView btnDenie = row.FindViewById<ImageView>(Resource.Id.requestListViewBtnDenie);
				btnAccept.SetOnClickListener(new RequestClickListener(RequestClickListener.ON_ACCEPT_REQUEST, request, this));
				btnDenie.SetOnClickListener(new RequestClickListener(RequestClickListener.ON_DENIE_REQUEST, request, this));

				listView.AddView(row);
			}
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
			private TeamDetailsProfileFragment t;
			private VBUser user;

			public TeamDetailsClickListener(string source, TeamDetailsProfileFragment t) {
				this.source = source;
				this.t = t;
				this.user = VBUser.GetUserFromPreferences();
			}

			public void OnClick(View view) {
				switch(this.source) {
				case ON_SAVE:
					this.onSave();
					break;
				case ON_REQUEST_RANK:
					this.onRequestRank();
					break;
				}
			}

			private async void onSave() {
				DB_Communicator db = DB_Communicator.getInstance();
				JsonValue json = await db.UpdateUser(user.name, t.teamrole.role, Convert.ToInt32(t.number.Text), 
					t.position.SelectedItem.ToString(), t.teamrole.teamId);

				//ändernungen im user speichern
				List<VBUser> list = db.createUserFromResponse(json, user.password);
				if(list.Count > 0) {
					VBUser updatedUser = db.createUserFromResponse(json, user.password)[0];
					updatedUser.StoreUserInPreferences(ViewController.getInstance().mainActivity, updatedUser);
				}

				Toast.MakeText(ViewController.getInstance().mainActivity, json["message"].ToString(), ToastLength.Long).Show();
			}

			private void onRequestRank() {
				RequestUserTypeDialog d = new RequestUserTypeDialog(user.idUser, t);
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

				TeamDetailsProfileFragment frag = vc.mainActivity.FindFragmentByTag(tag) as TeamDetailsProfileFragment;
				frag.teamrole = user.getTeamroleForTeam(t.team.id);

				ViewController.getInstance().refreshFragment(ViewController.TEAM_DETAILS_FRAGMENT);
			}
		}
	
		class RequestClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
			public const string ON_ACCEPT_REQUEST="onAcceptRequest", ON_DENIE_REQUEST="onDenieRequest";
			private string source;
			private VBRequest r;
			private TeamDetailsProfileFragment t;

			public RequestClickListener(string source, VBRequest r, TeamDetailsProfileFragment t) {
				this.source = source;
				this.r = r;
				this.t = t;
			}

			public void OnClick(View view) {
				switch(this.source) {
				case ON_ACCEPT_REQUEST:
					this.handleRequest("A");
					break;
				case ON_DENIE_REQUEST:
					this.handleRequest("D");
					break;
				}
			}

			private async void handleRequest(string answer) {
				DB_Communicator db = DB_Communicator.getInstance();
				string response = await db.handleUserTypeRequest(r, answer);
				JsonValue json = JsonValue.Parse(response);
				ViewController.getInstance().toastJson(null, json, ToastLength.Long, "handleRequest");

				//refresh the user
				VBUser user = new VBUser(json["data"]["User"]);
				user.StoreUserInPreferences(ViewController.getInstance().mainActivity, user);

				//refresh the view
				t.teamrole = user.getTeamroleForTeam(r.teamId);
				List<VBRequest> listRequests = db.createReqeuestList(JsonValue.Parse(await db.loadUserTypeRequest(r.teamId)));
				this.t.listRequests = listRequests;
				ViewController.getInstance().refreshFragment(ViewController.TEAM_DETAILS_FRAGMENT);
			}
		}
	}
}

