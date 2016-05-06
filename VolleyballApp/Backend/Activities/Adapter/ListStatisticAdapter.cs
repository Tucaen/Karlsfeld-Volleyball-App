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
	public class ListStatisticAdapter : BaseAdapter<string> {
		List<string> list;
		Fragment context;

		public ListStatisticAdapter(Fragment context, List<string> list) : base() {
			this.context = context;
			this.list = list;

			context.Activity.FindViewById<TextView>(Resource.Layout.StatisticListView);
		}

		public override long GetItemId(int position) {
			return position;
		}
		public override string this[int position] {
			get { return list[position]; }
		}
		public override int Count {
			get { return list.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent) {
			var item = list[position];
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.Activity.LayoutInflater.Inflate(Resource.Layout.StatisticListView, null);

			view.FindViewById<TextView>(Resource.Id.statisticDetailsSaison).Text = item;

			return view;
		}
	}
}


