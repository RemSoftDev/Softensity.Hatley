using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
	public class AweberAccount : IdModel, IServiceShortInfo
    {
        public DateTime ConnectingDate { get; set; }
        [MaxLength(100)]
        public string AccessToken { get; set; }
        [MaxLength(100)]
        public string TokenSecret { get; set; }

		public bool Enabled { get; set; }
		[MaxLength(100)]
		public string AccountName { get; set; }
    }
}
