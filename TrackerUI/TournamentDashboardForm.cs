using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;
namespace TrackerUI
{
    public partial class TournamentDashboardForm : Form, ITournamentRequester
    {
        List<TournamentModel> tournaments = GlobalConfig.Connection.GetTournaments_All();
        public TournamentDashboardForm()
        {
            
            InitializeComponent();
            WireupLists();
        }

        private void WireupLists()
        {
            loadExistingTournamentDropdown.DataSource = null;
            loadExistingTournamentDropdown.DataSource = tournaments;
            loadExistingTournamentDropdown.DisplayMember = "TournamentName";
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            CreateTournamentForm frm = new CreateTournamentForm(this);
            frm.Show();
        }

        private void loadTournamentButton_Click(object sender, EventArgs e)
        {
            TournamentModel tournament = (TournamentModel)loadExistingTournamentDropdown.SelectedItem;
            TournamentViewerForm frm = new TournamentViewerForm(tournament);
            frm.Show();
        }

        public void TournamentComplete(TournamentModel tournament)
        {
            tournaments.Add(tournament);
            WireupLists();
        }
    }
}
