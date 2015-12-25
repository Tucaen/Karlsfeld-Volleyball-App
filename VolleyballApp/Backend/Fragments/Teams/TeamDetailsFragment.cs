
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

namespace VolleyballApp {
	public class TeamDetailsFragment : Fragment {
		private const string MEMBER="teamDetailsMember", PROFILE="teamDetailsProfile";

		public VBTeam team { get; set; }
		public List<VBRequest> listRequests { get; set; }

		private VBTeamrole teamrole;
		private List<VBUser> listMember { get; set; }
		private TextView tabMember { get; set; }
		private TextView tabProfile { get; set; }

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

			tabMember.SetOnClickListener(new TabClickListener(TabClickListener.ON_TAB_MEMBER, this));
			tabProfile.SetOnClickListener(new TabClickListener(TabClickListener.ON_TAB_PROFILE, this));

			if(DB_Communicator.getInstance().isAtLeast(teamrole.getUserType(), UserType.Member)) {
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsTabs).Visibility = ViewStates.Visible;
			} else {
				view.FindViewById<LinearLayout>(Resource.Id.teamDetailsTabs).Visibility = ViewStates.Gone;
			}

			FrameLayout fragContainer = view.FindViewById<FrameLayout>(Resource.Id.teamDetailsFragmentContainer);

			if(activeFragment == null) {
				this.changeActiveTab(tabMember, tabProfile);
				this.initalizeFragment(TeamDetailsFragment.MEMBER, new TeamDetailsMemberFragment(this.listMember));
			} else {
				if(activeFragment.Equals(TeamDetailsFragment.MEMBER)) {
					this.changeActiveTab(tabMember, tabProfile);
					this.initalizeFragment(TeamDetailsFragment.MEMBER, new TeamDetailsMemberFragment(this.listMember));
				} else if(activeFragment.Equals(TeamDetailsFragment.PROFILE)) {
					this.changeActiveTab(tabProfile, tabMember);
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
			trans.AddToBackStack(activeFragmentTag);
			trans.CommitAllowingStateLoss();
		}

		private void changeActiveTab(TextView activeTab, TextView inactiveTab) {
			this.markTabAsActive(activeTab);
			this.markTabAsInactive(inactiveTab);
		}

		private void markTabAsActive(TextView activeTab) {
			activeTab.SetBackgroundColor(Color.ParseColor("#333333"));
			activeTab.PaintFlags = PaintFlags.UnderlineText;
		}

		private void markTabAsInactive(TextView inactiveTab) {
			inactiveTab.SetBackgroundColor(Color.ParseColor("#000000"));
			inactiveTab.SetTypeface(inactiveTab.Typeface, TypefaceStyle.Normal);
		}

		class TeamDetailsClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
			public const string ON_JOIN = "onJoin", ON_FOLLOW = "onFollow", ON_LEAVE = "onLeave";
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
				ViewController vc = ViewController.getInstance();
				string tag = ViewController.TEAM_DETAILS_FRAGMENT;

				TeamDetailsFragment frag = vc.mainActivity.FindFragmentByTag(tag) as TeamDetailsFragment;
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
						t.changeActiveTab(t.tabMember, t.tabProfile);
						Fragment frag = new TeamDetailsMemberFragment(t.listMember);
						this.switchFragment(t.activeFragment, TeamDetailsFragment.MEMBER, frag);
					}
					break;
				case ON_TAB_PROFILE:
					if(!t.activeFragment.Equals(TeamDetailsFragment.PROFILE)) {
						t.changeActiveTab(t.tabProfile, t.tabMember);
						Fragment frag = new TeamDetailsProfileFragment(t.team, t.teamrole, t.listRequests);
						this.switchFragment(t.activeFragment, TeamDetailsFragment.PROFILE, frag);
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
	}
}

