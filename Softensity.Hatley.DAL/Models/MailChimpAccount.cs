using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
    public class MailChimpAccount : IdModel, IServiceShortInfo
    {
        [MaxLength(50)]
        public string ApiKey { get; set; }

        public DateTime ConnectingDate { get; set; }

        [MaxLength(100)]
        public string AccountName { get; set; }

        public bool Enabled { get; set; }
    }
}