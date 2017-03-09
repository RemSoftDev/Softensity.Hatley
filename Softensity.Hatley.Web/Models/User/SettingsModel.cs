using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.Web.Models
{
    public class SettingsModel
    {
        public InfoModel InfoModel { get; set; }

        public PasswordModel PasswordModel { get; set; }
        
        [Display(Name = "Card Number:")]
        public string CardNumber { get; set; }

        [Display(Name = "Subscription Date:")]
        public string SubscriptionDate { get; set; }

        [Display(Name = "Status:")]
        public string Status { get; set; }

        public ICollection<Backup> Backup { get; set; }
    }

    public class InfoModel
    {
        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Full Name.")]
        [Display(Name = "Full Name:")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Enter Phone.")]
        [Display(Name = "Phone:")]
        [RegularExpression(@"(\(\d{3}\)\d{3}-\d{4})|(\d{10})", ErrorMessage = "Please enter the phone number in the format (xxx)xxx-xxxx or xxxxxxxxxx")]
        public string Phone { get; set; }

        public bool Done { get; set; }
    }

    public class PasswordModel
    {
        [Required(ErrorMessage = "Enter old password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Old password:")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Enter new password.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password:")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Passwords must be at least 6 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm new password.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password:")]
        public string ConfirmPassword { get; set; }
        
        public bool Done { get; set; }
    }
}