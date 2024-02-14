using System.ComponentModel.DataAnnotations;
using System.Data;

namespace WMBA_4.Models
{
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

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(100, ErrorMessage = "First name cannot be more than 100 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; } = true;

        public int RoleId { get; set; }
        public Role Roles { get; set; }
        public ICollection<TeamStaff> TeamStaff { get; set; } = new HashSet<TeamStaff>();
    }
}
