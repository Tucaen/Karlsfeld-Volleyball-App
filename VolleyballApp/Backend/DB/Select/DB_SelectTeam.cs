﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Json;
using Android.Widget;

namespace VolleyballApp {
	public class DB_SelectTeam : DB_Select {
		private string type;

		public DB_SelectTeam(DB_Communicator dbCommunicator) : base(dbCommunicator) {
			this.type = "DB_SelectTeam";
		}

		public async Task<VBTeam> SelectTeam(int idTeam) {
			string 	responseText = await dbCommunicator.makeWebRequest("service/team/load_teams.php?id=" + idTeam, type + ".SelectTeam");

			JsonValue json = JsonValue.Parse(responseText);
			if(!dbCommunicator.wasSuccesful(json)) {
				ViewController.getInstance().toastJson(null, json, ToastLength.Long, "There was an error while loading the team!");
			}

			return new VBTeam(json);
		}
		public async Task<List<VBTeam>> SelectTeams() {
			string responseText = await dbCommunicator.makeWebRequest("service/team/load_teams.php", type + ".SelectTeamsForUser");

			JsonValue json = JsonValue.Parse(responseText);
			if(!dbCommunicator.wasSuccesful(json)) {
				ViewController.getInstance().toastJson(null, json, ToastLength.Long, "There was an error while loading the teams!");
			}

			return createTeamListFromResponse(responseText);
		}

		public async Task<JsonValue> createTeam(string name, string sport, string location, string description) {
			string responseText = await dbCommunicator.makeWebRequest("service/team/create_team.php" + "?name=" + name + 
				"&location=" + location + "&sport=" + sport + "&desc="+ description, type + ".createTeam()");

			return JsonValue.Parse(responseText);
		}

		#region util
		private List<VBTeam> createTeamListFromResponse(string response) {
			List<VBTeam> list = new List<VBTeam>();
			JsonValue json = JsonValue.Parse(response);
			if(dbCommunicator.wasSuccesful(json)) {
				foreach(JsonValue jv in json["data"]) {
					JsonValue team = jv["Team"];
					list.Add(new VBTeam(team));
				}
			}
			return list;
		}
		#endregion
	}
}

