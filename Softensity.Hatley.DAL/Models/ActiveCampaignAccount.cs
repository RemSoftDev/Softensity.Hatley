using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
	public class ActiveCampaignAccount : IdModel, IServiceShortInfo
	{
        public DateTime ConnectingDate { get; set; }

		[MaxLength(100)]
		public string ApiUrl { get; set; }

		[MaxLength(100)]
		public string ApiKey { get; set; }

		public bool Enabled { get; set; }

		[MaxLength(100)]
		public string AccountName { get; set; }
	}
}
