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
    public partial class CreatePrizeForm : Form
    {
        IPrizeRequester callingForm;
        public CreatePrizeForm(IPrizeRequester caller)
        {
            InitializeComponent();

            placeNameValue.Text = "";
            placeNumberValue.Text = "";
            prizeAmountValue.Text = "0";
            prizePercentageValue.Text = "0";

            callingForm = caller;
        }
        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PrizeModel model = new PrizeModel(placeNameValue.Text, placeNumberValue.Text, prizeAmountValue.Text, prizePercentageValue.Text);

                GlobalConfig.Connection.CreatePrize(model);

                //The prize complete method from Iprizerequester Update the selected prizes list in the tournament form
                callingForm.PrizeComplete(model);

                //Close current form
                this.Close();

                
            }
            else
            {
                MessageBox.Show("This form has invalid message, Please Enter Again!");
            }
        }

        //Validate the form data
        private bool ValidateForm()
        {
            bool output = true;
            int placeNumber = 0;
            bool placeNumberValidate = int.TryParse(placeNumberValue.Text, out placeNumber);
            if (placeNumberValidate == false)
            {
                output = false;
            }
            if (placeNumber < 1)
            {
                output = false;
            }
            if (placeNameValue.Text.Length == 0)
            {
                output = false;
            }

            decimal prizeAmount = 0;
            double prizePercentage = 0;

            bool prizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageValue.Text, out prizePercentage);

            if (prizeAmountValid == false || prizePercentageValid == false)
            {
                output = false;
            }
            if(prizeAmount <= 0 && prizePercentage <= 0)
            {
                output = false;
            }
            if(prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
            }

            return output;
        }

        
    }
}
