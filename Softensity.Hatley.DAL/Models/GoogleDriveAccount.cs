using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
	public class GoogleDriveAccount : IdModel, IServiceShortInfo
	{
		public DateTime ConnectingDate { get; set; }

		[MaxLength(100)]
		public string AccountName { get; set; }

		public bool Enabled { get; set; }

		[MaxLength(100)]
		public string AccessToken { get; set; }

		[MaxLength(100)]
		public string RefreshToken { get; set; }

		public long? ExpiresInSeconds { get; set; }

		public DateTime Issued { get; set; }

		[MaxLength(50)]

		public string Scope { get; set; }
		[MaxLength(50)]
		public string TokenType { get; set; }

	}

}
