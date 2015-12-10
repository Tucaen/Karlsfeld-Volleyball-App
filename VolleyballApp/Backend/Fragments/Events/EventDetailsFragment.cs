
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
using Java.Lang;

namespace VolleyballApp {
	public class EventDetailsFragment : Fragment {
		MainActivity main;
		InviteUserDialog iud;

		MySqlUser user;
		public MySqlEvent _event { set; get;}
		private List<MySqlUser> listUser;

		public EventDetailsFragment(MySqlEvent _event, List<MySqlUser> listUser) {
			this._event = _event;
			this.listUser = listUser;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			main = (MainActivity)this.Activity;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
//			List<MySqlUser> listUser = MySqlUser.GetListUserFromPreferences();
			user = MySqlUser.GetUserFromPreferences();

			View view = inflater.Inflate(Resource.Layout.EventDetails, container, false);

			view.FindViewById<TextView>(Resource.Id.eventTitle).Text = _event.name;
			view.FindViewById<TextView>(Resource.Id.eventState).Text = "(" + getLoggedInUser(user.idUser, listUser).eventState + ")";
			view.FindViewById<TextView>(Resource.Id.eventLocation).Text = _event.location;
			view.FindViewById<TextView>(Resource.Id.eventTime).Text = _event.convertDateForLayout(_event);

			if(_event.description != null && !_event.description.Equals("")) {
				view.FindViewById<LinearLayout>(Resource.Id.eventDetailsEventDescriptionLine).Visibility = ViewStates.Visible;
				view.FindViewById<TextView>(Resource.Id.eventDetailsEventDescriptionValue).Text = _event.description;
			}

			ImageView btnInvite = this.Activity.FindViewById<ImageView>(Resource.Id.btnAddInToolbar);
			ImageView btnEdit = this.Activity.FindViewById<ImageView>(Resource.Id.btnEditInToolbar);
			ImageView btnDelete = this.Activity.FindViewById<ImageView>(Resource.Id.btnDeleteInToolbar);

			if(!this.isEditable(_event.startDate, _event.endDate)) {
				btnInvite.Visibility = ViewStates.Gone;
				btnEdit.Visibility = ViewStates.Gone;
				btnDelete.Visibility = ViewStates.Gone;

				view.FindViewById<Button>(Resource.Id.btnEventAbsagen).Visibility = ViewStates.Gone;
				view.FindViewById<Button>(Resource.Id.btnEventZusagen).Visibility = ViewStates.Gone;
			} else {
				if(DB_Communicator.getInstance().isAtLeast(user, UserType.Coremember)) {
					btnInvite.Visibility = ViewStates.Visible;
					btnEdit.Visibility = ViewStates.Visible;
					btnDelete.Visibility = ViewStates.Visible;
				}
			}

			//zusagen
			initalizeLinearLayout(view.FindViewById<LinearLayout>(Resource.Id.EventDetails_ListUser_Zugesagt), listUser,
				DB_Communicator.State.Accepted, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Zugesagt), inflater);
			//absagen
			initalizeLinearLayout(view.FindViewById<LinearLayout>(Resource.Id.EventDetails_ListUser_Abgesagt), listUser,
				DB_Communicator.State.Denied, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Abgesagt), inflater);
			//eingeladen
			initalizeLinearLayout(view.FindViewById<LinearLayout>(Resource.Id.EventDetails_ListUser_Eingeladen), listUser,
				DB_Communicator.State.Invited, view.FindViewById<TextView>(Resource.Id.EventDetails_Count_Eingeladen), inflater);

			view.FindViewById<Button>(Resource.Id.btnEventZusagen).Click += delegate {
				this.answerEventIvitation("G");
			};

			view.FindViewById<Button>(Resource.Id.btnEventAbsagen).Click += delegate {
				this.answerEventIvitation("D");
			};

			#region toolbar
//			btnInvite.Touch += (object sender, View.TouchEventArgs e) => {
//				if(e.Event.Action == MotionEventActions.Down) {
//					Console.WriteLine("Down");
//				}
//
//				if(e.Event.Action == MotionEventActions.Up) {
//					Console.WriteLine("Up");
//					onInvite();
//				}
//			};
			btnInvite.Click += (object sender, EventArgs e) => {
				Console.WriteLine("Click_Invite");
				onInvite();
			};

			btnEdit.Click += delegate {
				main.switchFragment(ViewController.EVENT_DETAILS_FRAGMENT, ViewController.EDIT_EVENT_FRAGMENT, new EditEventFragment(_event));
			};

			btnDelete.Click += (object sender, EventArgs e) => {
				Console.WriteLine("Click_Delte");
				onDelete();
			};
			#endregion

			return view;
		}

		private async void onInvite() {
			JsonValue json = await DB_Communicator.getInstance().SelectAllUser();

			if(iud == null)
				iud = new InviteUserDialog(null, _event, DB_Communicator.getInstance().createUserFromResponse(json));

			if(iud.isShown) 
				iud.Dismiss();

			iud.Show(main.FragmentManager, "INVITE_USER_DIALOG");
		}

		public void onDelete() {
			AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);
			builder.SetTitle("Event löschen!")
				.SetMessage("Sind sie sicher?")
				.SetIcon(Android.Resource.Drawable.IcDialogAlert)
				.SetNegativeButton("Ja", async (sender, e) => { //left button
					JsonValue json = await DB_Communicator.getInstance().deleteEvent(_event.idEvent);
					ViewController.getInstance().toastJson(main, json, ToastLength.Long, "Event delted");
					await ViewController.getInstance().refreshEvents();
					builder.Dispose();
					main.popBackstack();
				})
				.SetPositiveButton("Abbrechen", (sender, e) => { //right button
				})
				.Show();
		}

		private bool isEditable(DateTime startDate, DateTime endDate) {
			return startDate >  DateTime.Today;
		}

		private void initalizeLinearLayout(LinearLayout listView, List<MySqlUser> list, string eventState, TextView textView, LayoutInflater inflater) {
			List<MySqlUser> filteredList = getUserWithEventState(list, eventState);
			List<MySqlUser> sortedList = filteredList.OrderBy(u => u.teamRole.position.Equals("Keine") || u.teamRole.position.Equals("")).
				ThenBy(u => u.teamRole.position.Equals("Steller")).
				ThenBy(u => u.teamRole.position.Equals("Mittelblocker")).
				ThenBy(u => u.teamRole.position.Equals("Libero")).
				ThenBy(u => u.teamRole.position.Equals("Diagonalangreifer")).
				ThenBy(u => u.teamRole.position.Equals("Außenangreifer")).
				ThenBy(u => u.name). 
				ToList();
				
			foreach(MySqlUser user in sortedList) {
				View row = inflater.Inflate(Resource.Layout.UserListView, null);
				row.FindViewById<TextView>(Resource.Id.UserListViewName).Text = user.name;
				if(user.teamRole.position != null && !user.teamRole.position.Equals("") && !user.teamRole.position.Equals("Keine"))
					row.FindViewById<TextView>(Resource.Id.UserListViewPosition).Text = "(" + user.teamRole.position + ")";
				else
					row.FindViewById<TextView>(Resource.Id.UserListViewPosition).Text = "";

				listView.AddView(row);
			}

			textView.Text = filteredList.Count.ToString();
		}

		private void initalizeListView(ListView listView, List<MySqlUser> list, string eventState, TextView textView) {
			List<MySqlUser> filteredList = getUserWithEventState(list, eventState);
			List<MySqlUser> sortedList = filteredList.OrderBy(u => u.teamRole.position.Equals("Keine") || u.teamRole.position.Equals("")).
									ThenBy(u => u.teamRole.position.Equals("Steller")).
									ThenBy(u => u.teamRole.position.Equals("Mittelblocker")).
									ThenBy(u => u.teamRole.position.Equals("Libero")).
									ThenBy(u => u.teamRole.position.Equals("Diagonalangreifer")).
									ThenBy(u => u.teamRole.position.Equals("Außenangreifer")).
									ThenBy(u => u.name). 
									ToList();
			listView.Adapter = new ListUserAdapter(this, sortedList);
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
			await ViewController.getInstance().refreshDataForEvent(_event.idEvent);
			ViewController.getInstance().refreshFragment(ViewController.EVENT_DETAILS_FRAGMENT);
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

