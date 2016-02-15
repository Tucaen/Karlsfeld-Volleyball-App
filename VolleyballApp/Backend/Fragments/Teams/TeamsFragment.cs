
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
using System.Threading.Tasks;
using System.Threading;
using System.Json;

namespace VolleyballApp {
	public class TeamsFragment : Fragment {
		ListView listView;
		public List<VBTeam> listTeams { get; set; }
		View view;

		public TeamsFragment(List<VBTeam> listTeams) {
			this.listTeams = listTeams;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			view = inflater.Inflate(Resource.Layout.TeamsFragment, container, false);
			if(listTeams.Count == 0) {
				//display text that there are currently no events and hide list with events
				view.FindViewById(Resource.Id.teamsFragmentListTeams).Visibility = ViewStates.Gone;
			} else {
				//display list with events and hide the text
				view.FindViewById(Resource.Id.teamsFragmentNoTeams).Visibility = ViewStates.Gone;

				listView = view.FindViewById<ListView>(Resource.Id.teamsFragmentListTeams);
				listView.Adapter = new ListTeamsAdapter(this, listTeams);
				listView.ItemClick += OnListItemClick;
			}

			if(DB_Communicator.getInstance().isAtLeast(VBUser.GetUserFromPreferences().getUserType(), UserType.Coremember)) {
				view.FindViewById<LinearLayout>(Resource.Id.teamsFragmentBtnAddLine).Visibility = ViewStates.Visible;
			} else {
				view.FindViewById<LinearLayout>(Resource.Id.teamsFragmentBtnAddLine).Visibility = ViewStates.Gone;
			}

			view.FindViewById<Button>(Resource.Id.teamsFragmentBtnAdd).Click += (object sender, EventArgs e) => {
				ViewController.getInstance().mainActivity.switchFragment(ViewController.TEAMS_FRAGMENT, 
					ViewController.ADD_TEAM_FRAGMENT, new AddTeamFragment());
			};

			return view;
		}

		private async void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			VBTeam team = listTeams[e.Position];
			VBUser user = VBUser.GetUserFromPreferences();
			DB_Communicator db = DB_Communicator.getInstance();
			List<VBRequest> listRequests = db.createReqeuestList(JsonValue.Parse(await db.loadUserTypeRequest(team.id)));
			List<VBUser> listMember = db.createMemberList(JsonValue.Parse(await db.loadMember(team.id)));

			if(listMember != null && listRequests != null) {
				List<VBUser> sortedListMember = ViewController.getInstance().sortUserlistForTeam(listMember, team.id);
				
				TeamDetailsFragment frag = new TeamDetailsFragment(team, user.getTeamroleForTeam(team.id), listRequests, sortedListMember);
				
				ViewController.getInstance().mainActivity.switchFragment(
					ViewController.TEAMS_FRAGMENT, ViewController.TEAM_DETAILS_FRAGMENT, frag);
			}
		}
	}
}

