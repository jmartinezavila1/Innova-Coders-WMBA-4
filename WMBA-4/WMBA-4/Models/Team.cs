using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace WMBA_4.Models
{
    public class Team
    {
        public int ID { get; set; }

        [Display(Name = "Team Name")]
        [Required(ErrorMessage = "You cannot leave the Team Name blank.")]
        [StringLength(100, ErrorMessage = "Team name can not be more than 100 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Coach Name")]
        [Required(ErrorMessage = "You cannot leave the Coach Name blank.")]
        [StringLength(100, ErrorMessage = "Team name can not be more than 100 characters long.")]
        public string Coach_Name { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; } = true;

        [Display(Name = "Division")]
        public int DivisionID { get; set; }

        public Division Division { get; set; }

        public ICollection<Player> Players { get; set; } = new HashSet<Player>();

        public ICollection<TeamGame> TeamGames { get; set; } = new HashSet<TeamGame>();

        public ICollection<GameLineUp> GameLineUps { get; set; } = new HashSet<GameLineUp>();

        public ICollection<TeamStaff> TeamStaff { get; set; } = new HashSet<TeamStaff>();
    }
}
