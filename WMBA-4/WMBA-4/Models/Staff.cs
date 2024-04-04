using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace WMBA_4.Models
{
    [ModelMetadataType(typeof(StaffMetaData))]
    public class Staff
    {
        public int ID { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName + " ";
            }
        }

        public string FirstName { get; set; }

       
        public string LastName { get; set; }

       
        public string Email { get; set; }

      
        public bool Status { get; set; } = true;

        public int? RoleId { get; set; }
        public Role Roles { get; set; }

        public ICollection<TeamStaff> TeamStaff { get; set; } = new HashSet<TeamStaff>();

        public ICollection<StaffDivision> StaffDivision { get; set; } = new HashSet<StaffDivision>();

    }
}
