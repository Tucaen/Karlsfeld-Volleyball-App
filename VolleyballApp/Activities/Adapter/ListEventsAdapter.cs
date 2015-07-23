using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace VolleyballApp
{
	public class ListEventsAdapter : BaseAdapter<MySqlEvent> {
		List<MySqlEvent> listEvents;
		Activity context;

		public ListEventsAdapter(Activity context, List<MySqlEvent> listEvents) : base() {
			this.context = context;
			this.listEvents = listEvents;

//			Console.WriteLine("Event[0] = " + listEvents[0].idEvent + " " + listEvents[0].name + " " + listEvents[0].startDate + " " + listEvents[0].endDate + " " + listEvents[0].endDate);
			context.FindViewById<TextView>(Resource.Layout.EventListView);
		}

		public override long GetItemId(int position) {
			return position;
		}
		public override MySqlEvent this[int position] {
			get { return listEvents[position]; }
		}
		public override int Count {
			get { return listEvents.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent) {
			var item = listEvents[position];
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.EventListView, null);
			view.FindViewById<TextView>(Resource.Id.TitleText1).Text = item.name;
			view.FindViewById<TextView>(Resource.Id.TitleText2).Text = "(" + item.state + ")";
			view.FindViewById<TextView>(Resource.Id.Date).Text = MySqlEvent.convertDateToString(item.startDate, item.endDate);
			return view;
		}
	}
}


