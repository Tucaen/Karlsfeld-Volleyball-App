
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
using Java.Util;

namespace VolleyballApp {
	public class EditTeamFragment : Fragment {
		View view;
		private VBTeam team;
		private EditText name, sport, location, info;

		public EditTeamFragment(VBTeam team) {
			this.team = team;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			view = null;

			view = inflater.Inflate(Resource.Layout.AddTeamFragment, container, false);

			#region initialize
			name = view.FindViewById<EditText>(Resource.Id.addTeamName);
			sport = view.FindViewById<EditText>(Resource.Id.addTeamSport);
			location = view.FindViewById<EditText>(Resource.Id.addTeamLocation);
			info = view.FindViewById<EditText>(Resource.Id.addTeamDescription);

			name.Text = team.name;
			sport.Text = team.sport;
			location.Text = team.location;
			info.Text = team.description;

			Button btnSave = view.FindViewById<Button>(Resource.Id.addTeamBtnCreate);
			btnSave.Text = "Speichern";

			btnSave.SetOnClickListener(new EditTeamClickListener(EditTeamClickListener.ON_SAVE, this));
			#endregion

			return view;
		}

		class EditTeamClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
			public const string ON_SAVE = "onSave";
			private string source;
			private EditTeamFragment t;
			
			public EditTeamClickListener(string source, EditTeamFragment t) {
				this.source = source;
				this.t = t;
			}
			
			public void OnClick(View view) {
				switch(this.source) {
				case ON_SAVE:
					this.onSave();
					break;
				}
			}
			
			private async void onSave() {
				t.team.name = t.name.Text;
				t.team.sport = t.sport.Text;
				t.team.location = t.location.Text;
				t.team.description = t.info.Text;

				JsonValue json = JsonValue.Parse(await DB_Communicator.getInstance().updateTeam(t.team));
				
				if(DB_Communicator.getInstance().wasSuccesful(json)) {
					VBTeam team = new VBTeam(json["data"]["Team"]);
					TeamDetailsFragment.findTeamDetailsFragment().team = team;
					ViewController.getInstance().hideSoftKeyboard();

					ViewController.getInstance().mainActivity.popBackstack();
				}
			}
		}
	}
}

