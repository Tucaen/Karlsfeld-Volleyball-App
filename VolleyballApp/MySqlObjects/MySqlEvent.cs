﻿using System;
using Android.Content;
using System.Collections.Generic;
using Java.Interop;
using Android.OS;
using System.Linq;

namespace VolleyballApp {
	public class MySqlEvent : MySqlObject {
		public int idEvent { get; set; }
		public string name { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public string location { get; set; }
		public string state { get; set; }
		private static Intent intent;

		public MySqlEvent(){}

		public MySqlEvent(int idEvent, string name, DateTime startDate, DateTime endDate, string location, string state) {
			this.idEvent = idEvent;
			this.name = name;
			this.startDate = startDate;
			this.endDate = endDate;
			this.location = location;
			this.state = state;
		}

		public void StoreEventInPreferences(Context context, MySqlEvent e) {
			ISharedPreferences prefs = context.GetSharedPreferences("eventinformation", FileCreationMode.Private);
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutInt("idEvent", e.idEvent);
			editor.PutString("eventName", e.name);
			editor.PutString("eventLocation", e.location);
			editor.PutString("eventState", e.state);
			editor.PutString("eventStartDate", e.startDate.ToString());
			editor.PutString("eventEndDate", e.endDate.ToString());
			editor.Commit();
		}

		public static MySqlEvent GetEventFromPreferences(Context context) {
			ISharedPreferences prefs = context.GetSharedPreferences("eventinformation", FileCreationMode.Private);
			return new MySqlEvent(prefs.GetInt("idEvent", 0),
				prefs.GetString("eventName", ""),
				Convert.ToDateTime(prefs.GetString("eventStartDate", "")),
				Convert.ToDateTime(prefs.GetString("eventEndDate", "")),
				prefs.GetString("eventLocation", ""),
				prefs.GetString("eventState", ""));
		}

		public static void StoreEventListInPreferences(Intent intent, List<MySqlEvent> listEvents) {
			MySqlEvent.intent = intent;
			MySqlEvent[] array = new MySqlEvent[listEvents.Count];
			for(int i = 0; i < array.Length; i++) {
				array[i] = listEvents[i];
			}
			intent.PutParcelableArrayListExtra("listEvents", array);
			MySqlEvent[] tempArray = intent.GetParcelableArrayListExtra("listEvents").Cast<MySqlEvent>().ToArray();
		}

		public static List<MySqlEvent> GetListEventsFromPreferences() {
			List<MySqlEvent> listEvents = new List<MySqlEvent>();
			MySqlEvent[] array = MySqlEvent.intent.GetParcelableArrayListExtra("listEvents").Cast<MySqlEvent>().ToArray();
			for(int i = 0; i < array.Length; i++) {
				listEvents.Add(array[i]);
			}
			return listEvents;
		}

		public override string ToString() {
			return "[Event id=" + idEvent + ";name=" + name + ";startDate=" + startDate + ";endDate=" + endDate + ";location=" + location + ";state=" + state + "]";
		}

		#region ParcelableImplementation
		public MySqlEvent(Parcel p) {
			this.idEvent = p.ReadInt();
			this.name = p.ReadString();
			this.startDate = Convert.ToDateTime(p.ReadString());
			this.endDate = Convert.ToDateTime(p.ReadString());
			this.location = p.ReadString();
			this.state = p.ReadString();
		}
		
		public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags) {
			dest.WriteInt(idEvent);
			dest.WriteString(name);
			dest.WriteString(startDate.ToString());
			dest.WriteString(endDate.ToString());
			dest.WriteString(location);
			dest.WriteString(state);
		}

		public static readonly MyParcelableCreator<MySqlEvent> _creator 
		= new MyParcelableCreator<MySqlEvent>((parcel) => new MySqlEvent(parcel));

		[ExportField ("CREATOR")]
		public static MyParcelableCreator<MySqlEvent> InitializeCreator() {
			return _creator;
		}
		#endregion
	}
}

