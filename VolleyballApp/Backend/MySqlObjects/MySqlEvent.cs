using System;
using Android.Content;
using System.Collections.Generic;
using Java.Interop;
using Android.OS;
using System.Linq;

namespace VolleyballApp {
	public class MySqlEvent {
		public int idEvent { get; set; }
		public string name { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public string location { get; set; }
		public string state { get; set; }
//		private static Intent intent;

		/** Constructor for Parcelable **/
		public MySqlEvent(){}

		public MySqlEvent(int idEvent, string name, DateTime startDate, DateTime endDate, string location, string state) {
			this.idEvent = idEvent;
			this.name = name;
			this.startDate = startDate;
			this.endDate = endDate;
			this.location = location;

			DB_Communicator db = DB_Communicator.getInstance();
			switch(state) {
			case "G":
				this.state = DB_Communicator.State.Accepted;
				break;
			case "M":
				this.state = "Vielleicht";
				break;
			case "D":
				this.state = DB_Communicator.State.Denied;
				break;
			default:
				this.state = DB_Communicator.State.Invited;
				break;
			}
		}

//		public static void StoreEventListInPreferences(Intent intent, List<MySqlEvent> listEvents) {
//			MySqlEvent.intent = intent;
//			MySqlEvent[] array = new MySqlEvent[listEvents.Count];
//			for(int i = 0; i < array.Length; i++) {
//				array[i] = listEvents[i];
//			}
//			intent.PutParcelableArrayListExtra("listEvents", array);
//			MySqlEvent[] tempArray = intent.GetParcelableArrayListExtra("listEvents").Cast<MySqlEvent>().ToArray();
//		}

//		public static List<MySqlEvent> GetListEventsFromPreferences() {
//			List<MySqlEvent> listEvents = new List<MySqlEvent>();
//			MySqlEvent[] array = MySqlEvent.intent.GetParcelableArrayListExtra("listEvents").Cast<MySqlEvent>().ToArray();
//			for(int i = 0; i < array.Length; i++) {
//				listEvents.Add(array[i]);
//			}
//			return listEvents;
//		}

		/** Converts the start and end date of an Event
		 *	If the the dates occur on the same day the output format will be dd.MM.yy HH:mm - HH:mm
		 *	else dd.MM.yy HH:mm - dd.MM.yy HH:mm
		 **/
		public string convertDateForLayout(MySqlEvent item) {
			if(item.startDate.Day == item.endDate.Day && item.startDate.Month == item.endDate.Month && item.startDate.Year == item.endDate.Year) {
				return item.startDate.ToString("dd.MM.yy HH:mm") + " - " + item.endDate.ToString("HH:mm");
			} else {
				return item.startDate.ToString("dd.MM.yy HH:mm") + " - " + item.endDate.ToString("dd.MM.yy HH:mm");
			}
		}

//		public static MySqlEvent getEventWithId(int id) {
//			foreach(MySqlEvent e in MySqlEvent.GetListEventsFromPreferences()) {
//				if(e.idEvent == id)
//					return e;
//			}
//			return null;
//		}

		public override string ToString() {
			return "[Event id=" + idEvent + ";name=" + name + ";startDate=" + startDate + ";endDate=" + endDate + ";location=" + location + ";state=" + state + "]";
		}

		#region ParcelableImplementation
//		public MySqlEvent(Parcel p) {
//			this.idEvent = p.ReadInt();
//			this.name = p.ReadString();
//			this.startDate = Convert.ToDateTime(p.ReadString());
//			this.endDate = Convert.ToDateTime(p.ReadString());
//			this.location = p.ReadString();
//			this.state = p.ReadString();
////			switch(p.ReadString()) {
////			case "G":
////				this.state = "Zugesagt";
////				break;
////			case "M":
////				this.state = "Vielleicht";
////				break;
////			case "D":
////				this.state = "Abgesagt";
////				break;
////			}
//		}
//		
//		public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags) {
//			dest.WriteInt(idEvent);
//			dest.WriteString(name);
//			dest.WriteString(startDate.ToString());
//			dest.WriteString(endDate.ToString());
//			dest.WriteString(location);
//			dest.WriteString(state);
//		}
//
//		public static readonly MyParcelableCreator<MySqlEvent> _creator 
//		= new MyParcelableCreator<MySqlEvent>((parcel) => new MySqlEvent(parcel));
//
//		[ExportField ("CREATOR")]
//		public static MyParcelableCreator<MySqlEvent> InitializeCreator() {
//			return _creator;
//		}
		#endregion
	}
}

