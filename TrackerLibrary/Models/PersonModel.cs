using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /*
         Represent One Person
    */
    public class PersonModel
    {
        //Text File Format: Person Id, Person First name, Person last name, Email, Phone Number
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public string CellPhoneNumber { get; set; }

        //Display name in the dropdown list
        public string FullName
        {
            get
            {
                return $"{ FirstName } { LastName }";
            }
        }

        public PersonModel()
        {

        }

        public PersonModel(string firstName, string lastName, string emailAddress, string cellPhoneNumber)
        {
            FirstName = firstName; 
            LastName = lastName;    
            EmailAddress = emailAddress;   
            CellPhoneNumber = cellPhoneNumber;

        }


    }
}
