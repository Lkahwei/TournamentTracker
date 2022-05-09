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
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tm;
        //Using of Binding List would not need to re-wireup the list
        BindingList<int> rounds = new BindingList<int>();
        List<MatchupModel> selectedRoundDisplay = new List<MatchupModel>();
        public TournamentViewerForm(TournamentModel tournament)
        {
            InitializeComponent();
            tm = tournament;
            WireUpLists();
            LoadFormData();

            LoadRounds();
        }

        private void WireUpLists()
        {
            //roundDropDown.DataSource = null;
            roundDropDown.DataSource = rounds;
            //matchupListBox.DataSource = selectedRoundDisplay;
            //matchupListBox.DisplayMember = "DisplayName";

        }

        private void WireUpMatchupLists()
        {
            matchupListBox.DataSource = selectedRoundDisplay;
            matchupListBox.DisplayMember = "DisplayName";
        }

        private void LoadFormData()
        {
            tournamentName.Text = tm.TournamentName;
        }

        private void LoadRounds()
        {
            rounds.Clear();

            rounds.Add(1);
            int currRound = 1;

            foreach(List<MatchupModel> round in tm.Rounds)
            {
                int matchUpRound = round.First().MatchupRound;
                if(matchUpRound > currRound)
                {
                    currRound = round.First().MatchupRound;
                    rounds.Add(currRound);
                }
            }
            
            LoadMatchups(1);
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        //This method is to load the all the matchups for that particular round
        private void LoadMatchups(int selectedRound)
        {
            foreach (List<MatchupModel> round in tm.Rounds)
            {
                
                if (round.First().MatchupRound == selectedRound)
                {
                    selectedRoundDisplay = round;    
                }
                List<MatchupModel> newSelectedRound = new List<MatchupModel>();
                foreach(MatchupModel matchup in selectedRoundDisplay)
                {
                    if (matchup.Winner == null || !unplayedOnlyCheckbox.Checked)
                    {
                        newSelectedRound.Add(matchup);
                    }
                }
                selectedRoundDisplay = newSelectedRound;
            }
            if(selectedRoundDisplay.Count > 0)
            {
                LoadMatchup(selectedRoundDisplay.First());
            }
            WireUpMatchupLists();
            DisplayMatchInfo();
            
        }

        //If no more unplayed matchup is available, all become invisible
        private void DisplayMatchInfo()
        {
            bool isVisible = (selectedRoundDisplay.Count > 0);

            teamOneNameLabel.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;
            teamTwoNameLabel.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
            vsLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }


        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListBox.SelectedItem);
        }

        //This method is to load the matchup details for the selected matchup
        private void LoadMatchup(MatchupModel m)
        {

            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamOneNameLabel.Text = m.Entries[0].TeamCompeting.TeamName;
                        teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                        teamTwoNameLabel.Text = "BYES";
                        teamTwoScoreValue.Text = "0";
                    }
                    else
                    {
                        teamOneNameLabel.Text = "Not yet Set";
                    }
                }
                else
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        teamTwoNameLabel.Text = m.Entries[1].TeamCompeting.TeamName;
                        teamTwoScoreValue.Text = m.Entries[1].Score.ToString();
                    }
                    else
                    {
                        teamTwoNameLabel.Text = "Not yet Set";
                    }
                }

            }

        }

        //This method is to validate the data for two teams score
        private string ValidateData()
        {
            string output = "";

            double teamOneScore = 0;
            double teamTwoScore = 0;

            bool scoreOneValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);
            bool scoreTwoValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

            if (!scoreOneValid)
            {
                output = "Team one score is invalid!";
            }
            else if (!scoreTwoValid)
            {
                output = "Team two score is invalid!";
            }
            else if (teamOneScore == 0 && teamTwoScore == 0)
            {
                output = "You did not enter the score for either team!";
            }
            else if (teamOneScore == teamTwoScore)
            {
                output = "This application does not accept tie game!";
            }

            return output;
        }

        //This method is to update the score for the selected matchup
        private void scoreButton_Click(object sender, EventArgs e)
        {
            string errorMessage = ValidateData();
            if(errorMessage.Length > 0)
            {
                MessageBox.Show($"Input Error: { errorMessage }");
                return;
            }
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
            double teamOneScore = 0;
            double teamTwoScore = 0;
            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamOneNameLabel.Text = m.Entries[0].TeamCompeting.TeamName;

                        bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);
                        if (scoreValid)
                        {
                            m.Entries[0].Score = double.Parse(teamOneScoreValue.Text);
                        }
                        else 
                        {
                            MessageBox.Show("Please enter a valid score for team 1!");
                            return;
                        }
                        
                    }
                    else
                    {
                        teamOneNameLabel.Text = "Not yet Set";
                    }
                }
                else
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        teamTwoNameLabel.Text = m.Entries[1].TeamCompeting.TeamName;

                        bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);
                        if (scoreValid)
                        {
                            m.Entries[1].Score = double.Parse(teamTwoScoreValue.Text);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team 2!");
                            return;
                        }
                    }
                    else
                    {
                        teamTwoNameLabel.Text = "Not yet Set";
                    }
                }
            }
            TournamentLogic.UpdateTournamentResult(tm);

            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }
    }
}
