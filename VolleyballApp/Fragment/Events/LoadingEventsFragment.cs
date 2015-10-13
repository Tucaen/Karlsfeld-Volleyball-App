
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace VolleyballApp {
	public class LoadingEventsFragment : Fragment {
		public List<MySqlEvent> listEvents { get; }

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.LoadingEventsFragment, container, false);
			return view;
		}

		public override void OnStart() {
			MySqlUser user = MySqlUser.GetUserFromPreferences(this.Activity);
			listEvents = new List<MySqlEvent>();
			listEvents = DB_Communicator.getInstance().SelectEventsForUser(user.idUser, null).Result;

		}
	}
}

