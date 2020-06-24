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

        [StringLength(8,MinimumLength =4,ErrorMessage ="Password must be between 4 and 8 characters")]
        [Required]
        public string Password { get; set; }
    }
}
