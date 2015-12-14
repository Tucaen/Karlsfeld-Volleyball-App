
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
	public class AddEventFragment : Fragment {
		View view;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			view = null;

			view = inflater.Inflate(Resource.Layout.AddEvent, container, false);

			EditText name = view.FindViewById<EditText>(Resource.Id.addEventName);
			EditText location = view.FindViewById<EditText>(Resource.Id.addEventLocation);
			EditText info = view.FindViewById<EditText>(Resource.Id.addEventEventDescriptionValue);
			TextView startDate = view.FindViewById<TextView>(Resource.Id.addEventStartDateValue);
			TextView startTime = view.FindViewById<TextView>(Resource.Id.addEventStartDateTimeValue);
			TextView endDate = view.FindViewById<TextView>(Resource.Id.addEventEndDateValue);
			TextView endTime = view.FindViewById<TextView>(Resource.Id.addEventEndDateTimeValue);

			startDate.Click += delegate { pickDate(startDate, endDate); };
			startTime.Click += delegate { pickTime(startTime, endTime); };
			endDate.Click += delegate { pickDate(endDate); };
			endTime.Click += delegate { pickTime(endTime); };

			view.FindViewById<Button>(Resource.Id.btnCreateEvent).Click += async delegate {
				ProgressDialog d = (this.Activity as MainActivity).createProgressDialog("Please wait!", "Creating event...");
				//Excepted format by php 2015-10-30T19:00:00

				string start = ViewController.getInstance().convertDateForDb(startDate.Text) + "T" + startTime.Text + ":00";
				string end = ViewController.getInstance().convertDateForDb(endDate.Text) + "T" + endTime.Text + ":00";

				JsonValue json = await DB_Communicator.getInstance().createEvent(name.Text, location.Text, start, end, info.Text);

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

	class AppDatePicker : DialogFragment, DatePickerDialog.IOnDateSetListener {
		private TextView t, t2;

		public AppDatePicker(TextView t) : this(t, null) {}

		public AppDatePicker(TextView t, TextView t2) {
			this.t = t;
			this.t2 = t2;
		}

		public override Dialog OnCreateDialog(Bundle savedInstance) {
			Calendar c = Calendar.GetInstance(Java.Util.Locale.Germany);
			int year;
			int month;
			int day;

			if(t.Text.Equals("Datum")) {
				year = c.Get(CalendarField.Year);
				month = c.Get(CalendarField.Month);
				day = c.Get(CalendarField.DayOfMonth);
			} else {
				string[] date = t.Text.Split('.');
				year = Convert.ToInt32(date[2]);
				month = (Convert.ToInt32(date[1])-1);
				day = Convert.ToInt32(date[0]);
			}

			return new DatePickerDialog(this.Activity, this, year, month, day);
		}

		public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
//			t.Text = year + "-" + (monthOfYear+1) + "-" + dayOfMonth;
//
//			if(t2 != null)
//				t2.Text = year + "-" + (monthOfYear+1) + "-" + dayOfMonth;
			t.Text = dayOfMonth + "." + (monthOfYear+1) + "." + year;

			if(t2 != null)
				t2.Text = dayOfMonth + "." + (monthOfYear + 1) + "." + year;
		}
	}

	class AppTimePicker : DialogFragment, TimePickerDialog.IOnTimeSetListener {
		private TextView t, t2;

		public AppTimePicker(TextView t) : this(t, null) {}

		public AppTimePicker(TextView t, TextView t2) {
			this.t = t;
			this.t2 = t2;
		}

		public override Dialog OnCreateDialog(Bundle savedInstance) {
			Calendar c = Calendar.GetInstance(Java.Util.Locale.Germany);
			int hourOfDay = c.Get(CalendarField.HourOfDay);
			int minute = c.Get(CalendarField.Minute);

			return new TimePickerDialog(this.Activity, this, hourOfDay, minute, true);
		}

		public void OnTimeSet(TimePicker view, int hourOfDay, int minute) {
			string temp = minute.ToString();
			if(minute < 10)
				temp = "0" + minute;
			
			t.Text = hourOfDay + ":" + temp;

			if(t2 != null) {
				int hour = (hourOfDay + 2 > 23) ? hourOfDay : hourOfDay + 2;
				t2.Text = hour + ":" + temp;
			}
		}
	}
}

