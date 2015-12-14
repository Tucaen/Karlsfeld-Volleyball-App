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
	public class ListTeamsAdapter : BaseAdapter<VBTeam> {
		List<VBTeam> list;
		Fragment context;

		public ListTeamsAdapter(Fragment context, List<VBTeam> list) : base() {
			this.context = context;
			this.list = list;

			context.Activity.FindViewById<TextView>(Resource.Layout.TeamListView);
		}

		public override long GetItemId(int position) {
			return position;
		}
		public override VBTeam this[int position] {
			get { return list[position]; }
		}
		public override int Count {
			get { return list.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent) {
			var item = list[position];
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.Activity.LayoutInflater.Inflate(Resource.Layout.TeamListView, null);
			
			view.FindViewById<TextView>(Resource.Id.TitleText1).Text = item.name;

			return view;
		}
	}
}


