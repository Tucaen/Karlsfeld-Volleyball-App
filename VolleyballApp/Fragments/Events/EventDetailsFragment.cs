
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
	public class EventDetailsFragment : Fragment {
		ListView listView;
		List<MySqlUser> listUser;
		MySqlUser user;
		MySqlEvent _event;
		int position;

		public EventDetailsFragment(int position) {
			this.position = position;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			listUser = MySqlUser.GetListUserFromPreferences();
			user = MySqlUser.GetUserFromPreferences(this.Activity);
			_event = MySqlEvent.GetListEventsFromPreferences()[position];

			View view = inflater.Inflate(Resource.Layout.EventDetails, container, false);

			view.FindViewById<TextView>(Resource.Id.eventTitle).Text = _event.name;
			view.FindViewById<TextView>(Resource.Id.eventState).Text = "(" + getLoggedInUser(user.idUser).eventState + ")";
			view.FindViewById<TextView>(Resource.Id.eventLocation).Text = _event.location;
			view.FindViewById<TextView>(Resource.Id.eventTime).Text = _event.convertDateForLayout(_event);

			listView = view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser);
			listView.Adapter = new ListUserAdapter(this, listUser);

			view.FindViewById<Button>(Resource.Id.btnEventZusagen).Click += delegate {
				this.answerEventIvitation("G");
			};

			view.FindViewById<Button>(Resource.Id.btnEventAbsagen).Click += delegate {
				this.answerEventIvitation("D");
			};

			return view;
		}
			

		private async void answerEventIvitation(string state) {
			MainActivity main = (MainActivity)this.Activity;
			JsonValue json = await DB_Communicator.getInstance().updateEventState(_event.idEvent, state);

			if(!DB_Communicator.getInstance().wasSuccesful(json)) {
				Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long).Show();
			}

			//refresh the view
			main.refreshEventDetailsFragment(MainActivity.EVENT_DETAILS_FRAGMENT, this.position);

		}

		private MySqlUser getLoggedInUser(int id) {
			foreach(MySqlUser u in listUser) {
				if(u.idUser == id)
					return u;
			}
			return null;
		}
	}
}

