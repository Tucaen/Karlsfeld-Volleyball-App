
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

namespace VolleyballApp {
	public class StatisticDetailsFragment : Fragment {
		private VBStatistic stats;

		public StatisticDetailsFragment(VBStatistic stats) {
			this.stats = stats;
		}

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View view = inflater.Inflate(Resource.Layout.StatisticDetails, container, false);

			view.FindViewById<TextView>(Resource.Id.statisticDetailsSaisonValue).Text = this.stats.season;
			view.FindViewById<TextView>(Resource.Id.statisticDetailsFrom).Text = this.stats.start.ToString("dd.MM.yy");
			view.FindViewById<TextView>(Resource.Id.statisticDetailsTill).Text = this.stats.end.ToString("dd.MM.yy");

			view.FindViewById<TextView>(Resource.Id.statisticDetailsCountTrainingsValue).Text = this.stats.countTraining.ToString();
			view.FindViewById<TextView>(Resource.Id.statisticDetailsCountParticipatedTrainingsValue).Text = this.stats.countParticipatedTraining.ToString();
			view.FindViewById<TextView>(Resource.Id.statisticDetailsTrainingTitleProzentualValue).Text = "(" + this.stats.trainingPercentage.ToString() + "%)";

			view.FindViewById<TextView>(Resource.Id.statisticDetailsCountMatchdaysValue).Text = this.stats.countMatchday.ToString();
			view.FindViewById<TextView>(Resource.Id.statisticDetailsCountParticipatedMatchdaysValue).Text = this.stats.countParticipatedMatchday.ToString();
			view.FindViewById<TextView>(Resource.Id.statisticDetailsMatchdayTitleProzentualValue).Text = "(" + this.stats.matchdayPercentage.ToString() + "%)";

			return view;
		}
	}
}

