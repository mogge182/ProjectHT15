using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectHT15.Models
{
    public class LoginModel
    {

        [Required]
        [Display(Name = "Password")]
        [StringLength(50,ErrorMessage = "is wrong.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "is wrong.")]
        public string Salt { get; set; }
    }
}