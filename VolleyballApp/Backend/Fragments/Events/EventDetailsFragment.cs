
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
		public MySqlEvent _event { set; get;}

		public EventDetailsFragment(MySqlEvent _event) {
			this._event = _event;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			main = (MainActivity)this.Activity;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			List<MySqlUser> listUser = MySqlUser.GetListUserFromPreferences();
			user = MySqlUser.GetUserFromPreferences(this.Activity);

			View view = inflater.Inflate(Resource.Layout.EventDetails, container, false);

			view.FindViewById<TextView>(Resource.Id.eventTitle).Text = _event.name;
			view.FindViewById<TextView>(Resource.Id.eventState).Text = "(" + getLoggedInUser(user.idUser, listUser).eventState + ")";
			view.FindViewById<TextView>(Resource.Id.eventLocation).Text = _event.location;
			view.FindViewById<TextView>(Resource.Id.eventTime).Text = _event.convertDateForLayout(_event);

			//zusagen
			initalizeListView(view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser_Zugesagt), listUser,
				DB_Communicator.State.Accepted, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Zugesagt));

			//absagen
			initalizeListView(view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser_Abgesagt), listUser,
				DB_Communicator.State.Denied, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Abgesagt));

			//eingeladen
			initalizeListView(view.FindViewById<ListView>(Resource.Id.EventDetails_ListUser_Eingeladen), listUser,
				DB_Communicator.State.Invited, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Eingeladen));

			view.FindViewById<Button>(Resource.Id.btnEventZusagen).Click += delegate {
				this.answerEventIvitation("G");
			};

			view.FindViewById<Button>(Resource.Id.btnEventAbsagen).Click += delegate {
				this.answerEventIvitation("D");
			};

			#region toolbar
			ImageView btnInvite = this.Activity.FindViewById<ImageView>(Resource.Id.btnAddInToolbar);
			btnInvite.Visibility = ViewStates.Visible;
			btnInvite.Click += async delegate {
				try {
					JsonValue json = await DB_Communicator.getInstance().SelectAllUser();
					MySqlUser.StoreUserListInPreferences(this.Activity.Intent, DB_Communicator.getInstance().createUserFromResponse(json));
					
					InviteUserDialog iud = new InviteUserDialog(savedInstanceState, _event);
					iud.Show(FragmentManager, "INVITE_USER_DIALOG");
				} catch (Exception e) {
					Console.WriteLine("ERROR OPENING INVITE_USER-DIALOG: " + e.Source);
				}
			};

			ImageView btnEdit = this.Activity.FindViewById<ImageView>(Resource.Id.btnEditInToolbar);
			btnEdit.Visibility = ViewStates.Visible;
			btnEdit.Click += delegate {
				main.switchFragment(MainActivity.EVENT_DETAILS_FRAGMENT, MainActivity.EDIT_EVENT_FRAGMENT, new EditEventFragment(_event));
			};

			ImageView btnDelete = this.Activity.FindViewById<ImageView>(Resource.Id.btnDeleteInToolbar);
			btnDelete.Visibility = ViewStates.Visible;
			btnDelete.Click += delegate {
//				main.switchFragment(MainActivity.EVENT_DETAILS_FRAGMENT, MainActivity.EDIT_EVENT_FRAGMENT, new EditEventFragment(_event));
				AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);
				builder.SetTitle("Advent löschen!")
					.SetMessage("Sind sie sicher?")
					.SetIcon(Android.Resource.Drawable.IcDialogAlert)
					.SetNegativeButton("Muh", async (sender, e) => { //left button
						JsonValue json = await DB_Communicator.getInstance().deleteEvent(_event.idEvent);
						Toast.MakeText(this.Activity, json["message"].ToString(), ToastLength.Long);
						await main.refreshEvents();
						FragmentManager.PopBackStackImmediate();
					})
					.SetPositiveButton("Mäh", (sender, e) => { //right button
					})
					.Show();
			};
			#endregion

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
			await main.refreshDataForEvent(_event.idEvent);
			main.refreshFragment(MainActivity.EVENT_DETAILS_FRAGMENT);
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
			this.Activity.FindViewById<ImageView>(Resource.Id.btnAddInToolbar).Visibility = ViewStates.Gone;
			this.Activity.FindViewById<ImageView>(Resource.Id.btnEditInToolbar).Visibility = ViewStates.Gone;
			this.Activity.FindViewById<ImageView>(Resource.Id.btnDeleteInToolbar).Visibility = ViewStates.Gone;
		}
	}
}

