using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Appserverside3._0.DTO
{
    public class UserRegisterForDTO
    {
        [Required]
        public string Username { get; set;}
       
        [Required]
        public string Password { get; set; }
    }
}
