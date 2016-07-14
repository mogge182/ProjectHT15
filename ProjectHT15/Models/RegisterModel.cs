using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ProjectHT15.Models
{
    public class RegisterModel 
    {
        [Required(ErrorMessage = "required")]
        [Display(Name = "Username")]
        [Remote("CheckUserName", "Home", HttpMethod = "POST", ErrorMessage = "already exists.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "required")]
        [EmailAddress(ErrorMessage = " is invalid")]
        [Remote("CheckEmail", "Home", HttpMethod = "POST", ErrorMessage = "already exists.")]
        [StringLength(200, ErrorMessage = "adress to long.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(Int32.MaxValue, ErrorMessage = "to short.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "invalid")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(50, ErrorMessage = "is to long.")]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "required")]
        [StringLength(50, ErrorMessage = "is to long.")]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }


    }
}