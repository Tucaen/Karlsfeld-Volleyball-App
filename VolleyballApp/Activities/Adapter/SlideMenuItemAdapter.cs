using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace VolleyballApp {
	public class SlideMenuItemAdapter : BaseAdapter<string> {
		string[] listItems;
		Activity context;

		public SlideMenuItemAdapter(Activity context, string[] listItems) : base() {
			this.context = context;
			this.listItems = listItems;
		}

		public override long GetItemId(int position) {
			return position;
		}
		public override string this[int position] {
			get { return listItems[position]; }
		}
		public override int Count {
			get { return listItems.Length; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent) {
			var item = listItems[position];
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.SlideMenuItems, null);
			view.FindViewById<TextView>(Resource.Id.slideMenuItem).Text = item;
			return view;
		}
	}
}

