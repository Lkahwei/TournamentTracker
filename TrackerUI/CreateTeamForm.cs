using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {

        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();
        private List<PersonModel> availableTeamMembers = new List<PersonModel>();
        private ITeamRequester callingForm;
        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();

            callingForm = caller;
            //CreateSampleData();
            LoadListData();
            WireUpLists();
        }

        private void LoadListData()
        {
            availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        }

        private void WireUpLists()
        {

            selectTeamMemeberDropDown.DataSource = null;

            selectTeamMemeberDropDown.DataSource = availableTeamMembers;
            selectTeamMemeberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel model = new PersonModel(firstNameValue.Text, lastNameValue.Text, emailValue.Text, cellPhoneValue.Text);

                GlobalConfig.Connection.CreatePerson(model);

                availableTeamMembers.Add(model);

                WireUpLists();

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellPhoneValue.Text = "";

            }
            else
            {
                MessageBox.Show("You need to fill in all the fields");
            }
        }


        //This method is to validate the form
        private bool ValidateForm()
        {

            if (firstNameValue.Text.Length == 0 || lastNameValue.Text.Length == 0 || emailValue.Text.Length == 0 || cellPhoneValue.Text.Length == 0)
            {
                return false;
            }
            return true;
        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemeberDropDown.SelectedItem;

            if (p != null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);
            }

            WireUpLists();
        }

        private void deleteSelectedPlayers_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;

            if (p != null)
            {
                availableTeamMembers.Add(p);
                selectedTeamMembers.Remove(p);

                WireUpLists();
            }

        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel team = new TeamModel();

            team.TeamName = teamNameValue.Text;
            team.TeamMembers = selectedTeamMembers;

            GlobalConfig.Connection.CreateTeam(team);

            callingForm.TeamComplete(team);
            //Close the form
            this.Close();
        }
    }
}
