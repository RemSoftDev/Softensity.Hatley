using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.DAL.Models
{
    public class Backup
    {
        public Backup()
        {
            BackupId = Guid.NewGuid();
        }

        public Guid BackupId { get; set; }
        public Guid UserId { get; set; }
        public DateTime TimeOfBackup { get; set; }
        public string BackupedFrom { get; set; }
        public string BackupedTo { get; set; }
        public virtual User User { get; set; }
    }
}
