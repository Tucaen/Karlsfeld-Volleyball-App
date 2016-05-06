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
	public class ListEventsAdapter : BaseAdapter<VBEvent> {
		List<VBEvent> listEvents;
		Fragment context;

		public ListEventsAdapter(Fragment context, List<VBEvent> listEvents) : base() {
			this.context = context;
			this.listEvents = listEvents;

			context.Activity.FindViewById<TextView>(Resource.Layout.EventListView);
		}

		public override long GetItemId(int position) {
			return position;
		}
		public override VBEvent this[int position] {
			get { return listEvents[position]; }
		}
		public override int Count {
			get { return listEvents.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent) {
			var item = listEvents[position];
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.Activity.LayoutInflater.Inflate(Resource.Layout.EventListView, null);

			LinearLayout header = view.FindViewById<LinearLayout>(Resource.Id.eventListHeader);
			if(item.name.Length >= 20)
				header.Orientation = Orientation.Vertical;
				
			view.FindViewById<TextView>(Resource.Id.TitleText1).Text = item.name;
			view.FindViewById<TextView>(Resource.Id.TitleText2).Text = "(" + item.state + ")";

			MainActivity main = (MainActivity)this.context.Activity;
			view.FindViewById<TextView>(Resource.Id.Date).Text = item.convertDateForLayout(item);

			return view;
		}
	}
}


