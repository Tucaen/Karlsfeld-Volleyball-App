using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using VolleyballApp;

namespace VolleyballAp {
	
	[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
	public class MyGcmListenerService : GcmListenerService {
		
		public override void OnMessageReceived (string from, Bundle data) {
			var message = data.GetString ("message");
			Log.Info ("MyGcmListenerService", "From:    " + from);
			Log.Info ("MyGcmListenerService", "Message: " + message);
			SendNotification (data);
		}

		void SendNotification (Bundle data) {
			var intent = new Intent (this, typeof(MainActivity));
			intent.AddFlags (ActivityFlags.ClearTop);
			var pendingIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.OneShot);

			var notificationBuilder = new Notification.Builder(this)
				.SetSmallIcon (Resource.Drawable.pushnotification_icon)
				.SetContentTitle (data.GetString("title"))
				.SetContentText (data.GetString ("message"))
				.SetAutoCancel (true)
				.SetContentIntent (pendingIntent);

			var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			notificationManager.Notify (0, notificationBuilder.Build());
		}
	}
}

