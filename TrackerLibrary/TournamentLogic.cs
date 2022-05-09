using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        //Order the list randomly of teams
        //Check if it is big enough / enough of teams else add in byes
        // 2 ^ N (Two teams play with each other)
        //Create the first round of matchup
        //Create every round after that
        // (2^N) / 2 and all the way down to one

        //This method is to randomize the team to meet each other in the matchup
        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }

        public static void CreateRounds(TournamentModel tournament)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(tournament.EnteredTeams);
            int rounds = FindNumberofRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            tournament.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

            CreateOtherRounds(tournament, rounds);

            
        }

        //This method is to find the numbers of round needed to find the final winner.
        private static int FindNumberofRounds(int teamCount)
        {
            int totalRound = 1;
            int val = 2;

            while (val < teamCount)
            {
                totalRound += 1;

                val *= 2;
            }
            return totalRound;
        }

        //This method is to find the numbers of byes for the first round
        private static int NumberOfByes(int rounds, int numberOfTeamsEntered)
        {
            int byes = 0;
            int totalTeamsNeeded = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeamsNeeded *= 2;
            }

            byes = totalTeamsNeeded - numberOfTeamsEntered;

            return byes;
        }

        //This method is to create first round for the tournament with byes
        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> firstRound = new List<MatchupModel>();  
            MatchupModel currentMatchup = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                currentMatchup.Entries.Add(new MatchupEntryModel { TeamCompeting = team });
                
                if(byes > 0 || currentMatchup.Entries.Count > 1)
                {
                    currentMatchup.MatchupRound = 1;
                    firstRound.Add(currentMatchup);
                    currentMatchup = new MatchupModel();

                    if(byes > 0)
                    {
                        byes -= 1;
                    }
                }
            }
            return firstRound;
        }

        //This method is to create another round from the first round until the round that can determine the final winner
        //(round with no team id yet but with the parent matchup of which the team is came from)
        private static void CreateOtherRounds(TournamentModel tournament, int rounds)
        {
            int round = 2;
            //Previous Round of the second round will be the first round
            List<MatchupModel> previousRound = tournament.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();
            while(round <= rounds)
            {
                foreach(MatchupModel match in previousRound)
                {
                    //Has yet to determined the winner yet, hence only add the parent Matchup
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                    if(currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currentRound.Add(currMatchup);
                        currMatchup  = new MatchupModel();
                    }
                }
                tournament.Rounds.Add(currentRound);
                previousRound = currentRound;

                currentRound = new List<MatchupModel>();
                round += 1;
            }
        }

        //This method is to update the score and also the winner in the advanced (coming up) matchup and current matchup
        public static void UpdateTournamentResult(TournamentModel model)
        {
            List<MatchupModel> toScore = new List<MatchupModel>();

            foreach(List<MatchupModel> round in model.Rounds)
            {
                foreach(MatchupModel roundMatch in round)
                {
                    if(roundMatch.Winner == null && (roundMatch.Entries.Any(x => x.Score != 0) || roundMatch.Entries.Count == 1))
                    {
                        //Add that one matchup only with score entered
                        toScore.Add(roundMatch);
                    }
                }
            }

            MarkWinnerInMatchups(toScore);
            AdvanceWinners(toScore, model);
            //This is to update the current matchup winner and their score
            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));
        }
        
        //This method is to update the winner as the team competing in the next round matchup in advanced
        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            foreach (MatchupModel m in models)
            {
                foreach (List<MatchupModel> rounds in tournament.Rounds)
                {
                    foreach (MatchupModel roundMatchup in rounds)
                    {
                        foreach (MatchupEntryModel me in roundMatchup.Entries)
                        {
                            if (me.ParentMatchup != null)
                            {
                                if (me.ParentMatchup.Id == m.Id)
                                {
                                    me.TeamCompeting = m.Winner;
                                    //Update the winner and/or team competing for the next round matchup
                                    GlobalConfig.Connection.UpdateMatchup(roundMatchup);
                                }
                            }

                        }
                    }
                }
            }
        }

        //This method is to mark the winner in the current matchup with higher score
        private static void MarkWinnerInMatchups(List<MatchupModel> models)
        {
            foreach(MatchupModel matchup in models)
            {
                if(matchup.Entries.Count == 1)
                {
                    matchup.Winner = matchup.Entries[0].TeamCompeting;
                    continue;
                }
                
                if(matchup.Entries[0].Score > matchup.Entries[1].Score)
                {
                    matchup.Winner = matchup.Entries[0].TeamCompeting;
                }
                else if (matchup.Entries[0].Score < matchup.Entries[1].Score)
                {
                    matchup.Winner = matchup.Entries[1].TeamCompeting;
                }
                else
                {
                    throw new Exception("We do not allow ties in this application");
                }
            }
        }
    }
}
