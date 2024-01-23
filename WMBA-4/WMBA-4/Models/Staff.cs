using System.ComponentModel.DataAnnotations;

namespace WMBA_4.Models
{
    public class Staff
    {
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(100, ErrorMessage = "First name cannot be more than 100 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set;}

        public ICollection<TeamStaff> TeamStaff { get; set; } = new HashSet<TeamStaff>();
    }
}
