using System;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Android.App;
using System.Net.Http;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Client.Methods;


namespace VolleyballApp {
	public class DB_Communicator {
		static string connectionString;
		static string requestEventForUser = "http://localhost/requestEventForUser.php";

		public DB_Communicator() {
			connectionString = "SERVER=localhost;" +
			"DATABASE=Volleyball_App_DB;" +
			"UID=root;" +
			"PASSWORD=root;";
		}

		public async void test() {
//			WebClient client = new WebClient();
//			Uri uri = new Uri(requestEventForUser);
//
//			NameValueCollection parameters = new NameValueCollection();
//			parameters.Add("userId", "1");
//
//			client.UploadValuesCompleted += Client_UploadValuesCompleted;
//			client.UploadValuesAsync(uri, parameters);

			HttpClient client = new HttpClient();
			HttpGet httpGet = new HttpGet(requestEventForUser);
			
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(requestEventForUser);

			string responseText;
			try {
				response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync();
			} catch(Exception e) {
				Console.WriteLine("Error while selecting data from MySQL: " + e.Message);
			}

		}

//		void Client_UploadValuesCompleted (object sender, UploadValuesCompletedEventArgs e)
//		{
//			Activity.RunOnUiThread(() => {
//				string test = Encoding.UTF8.GetString(e.Result);
//			});
//		}
	}
}
