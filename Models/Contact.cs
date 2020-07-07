using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Http;

namespace ContactDetailsApp.Models
{
    public class Contact:TableEntity
    {

       
        public long ContactNo { get; set; }
       
        [Required(ErrorMessage ="Please Enter First Name")]
        public string FirstName { get; set; }
        
         [Required(ErrorMessage ="Please Enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Please Enter Cell No")]
        public Int32 CellNo { get; set; }
        public IFormFile PhotoFile { get; set; }

        public string Group {get;set;}
    }
}