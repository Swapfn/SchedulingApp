using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]              // makes field required to fill
        [EmailAddress]          // field name is as written
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]      // making datatype of the field is of the type (password)
        public string Password { get; set; }

        [Display(Name ="Remember Me ?")]        //Changing the display name to be "Remember Me ?"
        public bool RememberMe { get; set; }
    }
}
