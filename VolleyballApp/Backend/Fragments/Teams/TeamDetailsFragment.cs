
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
using Android.Graphics;
using System.Threading.Tasks;
using Android.Graphics.Drawables;

namespace VolleyballApp {
	public class TeamDetailsFragment : Fragment {
		public const string MEMBER="teamDetailsMember", PROFILE="teamDetailsProfile";

		public VBTeam team { get; set; }
		public List<VBRequest> listRequests { get; set; }

		private VBTeamrole teamrole;
		private List<VBUser> listMember { get; set; }
		private TextView tabMember { get; set; }
		private TextView tabProfile { get; set; }
		private View tabMemberUnderline { get; set; }
		private View tabProfileUnderline { get; set; }

		private FragmentTransaction trans;
		private string activeFragment;

		public TeamDetailsFragment(VBTeam team, VBTeamrole teamrole, List<VBRequest> listRequests, List<VBUser> listMember) {
			this.team = team;
			this.teamrole = teamrole;
			this.listRequests = listRequests;
			this.listMember = listMember;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			VBUser user = VBUser.GetUserFromPreferences();

			View view = inflater.Inflate(Resource.Layout.TeamDetailsFragment, container, false);

			#region toolbar
			ImageView btnEdit = this.Activity.FindViewById<ImageView>(Resource.Id.btnEditInToolbar);
			ImageView btnDelete = this.Activity.FindViewById<ImageView>(Resource.Id.btnDeleteInToolbar);

			if(DB_Communicator.getInstance().isAtLeast(user.getTeamroleForTeam(this.team.id).getUserType(), UserType.Operator)) {
				btnEdit.Visibility = ViewStates.Visible;
				btnDelete.Visibility = ViewStates.Visible;

				btnEdit.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_EDIT, this));
				btnDelete.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_DELETE, this));
			} else {
				btnEdit.Visibility = ViewStates.Gone;
				btnDelete.Visibility = ViewStates.Gone;
			}
			#endregion

			#region header
			view.FindViewById<TextView>(Resource.Id.teamDetailsName).Text = team.name;
			view.FindViewById<TextView>(Resource.Id.teamDetailsSport).Text = team.sport;
			if(!team.location.Equals("") && !team.sport.Equals("")) {
				view.FindViewById<TextView>(Resource.Id.teamDetailsLocation).Text = team.location + " - ";
			} else {
				view.FindViewById<TextView>(Resource.Id.teamDetailsLocation).Text = team.location;
			}
			#endregion

			#region tabs
			tabMember = view.FindViewById<TextView>(Resource.Id.teamDetailsTabMember);
			tabProfile = view.FindViewById<TextView>(Resource.Id.teamDetailsTabProfile);
			tabMemberUnderline = view.FindViewById<View>(Resource.Id.teamDetailsTabMemberUnderline);
			tabProfileUnderline = view.FindViewById<View>(Resource.Id.teamDetailsTabProfileUnderline);

			tabMember.SetOnClickListener(new TabClickListener(TabClickListener.ON_TAB_MEMBER, this));
			tabProfile.SetOnClickListener(new TabClickListener(TabClickListener.ON_TAB_PROFILE, this));

			if(DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Member)) {
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsTabs).Visibility = ViewStates.Visible;
			} else {
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsTabs).Visibility = ViewStates.Gone;
			}

			FrameLayout fragContainer = view.FindViewById<FrameLayout>(Resource.Id.teamDetailsFragmentContainer);

			if(activeFragment == null) {
				this.changeActiveTab(tabMember, tabMemberUnderline, tabProfile, tabProfileUnderline);
				this.initalizeFragment(TeamDetailsFragment.MEMBER, new TeamDetailsMemberFragment(this.listMember));
			} else {
				if(activeFragment.Equals(TeamDetailsFragment.MEMBER)) {
					this.changeActiveTab(tabMember, tabMemberUnderline, tabProfile, tabProfileUnderline);
					this.initalizeFragment(TeamDetailsFragment.MEMBER, new TeamDetailsMemberFragment(this.listMember));
				} else if(activeFragment.Equals(TeamDetailsFragment.PROFILE)) {
					this.changeActiveTab(tabProfile, tabProfileUnderline, tabMember, tabMemberUnderline);
					this.initalizeFragment(TeamDetailsFragment.PROFILE, new TeamDetailsProfileFragment(this.team, this.teamrole, this.listRequests));
				}
			}
			#endregion

			#region initialize buttons
			Button btnJoin = view.FindViewById<Button>(Resource.Id.teamDetailsBtnJoin);
			Button btnFollow = view.FindViewById<Button>(Resource.Id.teamDetailsBtnFollow);
			Button btnLeave = view.FindViewById<Button>(Resource.Id.teamDetailsBtnLeave);

			btnJoin.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_JOIN, this));
			btnFollow.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_FOLLOW, this));
			btnLeave.SetOnClickListener(new TeamDetailsClickListener(TeamDetailsClickListener.ON_LEAVE, this));

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
			#endregion

			return view;
		}

		private void initalizeFragment(string activeFragmentTag, Fragment frag) {
			activeFragment = activeFragmentTag;
			trans = FragmentManager.BeginTransaction();
			trans.Add(Resource.Id.teamDetailsFragmentContainer, frag, activeFragmentTag);
			trans.CommitAllowingStateLoss();
		}

		private void changeActiveTab(TextView activeTab, View activeUnderline, TextView inactiveTab, View inactiveUnderline) {
			this.markTabAsActive(activeTab, activeUnderline);
			this.markTabAsInactive(inactiveTab, inactiveUnderline);
		}

		private void markTabAsActive(TextView activeTab, View underline) {
			activeTab.SetBackgroundColor(Color.ParseColor("#000000"));
			underline.Visibility = ViewStates.Visible;
			//			activeTab.PaintFlags = PaintFlags.UnderlineText;
		}

		private void markTabAsInactive(TextView inactiveTab, View underline) {
			inactiveTab.SetBackgroundColor(Color.ParseColor("#333333"));
			underline.Visibility = ViewStates.Gone;

//			inactiveTab.PaintFlags = 0;
		}

		public static TeamDetailsFragment findTeamDetailsFragment() {
			return ViewController.getInstance().mainActivity.FindFragmentByTag(ViewController.TEAM_DETAILS_FRAGMENT) as TeamDetailsFragment;
		}

		public override void OnDestroyView() {
			base.OnDestroyView();
			this.Activity.FindViewById<ImageView>(Resource.Id.btnAddInToolbar).Visibility = ViewStates.Gone;
			this.Activity.FindViewById<ImageView>(Resource.Id.btnEditInToolbar).Visibility = ViewStates.Gone;
			this.Activity.FindViewById<ImageView>(Resource.Id.btnDeleteInToolbar).Visibility = ViewStates.Gone;
		}

		#region clicklistener
		class TeamDetailsClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
			public const string ON_JOIN = "onJoin", ON_FOLLOW = "onFollow", ON_LEAVE = "onLeave",
								ON_EDIT = "onEdit", ON_DELETE = "onDelete";
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
				case ON_JOIN:
					this.onJoin(t.team.id);
					break;
				case ON_FOLLOW:
					this.onFollow(t.team.id);
					break;
				case ON_LEAVE:
					this.onLeave();
					break;
				case ON_EDIT:
					this.onEdit();
					break;
				case ON_DELETE:
					this.onDelete();
					break;
				}
			}

			private async void onJoin(int teamId) {
				string response = await DB_Communicator.getInstance().makeWebRequest("service/team/join_team.php?id=" + teamId + "&type=M",
					"TeamDetailsFragment.onJoin");
				this.updateTeamrole(response);
				await this.updateListMember();
				this.refreshTeamDetailsFragment();
			}

			private async void onFollow(int teamId) {
				string response = await DB_Communicator.getInstance().makeWebRequest("service/team/follow_team.php?id=" + teamId,
					"TeamDetailsFragment.onFollow");

				this.updateTeamrole(response);
				await this.updateListMember();
				this.refreshTeamDetailsFragment();
			}

			private async void onLeave() {
				string response = await DB_Communicator.getInstance().makeWebRequest("service/team/leave_team.php" +
					"?userId=" + user.idUser +"&teamId=" + t.team.id, 
					"TeamDetailsFragment.onLeave");

				user.removeTeamrole(t.teamrole.teamId);
				await this.updateListMember();
				this.refreshTeamDetailsFragment();
			}

			private void onEdit() {
				ViewController.getInstance().mainActivity.
					switchFragment(ViewController.TEAM_DETAILS_FRAGMENT, ViewController.EDIT_TEAM_FRAGMENT, new EditTeamFragment(t.team));
			}

			private async void onDelete() {
				ViewController vc = ViewController.getInstance();

				JsonValue json = JsonValue.Parse(await DB_Communicator.getInstance().deleteTeam(t.team.id));

				vc.toastJson(null, json, ToastLength.Long, "");

				//refresh team list
				TeamsFragment tf = vc.mainActivity.FindFragmentByTag(ViewController.TEAMS_FRAGMENT) as TeamsFragment;
				tf.listTeams = await DB_Communicator.getInstance().SelectTeams();

				VBUser.GetUserFromPreferences().removeTeamrole(t.team.id);

				vc.mainActivity.popBackstack();
			}

			/**
			 * Updates the user with the teamrole in the response
			 * Doesn't affect the view!
			 * To refresh the GUI call refreshTeamDetailsFragment();
			 **/
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

			private async Task<List<VBUser>> updateListMember() {
				DB_Communicator db = DB_Communicator.getInstance();
				List<VBUser> listMember = db.createMemberList(JsonValue.Parse(await db.loadMember(t.team.id)));
				t.listMember = listMember;
				return listMember;
			}

			private void refreshTeamDetailsFragment() {
				TeamDetailsFragment frag = TeamDetailsFragment.findTeamDetailsFragment();
				frag.teamrole = user.getTeamroleForTeam(t.team.id);

				ViewController.getInstance().refreshFragment(ViewController.TEAM_DETAILS_FRAGMENT);
			}
		}
	
		class TabClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
			public const string ON_TAB_MEMBER = "onTabMember", ON_TAB_PROFILE = "onTabProfile";
			private string source;
			private TeamDetailsFragment t;

			public TabClickListener(string source, TeamDetailsFragment t) {
				this.source = source;
				this.t = t;
			}

			public void OnClick(View view) {
				switch(this.source) {
				case ON_TAB_MEMBER:
					if(!t.activeFragment.Equals(TeamDetailsFragment.MEMBER)) {
						t.changeActiveTab(t.tabMember, t.tabMemberUnderline, t.tabProfile, t.tabProfileUnderline);
						Fragment frag = new TeamDetailsMemberFragment(t.listMember);
						this.switchFragment(t.activeFragment, TeamDetailsFragment.MEMBER, frag);
					}
					break;
				case ON_TAB_PROFILE:
					try {
						if(!t.activeFragment.Equals(TeamDetailsFragment.PROFILE)) {
							t.changeActiveTab(t.tabProfile, t.tabProfileUnderline, t.tabMember, t.tabMemberUnderline);
							Fragment frag = new TeamDetailsProfileFragment(t.team, t.teamrole, t.listRequests);
							this.switchFragment(t.activeFragment, TeamDetailsFragment.PROFILE, frag);
						}
					} catch (System.Exception e) {
						Toast.MakeText(ViewController.getInstance().mainActivity, "Error! " + e.Message, ToastLength.Long);
					}
					break;
				}
			}

			private void switchFragment(string oldFragmentTag, string newFragmentTag, Fragment newFragment) {
				t.activeFragment = newFragmentTag;
				t.trans = t.FragmentManager.BeginTransaction();

				Fragment oldFragment = t.FragmentManager.FindFragmentByTag(oldFragmentTag);
				if(oldFragment != null)
					t.trans.Remove(oldFragment);

				t.trans.Add(Resource.Id.teamDetailsFragmentContainer, newFragment, newFragmentTag);
				t.trans.Commit();
			}
		}
		#endregion
	}
}

