using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Softensity.Hatley.Web.Models
{
    public class AccountInformationModel
    {
        [Required(ErrorMessage = "Enter your email.")]
        [EmailAddress()]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter your name.")]
        [Display(Name = "Full Name:")]
        public string FullName { get; set; }

        [Display(Name = "Phone:")]
        [RegularExpression(@"(\(\d{3}\)\d{3}-\d{4})|(\d{10})", ErrorMessage = "Please enter the phone number in the format (xxx)xxx-xxxx or xxxxxxxxxx")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Enter your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password:")]
        public string ConfirmPassword { get; set; }
    }
}