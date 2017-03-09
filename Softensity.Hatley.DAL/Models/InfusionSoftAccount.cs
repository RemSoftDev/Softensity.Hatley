using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
	public class InfusionSoftAccount : IdModel, IServiceShortInfo
    {
        public int ExpirationTime { get; set; }
        [MaxLength(100)]
        public string AccessToken { get; set; }
        [MaxLength(100)]
        public string RefreshToken { get; set; }
        public DateTime BegginingTime { get; set; }

		public bool Enabled { get; set; }

		[MaxLength(100)]
		public string AccountName { get; set; }
    }
}
