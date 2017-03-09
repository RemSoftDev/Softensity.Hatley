using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
    public class Payment : IdModel
    {
		[MaxLength(100)]
        public string PaymentId { get; set; }

		[Required]
		public DateTime PaymentDateUTC { get; set; }

        [Required]
        public string CardNumber { get; set; }

        public string Status { get; set; }

    }
}
