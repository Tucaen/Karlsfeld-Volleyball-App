
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
		MainActivity main;

		MySqlUser user;
		MySqlEvent _event;
		int position;

		public EventDetailsFragment(int position, MySqlEvent _event) {
			this.position = position;
			this._event = _event;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			main = (MainActivity)this.Activity;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			List<MySqlUser> listUser = MySqlUser.GetListUserFromPreferences();
			user = MySqlUser.GetUserFromPreferences(this.Activity);
//			_event = MySqlEvent.GetListEventsFromPreferences()[position];

			View view = inflater.Inflate(Resource.Layout.EventDetails, container, false);

			view.FindViewById<TextView>(Resource.Id.eventTitle).Text = _event.name;
			view.FindViewById<TextView>(Resource.Id.eventState).Text = "(" + getLoggedInUser(user.idUser, listUser).eventState + ")";
			view.FindViewById<TextView>(Resource.Id.eventLocation).Text = _event.location;
			view.FindViewById<TextView>(Resource.Id.eventTime).Text = _event.convertDateForLayout(_event);

			#region zusagen
			initalizeListView(view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser_Zugesagt), listUser,
				DB_Communicator.State.Accepted, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Zugesagt));
			#endregion

			#region abgesagt
			initalizeListView(view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser_Abgesagt), listUser,
				DB_Communicator.State.Denied, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Abgesagt));
			#endregion

			#region eingeladen
			initalizeListView(view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser_Eingeladen), listUser,
				DB_Communicator.State.Invited, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Eingeladen));
			#endregion



			view.FindViewById<Button>(Resource.Id.btnEventZusagen).Click += delegate {
				this.answerEventIvitation("G");
			};

			view.FindViewById<Button>(Resource.Id.btnEventAbsagen).Click += delegate {
				this.answerEventIvitation("D");
			};

			TextView btnInvite = this.Activity.FindViewById<TextView>(Resource.Id.btnAddInToolbar);
			btnInvite.Visibility = ViewStates.Visible;
			btnInvite.Click += async delegate {
				try {
					Console.WriteLine("btnAddInToolbar was pressed!");
					JsonValue json = await DB_Communicator.getInstance().SelectAllUser();
					MySqlUser.StoreUserListInPreferences(this.Activity.Intent, DB_Communicator.getInstance().createUserFromResponse(json));
					
					InviteUserDialog iud = new InviteUserDialog(savedInstanceState, _event, position);
					iud.Show(FragmentManager, "INVITE_USER_DIALOG");
				} catch (Exception e) {
					Console.WriteLine("ERROR OPENING INVITE_USER-DIALOG: " + e.Source);
				}
			};

			return view;
		}

		private void initalizeListView(ListView listView, List<MySqlUser> list, string eventState, TextView textView) {
			List<MySqlUser> filteredList = getUserWithEventState(list, eventState);
			listView.Adapter = new ListUserAdapter(this, filteredList);
			textView.Text = filteredList.Count.ToString();
		}

		private List<MySqlUser> getUserWithEventState(List<MySqlUser> list, string eventState) {
			List<MySqlUser> newList = new List<MySqlUser>();
			foreach(MySqlUser user in list) {
				if(user.eventState.Equals(eventState))
					newList.Add(user);
			}

			return newList;
		}

		private async void answerEventIvitation(string state) {
			JsonValue json = await DB_Communicator.getInstance().updateEventState(_event.idEvent, state);

			if(!DB_Communicator.getInstance().wasSuccesful(json)) {
				Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long).Show();
			}

			//refresh the view
			main.refreshEventDetailsFragment(MainActivity.EVENT_DETAILS_FRAGMENT, this.position);
		}

		private MySqlUser getLoggedInUser(int id, List<MySqlUser> listUser) {
			foreach(MySqlUser u in listUser) {
				if(u.idUser == id)
					return u;
			}
			return null;
		}

		public override void OnDestroyView() {
			base.OnDestroyView();
			this.Activity.FindViewById<TextView>(Resource.Id.btnAddInToolbar).Visibility = ViewStates.Gone;
		}
	}
}

