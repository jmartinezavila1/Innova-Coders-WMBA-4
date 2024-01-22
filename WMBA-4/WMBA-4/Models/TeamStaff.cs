using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class TeamStaff
    {
        [Display(Name = "Team")]
        public int TeamID { get; set; }
        public Team Team { get; set; }

        [Display(Name = "Staff")]
        public int StaffID { get; set; }
        public Staff Staff { get; set; }
    }
}
