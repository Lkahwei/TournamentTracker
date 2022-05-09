using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /*
        Represent One match in the tournament
    */
    public class MatchupModel
    {
        //Text File Format: MatchupID, entries (Which entry | (vs) which entry) (if entry only one, means byes), winner id, Current Matchup Round
        
        public int Id { get; set; }

        /*
        A set of teams that were involved in this match.
        */
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /*
        The winner of the match
        */
        public int WinnerId { get; set; }
        public TeamModel Winner { get; set; }

        /*
        Which round this match is a part of
        */
        public int MatchupRound { get; set; }

        public string DisplayName
        {
            get
            {
                string output = "";

                foreach(MatchupEntryModel entry in Entries)
                {
                    if(entry.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = entry.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. {entry.TeamCompeting.TeamName}";
                        }
                    }
                    else
                    {
                        output = "Matchup has yet to be determined";
                    }
                    
                }
                return output;
            }
        }
    }
}
