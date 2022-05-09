using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        //Text File Format: Team Id, Team Name, Team Members (Person Id | Person Id | Person Id)
        public int Id { get; set; }
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
        public string TeamName { get; set; }

        public TeamModel()
        {

        }

    }
}
