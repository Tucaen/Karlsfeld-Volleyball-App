
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
using System.Json;

namespace VolleyballApp {
	[Activity(Label = "FillDataActivity")]			
	public class FillDataActivity : AbstractActivity {
		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.FillData);

			FindViewById<Button>(Resource.Id.btnFillDataOk).Click += async (object sender, EventArgs e) => {
				//update user
				EditText name = FindViewById<EditText>(Resource.Id.fillDataNameData);
				EditText firstname = FindViewById<EditText>(Resource.Id.fillDataFirstnameData);
				JsonValue json = await DB_Communicator.getInstance().UpdateUser(firstname.Text + " " + name.Text);

				Toast.MakeText(this, json["message"].ToString(), ToastLength.Long).Show();

				Intent i = new Intent(this, typeof(MainActivity));	
				await base.loadAndSaveEvents(MySqlUser.GetUserFromPreferences(this), null);
				StartActivity(i);
			};

		}
	}
}

