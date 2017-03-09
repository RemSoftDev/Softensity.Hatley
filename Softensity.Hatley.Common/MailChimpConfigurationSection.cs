using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.Common
{
    public class MailChimpConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("clientId", IsRequired = false)]
        public string ClientId
        {
            get { return (string)this["clientId"]; }
            set { this["clientId"] = value; }
        }
        [ConfigurationProperty("clientSecret", IsRequired = false)]
        public string ClientSecret
        {
            get { return (string)this["clientSecret"]; }
            set { this["clientSecret"] = value; }
        }
    }
}
