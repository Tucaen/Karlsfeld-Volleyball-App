﻿
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
using System.Threading.Tasks;

namespace VolleyballApp {
	public class EditEventFragment : Fragment {
		View view;
		VBEvent _event;
		private List<VBTeam> listTeams;

		public EditEventFragment(VBEvent _event, List<VBTeam> listTeams) {
			this._event = _event;
			this.listTeams = new List<VBTeam>();
			VBTeam t = new VBTeam();
			t.name = "Keines";
			this.listTeams.Add(t);
			foreach(VBTeam team in listTeams) {
				this.listTeams.Add(team);
			}
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			view = null;

			view = inflater.Inflate(Resource.Layout.AddEvent, container, false);

			//get view components
			EditText name = view.FindViewById<EditText>(Resource.Id.addEventName);
			EditText location = view.FindViewById<EditText>(Resource.Id.addEventLocation);
			EditText info = view.FindViewById<EditText>(Resource.Id.addEventEventDescriptionValue);
			TextView startDate = view.FindViewById<TextView>(Resource.Id.addEventStartDateValue);
			TextView startTime = view.FindViewById<TextView>(Resource.Id.addEventStartDateTimeValue);
			TextView endDate = view.FindViewById<TextView>(Resource.Id.addEventEndDateValue);
			TextView endTime = view.FindViewById<TextView>(Resource.Id.addEventEndDateTimeValue);
			Button btnSave = view.FindViewById<Button>(Resource.Id.btnCreateEvent);
			Spinner teamIdSpinner = view.FindViewById<Spinner>(Resource.Id.addEventTeamIdValue);

			SpinnerTeamAdapter adapter = new SpinnerTeamAdapter(this, Resource.Layout.SpinnerTextView, listTeams);
			adapter.SetDropDownViewResource(Resource.Layout.SpinnerCheckedLayout);
			teamIdSpinner.Adapter = adapter;
			teamIdSpinner.SetSelection(this.getPositionOfTeam(_event.teamId), false);

			//initialize components
			name.Text = _event.name;
			location.Text = _event.location;
			info.Text = _event.description.Replace("\\n", "\n");
			startDate.Text = _event.startDate.ToString("dd.MM.yyyy"); 	startTime.Text = _event.startDate.ToString("HH:mm");
			endDate.Text = _event.endDate.ToString("dd.MM.yyyy");		endTime.Text = _event.endDate.ToString("HH:mm");
			btnSave.Text = "Speichern";

			//onClick events
			startDate.Click += delegate { pickDate(startDate, endDate, _event.startDate); };
			startTime.Click += delegate { pickTime(startTime, endTime, _event.startDate); };
			endDate.Click += delegate { pickDate(endDate, _event.endDate); };
			endTime.Click += delegate { pickTime(endTime, _event.endDate); };

			btnSave.Click += async delegate {
				//Excepted format by php 2015-10-30T19:00:00
				string start = ViewController.getInstance().convertDateForDb(startDate.Text) + "T" + startTime.Text + ":00";
				string end = ViewController.getInstance().convertDateForDb(endDate.Text) + "T" + endTime.Text + ":00";

				VBTeam team = teamIdSpinner.SelectedItem as VBTeam;

				JsonValue json = await DB_Communicator.getInstance().
					updateEvent(_event.idEvent, name.Text, location.Text, start, end, info.Text, team.id);

				Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long).Show();

				ViewController.getInstance().hideSoftKeyboard();
				this.finish(json);
			};

			return view;
		}

		private int getPositionOfTeam (int teamId) {
			for(int i = 0; i < listTeams.Count; i++) {
				if(listTeams[i].id == teamId)
					return i;
			}
			return 0;
		}

		/**
		 * Reloads all events for this user and goes back to the EVENT_DETAILS_FRAGMENT
		 **/
		private async void finish(JsonValue json) {
			if(DB_Communicator.getInstance().wasSuccesful(json)) {
				MainActivity main = this.Activity as MainActivity;

				EventDetailsFragment frag = (FragmentManager.FindFragmentByTag(ViewController.EVENT_DETAILS_FRAGMENT) as EventDetailsFragment);
				if(frag != null)
					frag._event = await ViewController.getInstance().refreshDataForEvent(_event.idEvent);
				
				main.popBackstack();
			}
		}

		private void pickDate(TextView t, DateTime date) {
			AppDateUpdater picker = new AppDateUpdater(t, date);
			picker.Show(FragmentManager, "datePicker");
		}

		private void pickDate(TextView t, TextView t2, DateTime date) {
			AppDateUpdater picker = new AppDateUpdater(t, t2, date);
			picker.Show(FragmentManager, "datePicker");
		}
			
		private void pickTime(TextView t, DateTime date) {
			AppTimeUpdater picker = new AppTimeUpdater(t, date);
			picker.Show(FragmentManager, "timePicker");
		}

		private void pickTime(TextView t, TextView t2, DateTime date) {
			AppTimeUpdater picker = new AppTimeUpdater(t, t2, date);
			picker.Show(FragmentManager, "timePicker");
		}
	}

	class AppDateUpdater : DialogFragment, DatePickerDialog.IOnDateSetListener {
		private TextView t, t2;
		private DateTime date;

		public AppDateUpdater(TextView t, DateTime date) : this(t, null, date) {}

		public AppDateUpdater(TextView t, TextView t2, DateTime date) {
			this.t = t;
			this.t2 = t2;
			this.date = date;
		}

		public override Dialog OnCreateDialog(Bundle savedInstance) {
			int year = date.Year;
			int month = (date.Month - 1);
			int day = date.Day;

			return new DatePickerDialog(this.Activity, this, year, month, day);
		}

		public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
			t.Text = dayOfMonth + "." + (monthOfYear + 1) + "." + year;

			if(t2 != null)
				t2.Text = dayOfMonth + "." + (monthOfYear + 1) + "." + year;
		}
	}

	class AppTimeUpdater : DialogFragment, TimePickerDialog.IOnTimeSetListener {
		private TextView t, t2;
		private DateTime date;

		public AppTimeUpdater(TextView t, DateTime date) : this(t, null, date) {}

		public AppTimeUpdater(TextView t, TextView t2, DateTime date) {
			this.t = t;
			this.t2 = t2;
			this.date = date;
		}

		public override Dialog OnCreateDialog(Bundle savedInstance) {
			int hourOfDay = date.Hour;
			int minute = date.Minute;

			return new TimePickerDialog(this.Activity, this, hourOfDay, minute, true);
		}

		public void OnTimeSet(TimePicker view, int hourOfDay, int minute) {
			string temp = minute.ToString();
			if(minute < 10)
				temp = "0" + minute;
			
			t.Text = hourOfDay + ":" + temp;

			if(t2 != null)
				t2.Text = (hourOfDay+2) + ":" + temp;
		}
	}
}

