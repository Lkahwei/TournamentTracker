using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupEntryModel
    {
       
        //Text File Format: Matchup Entry Id, TeamCompeting ID, Score, Come from which matchup in id

        public int Id { get; set; }
        
        /*
         Represent One team in the match up 
        */
        public int TeamCompetingId { get; set; }  
        public TeamModel TeamCompeting { get; set; }

        /*
         Represent the score for this particular team
        */
        public double Score { get; set; }

        /*
         Represent the matchup that this team came from as the winner
        */
        public int ParentMatchupId { get; set; }
        public MatchupModel ParentMatchup { get; set; }
    }
}
