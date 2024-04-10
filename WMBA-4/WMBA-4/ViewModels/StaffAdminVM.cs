using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WMBA_4.Models;

namespace WMBA_4.ViewModels
{
    [ModelMetadataType(typeof(StaffMetaData))]
    public class StaffAdminVM : StaffVM
    {
        public string Email { get; set; }
        public bool Status { get; set; } = true;

        [Display(Name = "Select Teams")]
        public int SelectedTeamID { get; set; }

        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }= new List<string>();

        public List<SelectListItem> TeamList { get; set; }
    }
}
