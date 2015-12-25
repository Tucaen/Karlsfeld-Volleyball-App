using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace VolleyballApp {
	class SpinnerTeamAdapter : ArrayAdapter<VBTeam> {
		private Fragment context;
		private List<VBTeam> list;

		public SpinnerTeamAdapter(Fragment context, int textViewResourceId,List<VBTeam> itemList) : 
		base(context.Activity, textViewResourceId, itemList.ToArray()) {
			this.context = context;
			this.list = itemList;
		}

		public override long GetItemId(int position) {
			return position;
		}
//		public override VBTeam this[int position] {
//			get { return list[position]; }
//		}
		public override int Count {
			get { return list.Count; }
		}
//
		public override View GetView(int position, View convertView, ViewGroup parent) {
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.Activity.LayoutInflater.Inflate(Resource.Layout.SpinnerTextView, null);

			TextView v = view.FindViewById<TextView>(Resource.Id.spinnerTextViewName);
			v.Text = list[position].name;
			return v;
		}

		public override View GetDropDownView(int position, View convertView, ViewGroup parent) {
			View view = convertView;

			if (view == null) // no view to re-use, create new
				view = context.Activity.LayoutInflater.Inflate(Resource.Layout.SpinnerCheckedLayout, null);

			TextView v = view.FindViewById<TextView>(Resource.Id.spinnerDropDownTextViewName);
			v.Text = list[position].name;
			return v;
		}
	}
}

