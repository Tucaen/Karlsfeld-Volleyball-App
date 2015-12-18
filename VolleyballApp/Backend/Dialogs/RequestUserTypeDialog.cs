
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

	class RequestUserTypeDialog : DialogFragment {
		public int userId { get; set; }
		public int teamId { get; set; }

		public RequestUserTypeDialog (int userId, int teamId) {
			this.userId = userId;
			this.teamId = teamId;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.RequestUserType, container, false);

			this.Dialog.SetTitle("Status beantragen");

//			RadioGroup rdg = view.FindViewById<RadioGroup>(Resource.Id.requestUserTypeRadioGroup);

			Button btnSendRequest = view.FindViewById<Button>(Resource.Id.requestUserTypeBtnSendRequest);
			btnSendRequest.SetOnClickListener(new RequestUTClickListener(RequestUTClickListener.ON_SEND_REQUEST, view, this));

			return view;
		}
	}

	class  RequestUTClickListener : Java.Lang.Object, Android.Views.View.IOnClickListener {
		public const string ON_SEND_REQUEST="onSendRequest";
		private string source;
		private RequestUserTypeDialog d;
//		private int userId, teamId;
		private View view;

		public RequestUTClickListener(string source, View view, RequestUserTypeDialog d) {
			this.source = source;
			this.view = view;
			this.d = d;
//			this.userId = userId;
//			this.teamId = teamId;
		}

		public void OnClick(View view) {
			switch(this.source) {
			case ON_SEND_REQUEST:
				this.onSendRequest();
				break;
			}
		}

		private async void onSendRequest() {
//			Toast.MakeText(ViewController.getInstance().mainActivity, "Not implemented, yet!", ToastLength.Long).Show();
			string response = await DB_Communicator.getInstance().createUserTypeRequest(d.userId, d.teamId, getRequest());
			ViewController.getInstance().toastJson(null, JsonValue.Parse(response), ToastLength.Long, "Request was send");
			d.Dismiss();
		}

		private string getRequest() {
			RadioGroup rdg = view.FindViewById<RadioGroup>(Resource.Id.requestUserTypeRadioGroup);

			if(rdg.CheckedRadioButtonId != -1) {
				RadioButton checkedBtn = view.FindViewById<RadioButton>(rdg.CheckedRadioButtonId);
				string selection = checkedBtn.Text;
				return selection.Substring(0, 1);
			}

			return "";
		}
	}
}
