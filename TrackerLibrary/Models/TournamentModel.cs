using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TournamentModel
    {
        //Text File Format: Tournament Id, Tournament Name, Entry Fee, Entered Teams (Team Id | Team Id | etc..), Tournamet Prize Id (Prize Id | Prize Id | etc...),
        //Rounds (MatchupId ^ MatchupId ^ etc... | MatchupId ^ MatchupId ^ etc... | etc...)
        public int Id { get; set; }
        public string TournamentName { get; set; }

        public decimal EntryFee { get; set; }

        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();

        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();

        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
    }
}
