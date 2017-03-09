using System.ComponentModel.DataAnnotations;

namespace Softensity.Hatley.Web.Models.AuthorizeNet
{
    public class PaymentInformationModel
    {
        [Required(ErrorMessage = "Enter your cart number.")]
        [Display(Name = "Cart Number:")]
        public string CartNumber { get; set; }

        [Required(ErrorMessage = "Enter expiration month.")]
        public int? ExpirationMonth { get; set; }

        [Required(ErrorMessage = "Enter expiration year.")]
        public int? ExpirationYear { get; set; }

        [Required(ErrorMessage = "Enter card code.")]
        [Display(Name = "Card Code:")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Enter your first name.")]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name.")]
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }

        public decimal Price { get; set; }
    }
}