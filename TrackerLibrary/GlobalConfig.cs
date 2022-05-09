using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess;
using System.Configuration;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        //List of text files for respective models 
        public const string PrizesFile = "PrizeModel.csv";
        public const string PeopleFile = "PersonModel.csv";
        public const string TeamFile = "TeamModel.csv";
        public const string TournamentFile = "TournamentModel.csv";
        public const string MatchupFile = "MatchupModel.csv";
        public const string MatchupEntryFile = "MatchupEntryModel.csv";

        public static IDataConnection Connection { get; private set; }

        //It is to initialize the connection that whether it is SQL or text file depends on the Program.cs main file
        public static void InitializeConnections (DatabaseType db)
        {
            if (db == DatabaseType.Sql)
            {
                // ToDO- Create the SQL connection
                SQLConnector sql  = new SQLConnector ();
                Connection = sql;
            }

            else if (db == DatabaseType.TextFile)
            {
                // TODO - Create the text connection
                TextConnector text = new TextConnector ();
                Connection = text;
            }
        }

        //This method is to return the connection string from the database in order to connect to the database
        public static string CnnString(string name)
        {
            //Return the connection string with the name passed into it.
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }

}
