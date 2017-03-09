using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
    public class IdModel
    {
        [Key, ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }

	public interface IServiceShortInfo
	{
		string AccountName { get; set; }

		bool Enabled { get; set; }
	}
}
