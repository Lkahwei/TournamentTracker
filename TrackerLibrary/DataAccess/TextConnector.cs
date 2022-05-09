using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        /*
         This method will create the person and write into the person text file
         */
        public void CreatePerson(PersonModel model)
        {
            
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            //Find the max ID
            int currentId = 1;
            if (people.Count > 0)
            {
                //Order by Descending by ID property
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            //Add the new record with the new id (max + 1)
            model.Id = currentId;
            people.Add(model);

            //COnvert the person to the list<string>
            //Save the list<string> to the text file
            people.SaveToPersonFile(GlobalConfig.PeopleFile);
        }

        //This method will create the prize and write to the prize text file
        public void CreatePrize(PrizeModel model)
        {
            //Load the text file
            //Convert the text to List<PrizeModeL>
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            
            //Find the max ID
            int currentId = 1;
            if(prizes.Count > 0)
            {
                //Order by Descending by ID property
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }

            //Add the new record with the new id (max + 1)
            model.Id = currentId;
            prizes.Add(model);
                       
            //COnvert the prizes to the list<string>
            //Save the list<string> to the text file
            prizes.SaveToPrizeFile(GlobalConfig.PrizesFile);
        }

        //This method will create the team and write to the team text file
        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(GlobalConfig.PeopleFile);

            int currentId = 1;

            if(teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamFile(GlobalConfig.TeamFile);
        }

        //This method will create te tournamnet and write to the tournament text file
        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels(GlobalConfig.PeopleFile, GlobalConfig.TeamFile, GlobalConfig.PrizesFile);

            int currentId = 1;

            if(tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            model.SaveRoundsToFile(GlobalConfig.MatchupFile, GlobalConfig.MatchupEntryFile);

            tournaments.Add(model);


            tournaments.SaveToTournamentsFile(GlobalConfig.TournamentFile);

            //This method call is to update the winner for the first round for those matchups with byes
            TournamentLogic.UpdateTournamentResult(model);
        }

        public void UpdateMatchup(MatchupModel matchup)
        {
            matchup.UpdateMatchupToFile();
        }

        //The methods below is to get the List<T> from the text file
        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(GlobalConfig.PeopleFile);
        }

        public List<TournamentModel> GetTournaments_All()
        {
            return GlobalConfig.TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels(GlobalConfig.PeopleFile, GlobalConfig.TeamFile, GlobalConfig.PrizesFile);
        }

        
    }
}
