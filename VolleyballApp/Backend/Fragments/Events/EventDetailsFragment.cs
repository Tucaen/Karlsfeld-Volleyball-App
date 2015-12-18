
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


		VBUser user;
		public VBEvent _event { set; get;}
		public List<VBUser> listUser { get; set; }

		public EventDetailsFragment(VBEvent _event, List<VBUser> listUser) {
			this._event = _event;
			this.listUser = listUser;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			main = (MainActivity)this.Activity;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			user = VBUser.GetUserFromPreferences();

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
				if(DB_Communicator.getInstance().isAtLeast(user.getUserType(), UserType.Coremember)) {
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

			view.FindViewById<Button>(Resource.Id.btnEventZusagen).
			SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_ANSWER_INVITE, _event, "G"));

			view.FindViewById<Button>(Resource.Id.btnEventAbsagen).
			SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_ANSWER_INVITE, _event, "D"));

			#region toolbar
			btnInvite.SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_INVITE, _event));

			btnEdit.SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_EDIT, _event));

			btnDelete.SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_DELETE, _event));
			#endregion

			return view;
		}

		private bool isEditable(DateTime startDate, DateTime endDate) {
			return startDate >  DateTime.Today;
		}

		private void initalizeLinearLayout(LinearLayout listView, List<VBUser> list, string eventState, TextView textView, LayoutInflater inflater) {
			List<VBUser> filteredList = getUserWithEventState(list, eventState);
			List<VBUser> sortedList = filteredList.OrderBy(u => u.listTeamRole[0].position.Equals("Keine") || u.listTeamRole[0].position.Equals("")).
				ThenBy(u => u.listTeamRole[0].position.Equals("Steller")).
				ThenBy(u => u.listTeamRole[0].position.Equals("Mittelblocker")).
				ThenBy(u => u.listTeamRole[0].position.Equals("Libero")).
				ThenBy(u => u.listTeamRole[0].position.Equals("Diagonalangreifer")).
				ThenBy(u => u.listTeamRole[0].position.Equals("Außenangreifer")).
				ThenBy(u => u.name). 
				ToList();
				
			foreach(VBUser user in sortedList) {
				View row = inflater.Inflate(Resource.Layout.UserListView, null);
				row.FindViewById<TextView>(Resource.Id.UserListViewName).Text = user.name;
				if(user.listTeamRole[0].position != null && !user.listTeamRole[0].position.Equals("") && !user.listTeamRole[0].position.Equals("Keine"))
					row.FindViewById<TextView>(Resource.Id.UserListViewPosition).Text = "(" + user.listTeamRole[0].position + ")";
				else
					row.FindViewById<TextView>(Resource.Id.UserListViewPosition).Text = "";

				listView.AddView(row);
			}

			textView.Text = filteredList.Count.ToString();
		}

		private List<VBUser> getUserWithEventState(List<VBUser> list, string eventState) {
			List<VBUser> newList = new List<VBUser>();
			foreach(VBUser user in list) {
				if(user.eventState.Equals(eventState))
					newList.Add(user);
			}

			return newList;
		}

		private VBUser getLoggedInUser(int id, List<VBUser> listUser) {
			foreach(VBUser u in listUser) {
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

	class EventDetailsClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
		public const string ON_ANSWER_INVITE = "onAnswerInvite", ON_INVITE = "onInvite", ON_EDIT = "onEdit", ON_DELETE = "onDelete";
		private string source;
		private VBEvent _event;
		private string answer;

		public EventDetailsClickListener(string source, VBEvent _event) : this(source, _event, ""){}

		public EventDetailsClickListener(string source, VBEvent _event, string answer) {
			this.source = source;
			this._event = _event;
			this.answer = answer;
		}

		public void OnClick(View v) {
			switch(this.source) {
			case ON_ANSWER_INVITE:
				answerEventIvitation(answer);
				break;
			case ON_INVITE:
				onInvite();
				break;
			case ON_EDIT:
				onEdit();
				break;
			case ON_DELETE:
				onDelete();
				break;
			}
		}

		private async void answerEventIvitation(string state) {
			JsonValue json = await DB_Communicator.getInstance().updateEventState(_event.idEvent, state);

			if(!DB_Communicator.getInstance().wasSuccesful(json)) {
				Toast.MakeText(ViewController.getInstance().mainActivity, json["message"].ToString(), ToastLength.Long).Show();
			}

			//refresh the view
			await ViewController.getInstance().refreshDataForEvent(_event.idEvent);
			EventDetailsFragment edf = ViewController.getInstance().mainActivity.FindFragmentByTag(ViewController.EVENT_DETAILS_FRAGMENT) as EventDetailsFragment;
			List<VBUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(_event.idEvent, "");
			edf.listUser = listUser;
			ViewController.getInstance().refreshFragment(ViewController.EVENT_DETAILS_FRAGMENT);
		}

		private async void onInvite() {
			JsonValue json = await DB_Communicator.getInstance().SelectAllUser();

			InviteUserDialog iud = new InviteUserDialog(null, _event, DB_Communicator.getInstance().createUserFromResponse(json));

			iud.Show(ViewController.getInstance().mainActivity.FragmentManager, "INVITE_USER_DIALOG");
		}

		private void onEdit() {
			ViewController.getInstance().mainActivity.switchFragment(ViewController.EVENT_DETAILS_FRAGMENT, ViewController.EDIT_EVENT_FRAGMENT, new EditEventFragment(_event));
		}

		private void onDelete() {
			AlertDialog.Builder builder = new AlertDialog.Builder(ViewController.getInstance().mainActivity);
			builder.SetTitle("Event löschen!")
				.SetMessage("Sind sie sicher?")
				.SetIcon(Android.Resource.Drawable.IcDialogAlert)
				.SetNegativeButton("Ja", async (sender, e) => { //left button
					JsonValue json = await DB_Communicator.getInstance().deleteEvent(_event.idEvent);
					ViewController.getInstance().toastJson(ViewController.getInstance().mainActivity, json, ToastLength.Long, "Event delted");
					await ViewController.getInstance().refreshEvents();
					builder.Dispose();
					ViewController.getInstance().mainActivity.popBackstack();
				})
				.SetPositiveButton("Abbrechen", (sender, e) => { //right button
				})
				.Show();
		}
	}
}

