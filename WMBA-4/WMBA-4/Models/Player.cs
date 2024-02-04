using OfficeOpenXml.Utils;
using System.ComponentModel.DataAnnotations;
using WMBA_4.Data;

namespace WMBA_4.Models
{
    public class Player
    {
        public int ID { get; set; }


        // Summary properties
        [Display(Name = "Player")]
        public string Summary
        {
            get
            {
                return FirstName + " "
                    + LastName + " "
                    + (string.IsNullOrEmpty(JerseyNumber) ? " " :
                        (", " + JerseyNumber));
            }
        }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        //End Summary


        [Display(Name = "Member ID")]
        [Required(ErrorMessage = "You cannot leave the MemberID blank.")]
        [StringLength(8, ErrorMessage = "Member ID must be exactly 8 characters long.")]
        [RegularExpression(@"^[A-Za-z0-9]{8}$", ErrorMessage = "Member ID must be a combination of letters and numbers, exactly 8 characters long.")]
        public string MemberID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(100, ErrorMessage = "First name cannot be more than 100 characters long.")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; }


        [Display(Name = "Jersey Number")]
        [Range(1, 100, ErrorMessage = "Please enter a number between 1 and 100.")]
        public string JerseyNumber { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; } = true;

        [Display(Name = "Team")]
        public int TeamID { get; set; }
        public Team Team { get; set; }
        public ICollection<GameLineUp> GameLineUps { get; set; } = new HashSet<GameLineUp>();
        public ICollection<ScorePlayer> ScorePlayers { get; set; } = new HashSet<ScorePlayer>();
        
    }
}
