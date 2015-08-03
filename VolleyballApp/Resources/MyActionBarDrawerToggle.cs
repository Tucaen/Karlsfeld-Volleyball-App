using System;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V7.App;
using Android.Support.V4.Widget;


namespace VolleyballApp {
	public class MyActionBarDrawerToggle : SupportActionBarDrawerToggle {
		private ActionBarActivity mHostActivity;
		private int mOpenedResource;
		private int mCLosedResource;


		public MyActionBarDrawerToggle(ActionBarActivity host, DrawerLayout drawerLayout, int openedResource, int closedResource)
			: base(host, drawerLayout, openedResource, closedResource) {
			mHostActivity = host;
			mOpenedResource = openedResource;
			mCLosedResource = closedResource;
		}

		public override void OnDrawerOpened(Android.Views.View drawerView) {
			base.OnDrawerOpened(drawerView);
		}

		public override void OnDrawerClosed(Android.Views.View drawerView) {
			base.OnDrawerClosed(drawerView);
		}

		public override void OnDrawerSlide(Android.Views.View drawerView, float slideOffset) {
			base.OnDrawerSlide(drawerView, slideOffset);
		}
	}
}

