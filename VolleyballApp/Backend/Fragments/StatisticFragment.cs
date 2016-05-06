
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Threading;

namespace VolleyballApp {
	public class StatisticFragment : Fragment {
		ListView listView;
		public List<string> list { get; set; }
		View view;

		public StatisticFragment() {
			this.list = new List<string>();
			this.list.Add("Saison 2015/16");
			this.list.Add("Saison 2016/17");
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			view = inflater.Inflate(Resource.Layout.StatisticFragment, container, false);

			listView = view.FindViewById<ListView>(Resource.Id.listStatistics);
			listView.Adapter = new ListStatisticAdapter(this, list);
			listView.ItemClick += OnListItemClick;

			return view;
		}

		private async void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
			DB_Communicator db = DB_Communicator.getInstance();
			int userId = VBUser.GetUserFromPreferences().idUser;
			MainActivity main = ViewController.getInstance().mainActivity;

			ProgressDialog dialog = main.createProgressDialog("Please wait!", "Loading...");

			VBStatistic stats;
			switch(this.list[e.Position]) {
			case "Saison 2015/16":
				stats = new VBStatistic(userId, "15/16");
				await stats.loadAllData();
				main.switchFragment(ViewController.STATISTIC_FRAGMENT, ViewController.STATISTIC_DETAILS_FRAGMENT, new StatisticDetailsFragment(stats));
				break;
			case "Saison 2016/17":
				stats = new VBStatistic(userId, "16/17");
				await stats.loadAllData();
				main.switchFragment(ViewController.STATISTIC_FRAGMENT, ViewController.STATISTIC_DETAILS_FRAGMENT, new StatisticDetailsFragment(stats));
				break;
			}

			dialog.Dismiss();
		}
	}
}

