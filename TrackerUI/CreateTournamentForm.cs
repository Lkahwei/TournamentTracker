using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess;
using TrackerLibrary;
namespace TrackerUI
{
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        private ITournamentRequester callingForm;
        public CreateTournamentForm(ITournamentRequester caller)
        {
            InitializeComponent();
            callingForm = caller;
            WireUpLists();
        }

        private void WireUpLists()
        {

            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            //What to display in the dropdown
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentPlayersListBox.DataSource = null;
            tournamentPlayersListBox.DataSource = selectedTeams;
            tournamentPlayersListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";

        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;

            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);
            }

            WireUpLists();
        }

        private void deleteSelectedTeams_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentPlayersListBox.SelectedItem;

            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);
            }

            WireUpLists();
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            //Call the CreatePrizeForm obj
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();
        }

        public void PrizeComplete(PrizeModel model)
        {
            selectedPrizes.Add(model);
            WireUpLists();
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }

        private void deleteSelectedPrizes_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;

            if (p != null)
            {
                selectedPrizes.Remove(p);

                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            //Validate data
            decimal fee = 0;

            bool feeAcceptable = decimal.TryParse(entryFeeValue.Text, out fee);

            if (!feeAcceptable) 
            {
                MessageBox.Show("You need to enter a valid Entry Fee.", "Invalid Fee", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Create the tournament model
            TournamentModel tournament = new TournamentModel();

            tournament.TournamentName = tournamentNameValue.Text;
            tournament.EntryFee = fee;

            

            //First Approach
            foreach (PrizeModel prize in selectedPrizes)
            {
                tournament.Prizes.Add(prize);
            }

            foreach (TeamModel team in selectedTeams)
            {
                tournament.EnteredTeams.Add(team);
            }

            ////Second Approach
            //tournament.Prizes = selectedPrizes;
            //tournament.EnteredTeams = selectedTeams;

           //Create Rounds for all the matchups
            TournamentLogic.CreateRounds(tournament);

            GlobalConfig.Connection.CreateTournament(tournament);

            //TournamentViewerForm frm = new TournamentViewerForm(tournament);
            //frm.Show();
            callingForm.TournamentComplete(tournament);
            this.Close();
        }
    }
}
