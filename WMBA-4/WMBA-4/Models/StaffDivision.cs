using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class StaffDivision
    {
        [Display(Name = "Division")]
        public int DivisionID { get; set; }
        public Division Division { get; set; }

        [Display(Name = "Staff")]
        public int StaffID { get; set; }
        public Staff Staff { get; set; }
    }
}
