using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.OneDrive.Pickers;

namespace VolleyballApp
{
	[Activity (Label = "VolleyballApp", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
		private IPicker picker;
		static string ONEDRIVE_APP_ID = "000000004015DC12";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//Create a picker
			picker = Picker.CreatePicker(ONEDRIVE_APP_ID);

			// Get our button from the layout resource,
			// and attach an event to it
			Button btnPicker = FindViewById<Button> (Resource.Id.OneDrivePicker);
			
			btnPicker.Click += delegate {
				picker.StartPicking(this, LinkType.DownloadLink);
			};
		}
	}
}


