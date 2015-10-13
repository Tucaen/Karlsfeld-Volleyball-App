
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace VolleyballApp {
	[Activity(Label = "MainActivity")]			
	public class MainActivity : Activity {
		FragmentTransaction trans;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

			trans = FragmentManager.BeginTransaction();
			trans.Add(Resource.Id.fragmentContainer, new EventsFragment(), "EventsFragment");
			trans.Commit();

		}
	}
}

