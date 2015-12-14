using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Json;

namespace VolleyballApp {
	public class DB_SelectTeam : DB_Select {
		public DB_SelectTeam(DB_Communicator dbCommunicator) : base(dbCommunicator) {}



		public async Task<List<VBTeam>> SelectTeams(string host) {
			return await this.SelectTeamsForUser(host, 0);
		}
		public async Task<List<VBTeam>> SelectTeamsForUser(string host, int idUser) {
			string responseText;
			if(idUser == 0) {
				responseText = await dbCommunicator.makeWebRequest("service/team/load_teams.php", "DB_SelectTeam.SelectTeamsForUser");
			} else {
				responseText = await dbCommunicator.makeWebRequest("service/team/load_teams.php?id=" + idUser,
																									"DB_SelectTeam.SelectTeamsForUser");
			}

			return createTeamListFromResponse(responseText);
		}

		private List<VBTeam> createTeamListFromResponse(string response) {
			List<VBTeam> list = new List<VBTeam>();
			JsonValue json = JsonValue.Parse(response);
			if(dbCommunicator.wasSuccesful(json)) {
				foreach(JsonValue jv in json["data"]) {
					JsonValue team = jv["Team"];
					list.Add(createTeamFromResponse(team));
				}
			}
			return list;
		}

		private VBTeam createTeamFromResponse(JsonValue json) {
			return new VBTeam(dbCommunicator.convertAndInitializeToInt(dbCommunicator.containsKey(json, "id", DB_Communicator.JSON_TYPE_INT)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(json, "name", DB_Communicator.JSON_TYPE_STRING)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(json, "sport", DB_Communicator.JSON_TYPE_STRING)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(json, "location", DB_Communicator.JSON_TYPE_STRING)),
				dbCommunicator.convertAndInitializeToString(dbCommunicator.containsKey(json, "description", DB_Communicator.JSON_TYPE_STRING)));
		}
	}
}

