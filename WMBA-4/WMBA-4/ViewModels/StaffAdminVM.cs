using Microsoft.AspNetCore.Mvc;
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
        
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }= new List<string>();
    }
}
