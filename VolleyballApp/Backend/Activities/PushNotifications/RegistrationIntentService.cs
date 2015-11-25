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

		public RegistrationIntentService() : base("RegistrationIntentService") {}

		protected override void OnHandleIntent (Intent intent)
		{
			try {
				Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
				lock(locker) {
					var instanceID = InstanceID.GetInstance(this);
//					instanceID.DeleteToken("826575748241", GoogleCloudMessaging.InstanceIdScope);
					var token = instanceID.GetToken("826575748241", GoogleCloudMessaging.InstanceIdScope, null);
					Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
					SendRegistrationToAppServer(token);
					Subscribe(token, "global");
				}
			} catch {
				Log.Debug("RegistrationIntentService", "Failed to get a registration token");
				return;
			}
		}

		async void SendRegistrationToAppServer (string token) {
			DB_Communicator db = DB_Communicator.getInstance();
			await db.makeWebRequest("service/user/save_token.php?token=" + token, "RegistrationIntentService.SendRegistrationToAppServer()");
		}

		void Subscribe (string token, string topic)
		{
			var pubSub = GcmPubSub.GetInstance(this);
			pubSub.Subscribe(token, "/topics/" + topic, null);
		}
	}
}