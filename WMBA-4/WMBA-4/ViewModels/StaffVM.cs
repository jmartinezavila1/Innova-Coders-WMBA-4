using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WMBA_4.Models;

namespace WMBA_4.ViewModels
{
    [ModelMetadataType(typeof(StaffMetaData))]
    public class StaffVM
    {
        public int ID { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName + " ";
            }
        }

        public string FirstName { get; set; }


        public string LastName { get; set; }


  
    }
}
