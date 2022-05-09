using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;

namespace TrackerUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Initialize the connection to SQL and Textfile before run the main
            //It is now connected to SQL
            GlobalConfig.InitializeConnections(DatabaseType.Sql);

            ////It is now connected to Textfile
            //GlobalConfig.InitializeConnections(DatabaseType.TextFile);

            Application.Run(new TournamentDashboardForm());
        }
    }
}
