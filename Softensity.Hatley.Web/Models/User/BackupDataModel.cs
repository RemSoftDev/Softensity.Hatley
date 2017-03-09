using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softensity.Hatley.Web.Models.User
{
    public enum AccountEnum
    {
        Aweber,
        ActiveCampaign,
        InfusionSoft,
        Dropbox,
        GoogleDrive,
        MailChimp,
        Ontraport,
        GetResponse
    }

    public class BackupDataModel
    {
        public Guid UserId { get; set; }
        public List<string> Sources { get; set; }
        public List<string> Receivers { get; set; }

        public BackupDataModel()
        {
            Sources = new List<string>();
            Receivers = new List<string>();
        }
    }

    public enum ScheduleEnum
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }
}