using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace WMBA_4.ViewModels
{
    public class UserVM
    {
        public string ID { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; }
    }
    

}
