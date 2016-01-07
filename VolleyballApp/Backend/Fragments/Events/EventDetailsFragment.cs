
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
		VBUser user;
		public VBEvent _event { set; get;}
		public List<VBUser> listUser { get; set; }
		public Button btnAccept, btnDenie;

		public EventDetailsFragment(VBEvent _event, List<VBUser> listUser) {
			this._event = _event;
			this.listUser = listUser;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			user = VBUser.GetUserFromPreferences();

			View view = inflater.Inflate(Resource.Layout.EventDetails, container, false);

			view.FindViewById<TextView>(Resource.Id.eventTitle).Text = _event.name;
			view.FindViewById<TextView>(Resource.Id.eventState).Text = "(" + getLoggedInUser(user.idUser, listUser).getEventState() + ")";
			view.FindViewById<TextView>(Resource.Id.eventLocation).Text = _event.location;
			view.FindViewById<TextView>(Resource.Id.eventTime).Text = _event.convertDateForLayout(_event);

			if(_event.description != null && !_event.description.Equals("")) {
				view.FindViewById<LinearLayout>(Resource.Id.eventDetailsEventDescriptionLine).Visibility = ViewStates.Visible;
				view.FindViewById<TextView>(Resource.Id.eventDetailsEventDescriptionValue).Text = _event.description.Replace("\\n", "\n");
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

			#region buttons
			btnAccept = view.FindViewById<Button>(Resource.Id.btnEventZusagen);
			btnAccept.SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_ANSWER_INVITE, this, "G"));

			btnDenie = view.FindViewById<Button>(Resource.Id.btnEventAbsagen);
			btnDenie.SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_ANSWER_INVITE, this, "D"));

			if(getEventStateOfLoggedInUser().Equals(VBUser.ACCEPTED)) {
				btnAccept.Enabled = false;
				btnDenie.Enabled = true;
			}
			if(getEventStateOfLoggedInUser().Equals(VBUser.DENIED)) {
				btnAccept.Enabled = true;
				btnDenie.Enabled = false;
			}
			#endregion

			#region toolbar
			btnInvite.SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_INVITE, this));

			btnEdit.SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_EDIT, this));

			btnDelete.SetOnClickListener(new EventDetailsClickListener(EventDetailsClickListener.ON_DELETE, this));
			#endregion

			return view;
		}

		private bool isEditable(DateTime startDate, DateTime endDate) {
			return startDate >  DateTime.Today;
		}

		private void initalizeLinearLayout(LinearLayout listView, List<VBUser> list, string eventState, TextView textView, LayoutInflater inflater) {
			List<VBUser> filteredList = getUserWithEventState(list, eventState);

			List<VBUser> sortedList = ViewController.getInstance().sortUserlistForTeam(filteredList, _event.teamId);
				
			foreach(VBUser user in sortedList) {
				View row = inflater.Inflate(Resource.Layout.UserListView, null);
				row.FindViewById<TextView>(Resource.Id.UserListViewName).Text = user.name;
				VBTeamrole teamrole = user.getTeamroleForTeam(_event.teamId);
				if(teamrole.position != null && !teamrole.position.Equals("") && !teamrole.position.Equals("Keine"))
					row.FindViewById<TextView>(Resource.Id.UserListViewPosition).Text = "(" + teamrole.position + ")";
				else
					row.FindViewById<TextView>(Resource.Id.UserListViewPosition).Text = "";

				listView.AddView(row);
			}

			textView.Text = filteredList.Count.ToString();
		}

		private List<VBUser> getUserWithEventState(List<VBUser> list, string eventState) {
			List<VBUser> newList = new List<VBUser>();
			foreach(VBUser user in list) {
				if(user.getEventState().Equals(eventState))
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

		private string getEventStateOfLoggedInUser() {
			foreach(VBUser u in listUser) {
				if(u.idUser == user.idUser)
					return u.getEventState();
			}
			return "";
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
		private EventDetailsFragment edf;

		public EventDetailsClickListener(string source, EventDetailsFragment edf) : this(source, edf, ""){}

		public EventDetailsClickListener(string source, EventDetailsFragment edf, string answer) {
			this.source = source;
			this.edf = edf;
			this._event = edf._event;
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

		private async void answerEventIvitation(string answer) {
			JsonValue json = await DB_Communicator.getInstance().updateEventState(_event.idEvent, answer);

			//Display error message, if there was a problem with the DB
			if(!DB_Communicator.getInstance().wasSuccesful(json)) {
				Toast.MakeText(ViewController.getInstance().mainActivity, json["message"].ToString(), ToastLength.Long).Show();
			} else {
				if(answer.Equals("G")) {
					this.edf.btnAccept.Enabled = false;
					this.edf.btnDenie.Enabled = true;
				}
				if(answer.Equals("D")) {
					this.edf.btnAccept.Enabled = true;
					this.edf.btnDenie.Enabled = false;
				}
			}

			//refresh the view
			await ViewController.getInstance().refreshDataForEvent(_event.idEvent);
			EventDetailsFragment edf = ViewController.getInstance().mainActivity.FindFragmentByTag(ViewController.EVENT_DETAILS_FRAGMENT) as EventDetailsFragment;
			List<VBUser> listUser = await DB_Communicator.getInstance().SelectUserForEvent(_event.idEvent, "");
			edf.listUser = listUser;
			ViewController.getInstance().refreshFragment(ViewController.EVENT_DETAILS_FRAGMENT);
		}

		private async void onInvite() {
			JsonValue json = await DB_Communicator.getInstance().loadUninvtedUser(_event.idEvent);

			InviteUserDialog iud = new InviteUserDialog(null, _event, DB_Communicator.getInstance().createUserFromResponse(json));

			iud.Show(ViewController.getInstance().mainActivity.FragmentManager, "INVITE_USER_DIALOG");
		}

		private async void onEdit() {
			List<VBTeam> listTeams = await DB_Communicator.getInstance().SelectTeams();
			ViewController.getInstance().mainActivity.switchFragment(ViewController.EVENT_DETAILS_FRAGMENT, ViewController.EDIT_EVENT_FRAGMENT, new EditEventFragment(_event, listTeams));
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

