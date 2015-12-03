using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using VolleyballApp;
using System;

namespace VolleyballAp {
	
	[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
	public class MyGcmListenerService : GcmListenerService {
		public const string PUSH_INVITE = "PushInvite", PUSH_DELETE = "PushDelete", 
							PUSH_EVENT_UPDATE="PushEventUpdate", PUSH_REGISTRATION_VALIDATION = "RegistrationValidation";
		private int notificationId = 0;

		public override void OnMessageReceived (string from, Bundle data) {
			var message = data.GetString ("message");
			var type = data.GetString("type");
			Log.Info ("MyGcmListenerService", "From:    " + from);
			Log.Info ("MyGcmListenerService", "Message: " + message);
			Log.Info ("MyGcmListenerService", "Type:    " + type);

			if(!type.Equals(PUSH_REGISTRATION_VALIDATION))
				SendNotification (data);
		}

		private void SendNotification (Bundle data) {
			Intent intent;
			string type = data.GetString("type");

			intent = new Intent (this, typeof(MainActivity));
			if(type.Equals(PUSH_INVITE)) {
				ViewController.getInstance().pushEventId = Convert.ToInt32(data.GetString("eventId"));
				intent.SetAction(PUSH_INVITE);
			} else if(type.Equals(PUSH_EVENT_UPDATE)) {
				ViewController.getInstance().pushEventId = Convert.ToInt32(data.GetString("eventId"));
				intent.SetAction(PUSH_EVENT_UPDATE);
			} else if(type.Equals(PUSH_DELETE)) {
				//nothing else need to be done
				//MainActivity will refresh the events on start
			}

			intent.AddFlags (ActivityFlags.ClearTop);
			var pendingIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.OneShot);

			var notificationBuilder = new Notification.Builder(this)
				.SetSmallIcon (Resource.Drawable.pushnotification_icon)
				.SetContentTitle (data.GetString("title"))
				.SetContentText (data.GetString ("message"))
				.SetVibrate(new long[] {600, 600})
				.SetAutoCancel (true)
				.SetContentIntent (pendingIntent);

			var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			notificationManager.Notify (notificationId, notificationBuilder.Build());
			notificationId++;
		}
	}
}

