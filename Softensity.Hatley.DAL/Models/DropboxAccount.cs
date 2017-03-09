using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
	public class DropboxAccount : IdModel, IServiceShortInfo
    {
		[MaxLength(30)]
        public string UserSecret { get; set; }

        [MaxLength(30)]
        public string UserToken { get; set; }
        public DateTime ConnectingDate { get; set; }

		[MaxLength(100)]
		public string AccountName { get; set; }

		public bool Enabled { get; set; }
    }
}
