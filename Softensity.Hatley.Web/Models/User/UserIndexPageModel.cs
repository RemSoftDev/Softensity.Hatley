using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Models.User;

namespace Softensity.Hatley.Web.Models
{
    public class UserIndexPageModel
    {
        public FullServiceInfo DropBox { get; set; }
        public FullServiceInfo GoogleDrive { get; set; }

        public ServiceInfo Aweber { get; set; }
        public ServiceInfo InfusionSoft { get; set; }
        public ServiceInfo ActiveCampaign { get; set; }
        public ServiceInfo MailChimp { get; set; }
        public ServiceInfo Ontraport { get; set; }
        public ServiceInfo GetResponse { get; set; }
        
        public bool AvailableBackup { get; set; }
        public ScheduleModel Schedule { get; set; }
    }

    public class ServiceInfo : IServiceShortInfo
    {
        public bool Connected { get; set; }
        public bool Enabled { get; set; }
        public string AccountName { get; set; }
        public string Url { get; set; }
    }

    public class FullServiceInfo : ServiceInfo
    {
        public string Controller { get; set; }
        public string ServiceName { get; set; }
        public string ServiceIcon { get; set; }

        public FullServiceInfo()
        {

        }

        public FullServiceInfo(ServiceInfo source)
        {
            Connected = source.Connected;
            Enabled = source.Enabled;
            AccountName = source.AccountName;
        }

    }

}