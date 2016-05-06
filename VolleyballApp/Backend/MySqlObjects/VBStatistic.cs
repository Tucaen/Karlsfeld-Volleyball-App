using System;
using System.Json;
using System.Threading.Tasks;

namespace VolleyballApp {
	public class VBStatistic {
		private const string STATS_TRAINING = "Training", STATS_MATCHDAY = "Spieltag";
		private DB_Communicator db;
		public string season { get; set;}
		private int userId;

		public DateTime start { get; set;}
		public DateTime end { get; set;}

		public int countTraining { get; set;}
		public int countParticipatedTraining { get; set;}
		public int trainingPercentage { get; set;}

		public int countMatchday { get; set;}
		public int countParticipatedMatchday { get; set;}
		public int matchdayPercentage { get; set;}


		public VBStatistic(int userId, string season) {
			db = DB_Communicator.getInstance();
			this.season = season;
			this.userId = userId;
		}

		public async Task<Boolean> loadAllData() {
			await this.loadData(STATS_TRAINING);
			await this.loadData(STATS_MATCHDAY);
			return true;
		}

		private async Task<Boolean> loadData(string type) {
			string response = await db.makeWebRequest("service/user/stats.php?userId="+this.userId+"&eventName="+type+"&season="+this.season, "VBStatistic.OnListItemClick");
			JsonValue json = JsonValue.Parse(response);
			if(db.wasSuccesful(json)) {
				this.start = db.convertAndInitializeToDateTime(db.containsKey(json, "start", DB_Communicator.JSON_TYPE_DATE));
				this.end = db.convertAndInitializeToDateTime(db.containsKey(json, "end", DB_Communicator.JSON_TYPE_DATE));

				switch(type) {
				case STATS_TRAINING:
					this.countTraining = json["count_events"];
					this.countParticipatedTraining = json["count_participated_events"];
					if(countTraining == 0) {
						this.trainingPercentage = 0;						
					} else {
						this.trainingPercentage = Convert.ToInt16(Math.Round((Convert.ToDouble(this.countParticipatedTraining) / Convert.ToDouble(this.countTraining)) * 100));
					}
					break;
				case STATS_MATCHDAY:
					this.countMatchday = json["count_events"];
					this.countParticipatedMatchday = json["count_participated_events"];
					if(countMatchday == 0) {
						this.matchdayPercentage = 0;						
					} else {
						this.matchdayPercentage = Convert.ToInt16(Math.Round((Convert.ToDouble(this.countParticipatedMatchday) / Convert.ToDouble(this.countMatchday)) * 100));
					}
					break;
				}
			}
			return true;
		}
	}
}

