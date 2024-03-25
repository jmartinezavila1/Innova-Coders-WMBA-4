using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace WMBA_4.Models
{
    public class Division
    {
        public int ID { get; set; }

        [Display(Name = "Division Name")]
        [Required(ErrorMessage = "You cannot leave the Division Name blank.")]
        [StringLength(100, ErrorMessage = "Division name can not be more than 100 characters long.")]
        public string DivisionName { get; set; }

        [Display(Name = "Division Number")]
        public int DivisionNumber {  get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; } = true;

        [Display(Name = "Club")]
        public int ClubID { get; set; }

        public Club Club { get; set; }

        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();
    }
}
