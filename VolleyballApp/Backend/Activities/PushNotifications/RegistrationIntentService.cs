using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using System.Json;
using Android.Widget;

namespace VolleyballApp
{
	[Service(Exported = false)]
	class RegistrationIntentService : IntentService
	{
		static object locker = new object();

		public RegistrationIntentService() : base("RegistrationIntentService") { }

		protected override void OnHandleIntent (Intent intent)
		{
			try {
				Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
				lock(locker) {
					var instanceID = InstanceID.GetInstance(this);
					instanceID.DeleteToken("826575748241", GoogleCloudMessaging.InstanceIdScope);
					var token = instanceID.GetToken("826575748241", GoogleCloudMessaging.InstanceIdScope);
					Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
					SendRegistrationToAppServer(token);
					Subscribe(token);
				}
			} catch {
				Log.Debug("RegistrationIntentService", "Failed to get a registration token");
				return;
			}
		}

		void SendRegistrationToAppServer (string token) {
			DB_Communicator db = DB_Communicator.getInstance();
			db.makeWebRequest("util/notification_util.php?token=" + token, "RegistrationIntentService.SendRegistrationToAppServer()");
		}

		void Subscribe (string token)
		{
			var pubSub = GcmPubSub.GetInstance(this);
			pubSub.Subscribe(token, "/topics/global", null);
		}
	}
}