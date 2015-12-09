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
		private string senderId = "826575748241";

		public RegistrationIntentService() : base("RegistrationIntentService") {}

		protected override void OnHandleIntent (Intent intent)
		{
			try {
				Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
				lock(locker) {
					var instanceID = InstanceID.GetInstance(this);
//					instanceID.DeleteToken("826575748241", GoogleCloudMessaging.InstanceIdScope);
					var token = instanceID.GetToken(senderId, GoogleCloudMessaging.InstanceIdScope, null);
					ViewController.getInstance().token = token;
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
			string response = await db.makeWebRequest("service/user/save_token.php?token=" + token, "RegistrationIntentService.SendRegistrationToAppServer()");
			JsonValue json = JsonValue.Parse(response);
			if(json["message"].ToString().Equals("\"NotRegistered\"")) {
				var instanceID = InstanceID.GetInstance(this);
				instanceID.DeleteToken(senderId, GoogleCloudMessaging.InstanceIdScope);
				ViewController.getInstance().token = "";
				var intent = new Intent (this, typeof (RegistrationIntentService));
				StartService (intent);
			}
		}

		void Subscribe (string token, string topic)
		{
			var pubSub = GcmPubSub.GetInstance(this);
			pubSub.Subscribe(token, "/topics/" + topic, null);
		}
	}
}