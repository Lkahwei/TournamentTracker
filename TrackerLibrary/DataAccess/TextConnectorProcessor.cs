using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
//* Load the text file
//* Convert the text to List<PrizeModeL>
//Find the max ID
//Add the new record with the new id (max + 1)
//COnvert the prizes to the list<string>
//Save the list<string> to the text file22

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        /*
            E.g: File name: PrizeModel.csv
            This method will return the full file path (including the c:/... as described in the App.config
            E.g Full File path: C:\data\TournamentTracker\PrizeModel.csv
         */
        public static string FullFilePath(this string fileName)
        {
            return $"{ConfigurationManager.AppSettings["filePath"]}\\{ fileName } ";
        }

        /*
         This method will read all the lines in the file and convert it into a list
         */
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file)){
                return new List<string>();
            }
            return File.ReadAllLines(file).ToList();
        }

        //Prize Methods
        /*
         This method will split each line by , (depending on how the data is stored in the file) and add to the new List<T>
         */
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel> ();

            foreach(string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();
                //Convert string "1" "2" to an integer
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2]; 
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }
            return output;
        }

        /*
         This method will loop through the List<T> and add the data into lines, and write to the file at the end (Delete the whole file and write the whole new data into it)
         */
        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage} ");
            }
            //List is an IEnumerable
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        //Person methods
        /*
         This method will split each line by , (depending on how the data is stored in the file) and add to the new List<T>
         */
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();
                //Convert string "1" "2" to an integer
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellPhoneNumber = cols[4];
                output.Add(p);
            }
            return output;
        }

        /*
         This method will loop through the List<T> and add the data into lines, and write to the file at the end (Delete the whole file and write the whole new data into it)
         */
        public static void SaveToPersonFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellPhoneNumber} ");
            }
            //List is an IEnumerable
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        //Team methods
        /*
         This method will split each line by , (depending on how the data is stored in the file) and add to the new List<T>
        And also since the person is stored as (PersonID | PersonID | etc..)
         */
        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel();
                
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    //This method will look through the List<PersonModel> and add the data from person into this List<PersonModel> with id matched
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);
            }
            return output;
        }

        /*
         This method will convert list<PersonModel> into string with the pattern of (PersonID1 | PersonID2 | etc..)
         */
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return "";
            }

            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";
            }

            //Remove the last |
            output = output.Substring(0, output.Length - 1);

            return output;

        }

        /*
         This method will loop through the List<T> and add the data into lines, and write to the file at the end (Delete the whole file and write the whole new data into it)
         */
        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models)
            {
                string peopleList = ConvertPeopleListToString(t.TeamMembers);
                lines.Add($"{t.Id},{t.TeamName},{peopleList}");
            }
            //List is an IEnumerable
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        //Tournament methods
        /*
         This method will split each line by , (depending on how the data is stored in the file) and add to the new List<T>
         */
        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines, string peopleFileName, string teamFileName, string prizeFileName)
        { 
            List<TournamentModel> output = new List<TournamentModel>();

            //Get List<T> for the element in the tournaments (Entered teams, tournament prizes,etc..)
            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel tm = new TournamentModel();
                
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');

                foreach (string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());

                }

                string[] prizeIds = cols[4].Split('|');
                foreach (string id in prizeIds)
                {
                    tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());

                }
                
                //Round is stored as matchupid^matchupid (round 1) | matchupid^matchupid (round 2) and etc..
                string[] rounds = cols[5].Split('|');
                List<MatchupModel> eachRound = new List<MatchupModel>();

                foreach(string round in rounds)
                {
                    eachRound = new List<MatchupModel>();
                    string[] macthupIds = round.Split('^');

                    foreach(string matchup in macthupIds)
                    {
                        eachRound.Add(matchups.Where(x => x.Id == int.Parse(matchup)).First());
                    }
                    tm.Rounds.Add(eachRound);
                }

                output.Add(tm);
            }
            return output;
        }

        //This method will convert each matchup into the format of matchup1 ^ matchup 2 ^ etc.. in that particular round
        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
            {
                return "";
            }

            foreach (MatchupModel matchup in matchups)
            {
                output += $"{matchup.Id}^";
            }

            //Remove the last |
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        //This method Convert each round into the format of round 1 | round 2 | round 3 | etc..
        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            //Round - id^id^id | id^id^id
            string output = "";

            if (rounds.Count == 0)
            {
                return "";
            }

            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ ConvertMatchupListToString(r)}|";
            }

            //Remove the last |
            output = output.Substring(0, output.Length - 1);

            return output;

        }

        //This method will save the tournaments details into the text file
        public static void SaveToTournamentsFile(this List<TournamentModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tm in models)
            {
                string teamList = ConvertTeamListToString(tm.EnteredTeams);
                string prizeList = ConvertPrizeListToString(tm.Prizes);
                lines.Add($"{tm.Id},{tm.TournamentName},{tm.EntryFee},{teamList},{ prizeList},{ ConvertRoundListToString(tm.Rounds)}");
            }
            //List is an IEnumerable
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        //Matchup entries methods
        /*
         This method will convert the lines from text file into List<MatchupEntryModel>
         */
        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupEntryModel me = new MatchupEntryModel();
                //Convert string "1" "2" to an integer
                me.Id = int.Parse(cols[0]);
                //Team Competing can be null if byes
                if(cols[1].Length == 0)
                {
                    me.TeamCompeting = null;
                }
                else
                {
                    me.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                }
                
                me.Score = double.Parse(cols[2]);
                //Represent parent matchup is null (For first round matchups)
                int parentId = 0;
                if (int.TryParse(cols[3], out parentId))
                {
                    me.ParentMatchup = LookupMatchupById(parentId);
                }
                else
                {
                    me.ParentMatchup = null;
                }
                
                output.Add(me);
            }
            return output;
        }

        //Matchups methods
        //This method will convert the string of matchup entry id ( matchup entry id 1 | matchup entry id 2) into the <List> matchup entry model
        private static List<MatchupEntryModel> ConvertMatchupEntryIdStringToMatchupEntryModel(string input)
        {
            //Ids for matchup entry in the matchup file
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach(string id in ids)
            {
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');

                    if(cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }
            //This method will convert the matching matchup entry id into the <List>matchup entry models
            output = matchingEntries.ConvertToMatchupEntryModels();
            return output;
        }

        /*
         This method will convert the lines from text file into List<matchupModel>
         */
        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            List<MatchupModel> output = new List<MatchupModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupModel m = new MatchupModel();

                m.Id = int.Parse(cols[0]);
                //Entries will be the matchup entry id
                m.Entries = ConvertMatchupEntryIdStringToMatchupEntryModel(cols[1]);
                if(cols[2].Length != 0)
                {
                    m.Winner = LookupTeamById(int.Parse(cols[2]));
                }
                else
                {
                    m.Winner = null;
                }
                
                m.MatchupRound = int.Parse(cols[3]);

                output.Add(m);
            }
            return output;
        }

        
        public static void SaveRoundsToFile(this TournamentModel tournament, string matchupFile, string matchupEntryFile)
        {
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    //Load all the matchups from file
                    //Get the top id and add one
                    //Store the id
                    //Save the matchup record
                    matchup.SaveMatchupToFile(matchupFile, matchupEntryFile);
                }


            }
        }

        //This method is to save the matchup details with the ids only into the text file
        public static void SaveMatchupToFile(this MatchupModel matchup, string matchupFile, string matchupEntryFile)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            //Get the latest id of the matchup in the text file
            int currentId = 1;

            if(matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }

            matchup.Id = currentId;

            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                
                entry.SaveEntryToFile(matchupEntryFile);
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }
            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        //This method is to convert the each matchup entry model into their id only and return the result string (Matchup entry1 | Matchup entry2)
        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";

            if (entries.Count == 0)
            {
                return "";
            }

            foreach (MatchupEntryModel me in entries)
            {
                output += $"{me.Id}|";
            }

            //Remove the last |
            output = output.Substring(0, output.Length - 1);

            return output;

        }

        //This method is to save the matchup entry for each matchup into the text file
        public static void SaveEntryToFile(this MatchupEntryModel matchupEntry, string matchupEntryFile)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            int currentId = 1;

            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }

            matchupEntry.Id = currentId;
            entries.Add(matchupEntry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{e.Id},{teamCompeting},{e.Score},{parent}");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
        }

        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return "";
            }

            foreach (TeamModel t in teams)
            {
                output += $"{t.Id}|";
            }

            //Remove the last |
            output = output.Substring(0, output.Length - 1);

            return output;

        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return "";
            }

            foreach (PrizeModel p in prizes)
            {
                output += $"{p.Id}|";
            }

            //Remove the last |
            output = output.Substring(0, output.Length - 1);

            return output;

        }

        private static TeamModel LookupTeamById(int id)
        {
            List<string> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile();
            
            foreach (string team in teams)
            {
                string[] cols = team.Split(',');
                if(cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels(GlobalConfig.PeopleFile).First();
                }
            }
            return null;
        }

        private static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile();
            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }
            return null;
        }

        //This method is to update the matchup model to the text file with the winner id
        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            MatchupModel oldMatchup = new MatchupModel();

            foreach(MatchupModel m in matchups)
            {
                if(m.Id == matchup.Id)
                {
                    oldMatchup = m;
                }
            }
            matchups.Remove(oldMatchup);
            matchups.Add(matchup);

            //This method is to update the entry to the file with the team competing id (in the advanced method) and also their respective score (if applicable)
            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                //Store the team competing id
                entry.UpdateEntriesToFile();
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }

            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        //This method is to update the entry to the file with the team competing id (in the advanced method) and also their respective score (if applicable)
        public static void UpdateEntriesToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            MatchupEntryModel oldEntry = new MatchupEntryModel(); 

            foreach(MatchupEntryModel me in entries)
            {
                if(me.Id == entry.Id)
                {
                    oldEntry = me;
                }
            }
            entries.Remove(oldEntry);

            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach(MatchupEntryModel me in entries)
            {

                string parent = "";
                if(me.ParentMatchup != null)
                {
                    parent = me.ParentMatchup.Id.ToString();   

                }
                string teamCompeting = "";
                if (me.TeamCompeting != null)
                {
                    teamCompeting = me.TeamCompeting.Id.ToString();

                }
                lines.Add($"{me.Id},{teamCompeting},{me.Score},{parent}");

            }
            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
        }
    }
}
