
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

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			view = null;

			view = inflater.Inflate(Resource.Layout.AddTeamFragment, container, false);

			EditText name = view.FindViewById<EditText>(Resource.Id.addTeamName);
			EditText sport = view.FindViewById<EditText>(Resource.Id.addTeamSport);
			EditText location = view.FindViewById<EditText>(Resource.Id.addTeamLocation);
			EditText info = view.FindViewById<EditText>(Resource.Id.addTeamDescription);


			view.FindViewById<Button>(Resource.Id.addTeamBtnCreate).Click += async delegate {
				ProgressDialog d = (this.Activity as MainActivity).createProgressDialog("Please wait!", "Creating team...");

				JsonValue json = await DB_Communicator.getInstance().createTeam(name.Text, sport.Text, location.Text, info.Text);

				Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long).Show();

				this.finish(json);
				d.Dismiss();
			};

			return view;
		}

		/**
		 * Reloads all events for this user and goes back to the EVENTS_FRAGMENT
		 **/
		private async void finish(JsonValue json) {
			if(DB_Communicator.getInstance().wasSuccesful(json)) {
				MainActivity main = this.Activity as MainActivity;
				await ViewController.getInstance().refreshEvents();
				main.popBackstack();
			}
		}

		private void pickDate(TextView t) {
			AppDatePicker picker = new AppDatePicker(t);
			picker.Show(FragmentManager, "datePicker");
		}

		private void pickDate(TextView t, TextView t2) {
			AppDatePicker picker = new AppDatePicker(t, t2);
			picker.Show(FragmentManager, "datePicker");
		}
			
		private void pickTime(TextView t) {
			AppTimePicker picker = new AppTimePicker(t);
			picker.Show(FragmentManager, "timePicker");
		}

		private void pickTime(TextView t, TextView t2) {
			AppTimePicker picker = new AppTimePicker(t, t2);
			picker.Show(FragmentManager, "timePicker");
		}
	}
}

