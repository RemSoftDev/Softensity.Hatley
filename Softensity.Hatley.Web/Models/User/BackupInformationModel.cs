using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.Web.Models.User
{
    public class BackupInformationModel
    {
        public double Date { get; set; }
        public string BackupedFrom { get; set; }
        public string BackupedTo { get; set; }

        public BackupInformationModel(Backup backup)
        {
            Date = backup.TimeOfBackup.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            BackupedTo = backup.BackupedTo;
            BackupedFrom = backup.BackupedFrom;
        }
    }
}