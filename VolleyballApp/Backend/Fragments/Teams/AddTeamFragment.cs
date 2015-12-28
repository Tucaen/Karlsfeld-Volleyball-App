
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
	public class AddTeamFragment : Fragment {
		View view;
		private EditText name, sport, location, info;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			view = null;

			view = inflater.Inflate(Resource.Layout.AddTeamFragment, container, false);

			name = view.FindViewById<EditText>(Resource.Id.addTeamName);
			sport = view.FindViewById<EditText>(Resource.Id.addTeamSport);
			location = view.FindViewById<EditText>(Resource.Id.addTeamLocation);
			info = view.FindViewById<EditText>(Resource.Id.addTeamDescription);


			Button btnAdd = view.FindViewById<Button>(Resource.Id.addTeamBtnCreate);
			btnAdd.SetOnClickListener(new AddTeamClickListener(AddTeamClickListener.ON_ADD, this));

			return view;
		}

		class AddTeamClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
			public const string ON_ADD = "onAdd";
			private string source;
			private AddTeamFragment t;

			public AddTeamClickListener(string source, AddTeamFragment t) {
				this.source = source;
				this.t = t;
			}

			public void OnClick(View view) {
				switch(this.source) {
				case ON_ADD:
					this.onAdd();
					break;
				}
			}

			private async void onAdd() {
				if(t.name.Text == null || t.name.Text.Equals("")) {
					Toast.MakeText(ViewController.getInstance().mainActivity, "You have to enter a name for your team!", ToastLength.Long).Show();
				} else {
					ProgressDialog d = ViewController.getInstance().mainActivity.createProgressDialog("Please wait!", "Creating team...");
					
					JsonValue json = await DB_Communicator.getInstance().createTeam(t.name.Text, t.sport.Text, t.location.Text, t.info.Text);
					
					Toast.MakeText(ViewController.getInstance().mainActivity, json["message"].ToString(), ToastLength.Long).Show();

					//update team list
					TeamsFragment tf = ViewController.getInstance().mainActivity.FindFragmentByTag(ViewController.TEAMS_FRAGMENT) as TeamsFragment;
					tf.listTeams = await DB_Communicator.getInstance().SelectTeams();

					//update teamrole
					VBUser user = VBUser.GetUserFromPreferences();
					user.listTeamRole.Add(new VBTeamrole(json["data"]["TeamRole"]));
					user.StoreUserInPreferences(ViewController.getInstance().mainActivity, user);

					ViewController.getInstance().hideSoftKeyboard();

					ViewController.getInstance().mainActivity.popBackstack();
					d.Dismiss();
				}
			}
		}
	}
}

