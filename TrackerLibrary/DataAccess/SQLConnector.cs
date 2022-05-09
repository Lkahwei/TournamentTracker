using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SQLConnector : IDataConnection
    {
        //Database Name
        private const string db = "Tournaments";

        //Create T Methods
        public void CreatePrize(PrizeModel model)
        {
            //IDbConnection is something like the microsoft setup
            //Connect to SQL using this statement
            //Using using, whenever the } is reached, it ends the connection

            //Get the Connection to the Database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                //Normal Data Binding
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                //Output Stands for output variable
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                //Execute the procedure
                connection.Execute("dbo.inputPrizes", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }

        public void CreatePerson(PersonModel model)
        {

            //IDbConnection is something like the microsoft setup
            //Connect to SQL using this statement
            //Using using, whenever the } is reached, it ends the connection
            //Connection to the Database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                //Normal Data Binding
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@Email", model.EmailAddress);
                p.Add("@CellPhoneNumber", model.CellPhoneNumber);
                //Output Stands for output variable
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                //Execute the procedure
                connection.Execute("dbo.inputPersons", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

            }
        }

        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                //Normal Data Binding
                p.Add("@TeamName", model.TeamName);
                //Output Stands for output variable
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                //Execute the procedure
                connection.Execute("dbo.spTeam_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                foreach (PersonModel teamMembers in model.TeamMembers)
                {
                    //ToDo - Why can automatically insert id without the statement
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", teamMembers.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

            }
        }

        public void CreateTournament(TournamentModel tournament)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                //Normal Data Binding
                p.Add("@TournamentName", tournament.TournamentName);
                p.Add("@EntryFee", tournament.EntryFee);

                //This id will be getting from the current id in the database
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                //Execute the procedure
                connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

                //Get back the id from the binding parameters and bind to the current tournament model id
                tournament.Id = p.Get<int>("@id");

                foreach (PrizeModel prizeModel in tournament.Prizes)
                {
                    p = new DynamicParameters();
                    p.Add("@TournamentId", tournament.Id);
                    p.Add("@PrizeId", prizeModel.Id);
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
                }
                foreach (TeamModel teamModel in tournament.EnteredTeams)
                {
                    p = new DynamicParameters();
                    p.Add("@TournamentId", tournament.Id);
                    p.Add("@TeamId", teamModel.Id);
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
                }

                //Insert rounds for tournaments into the database or text file
                InsertRoundsForTournament(connection, tournament);

                //Update the winner for the byes matchup in the first round
                TournamentLogic.UpdateTournamentResult(tournament);

            }
        }

        //This method is to insert round into the database without winner, teamcompeting or parent matchup if applicable
        private void InsertRoundsForTournament(IDbConnection connection, TournamentModel tournament)
        {   
            /*
                1. Insert matchups into the matchup table
                2. Insert matchup entry (Represent each team) into the table
             */

            foreach (List<MatchupModel> round in tournament.Rounds)

                foreach (MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", tournament.Id);
                    p.Add("@MatchupRound", matchup.MatchupRound);
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", p, commandType: CommandType.StoredProcedure);

                    matchup.Id = p.Get<int>("@id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.Id);


                        if (entry.ParentMatchup == null)
                        {
                            p.Add("@ParentMatchupId", null);
                        }
                        else
                        {
                            p.Add("@ParentMatchupId", entry.ParentMatchup.Id);
                        }

                        if (entry.TeamCompeting == null)
                        {
                            p.Add("@TeamCompetingId", null);
                        }
                        else
                        {
                            p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                        
                        connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
                        entry.Id = p.Get<int>("@id");
                    }
                }
        }

        //Get T from Database
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.getAllPeople").ToList();
            }
            return output;
            
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.getAllTeams").ToList();

                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);

                    //Command Type to declare to run store procedure
                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            return output;

        }

        public List<TournamentModel> GetTournaments_All()
        {
            List<TournamentModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();

                foreach (TournamentModel tournament in output)
                {

                    p = new DynamicParameters();
                    p.Add("@TournamentId", tournament.Id);
                    //Populate Prizes
                    
                    tournament.Prizes = connection.Query<PrizeModel>("dbo.getPrizeByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    //Populate Teams

                    tournament.EnteredTeams = connection.Query<TeamModel>("dbo.spTeam_GetByTournaments", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel team in tournament.EnteredTeams)
                    {
                        p = new DynamicParameters();
                        p.Add("@TeamId", team.Id);

                        //Command Type to declare to run store procedure
                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }

                    //Populate Rounds
                    p = new DynamicParameters();
                    p.Add("@TournamentId", tournament.Id);
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach(MatchupModel matchup in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.Id);

                        matchup.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        //Populate each entry
                        //Populate each matchup
                        List<TeamModel> allTeams = GetTeam_All();

                        if(matchup.WinnerId > 0)
                        {
                            matchup.Winner = allTeams.Where(x => x.Id == matchup.WinnerId).First();
                        }

                        foreach(MatchupEntryModel matchEntry in matchup.Entries)
                        {
                            if(matchEntry.TeamCompetingId > 0)
                            {
                                matchEntry.TeamCompeting = allTeams.Where(x => x.Id == matchEntry.TeamCompetingId).First();
                            }

                            if(matchEntry.ParentMatchupId > 0)
                            {
                                matchEntry.ParentMatchup = matchups.Where(x => x.Id == matchEntry.ParentMatchupId).First();
                            }
                        }
                    }

                    List<MatchupModel> currentRow = new List<MatchupModel>();

                    int currentRound = 1;

                    foreach(MatchupModel matchup in matchups)
                    {
                        if(matchup.MatchupRound > currentRound)
                        {
                            tournament.Rounds.Add(currentRow);
                            currentRow = new List<MatchupModel>();
                            currentRound += 1;
                        }

                        currentRow.Add(matchup);
                    }
                    tournament.Rounds.Add(currentRow);
                }
            }
            return output;

        }

        //This method is to update the winner and also their score in the database
        public void UpdateMatchup(MatchupModel m)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                if (m.Winner != null)
                {
                    p.Add("@Id", m.Id);
                    p.Add("@WinnerId", m.Winner.Id);
                    connection.Execute("dbo.spMatchups_Update", p, commandType: CommandType.StoredProcedure);
                }

                //MatchupEntries_update
                foreach(MatchupEntryModel entry in m.Entries)
                {
                    p = new DynamicParameters();
                    if(entry.TeamCompeting != null)
                    {
                        p.Add("@id", entry.Id);
                        p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        p.Add("@Score", entry.Score);
                        connection.Execute("dbo.spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure);
                    }             
                }
            }
            
        }
    }
}
