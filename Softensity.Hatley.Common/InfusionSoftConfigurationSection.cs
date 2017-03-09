using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.Common
{
    public class InfusionSoftConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("developerAppKey", IsRequired = false)]
        public string DeveloperAppKey
        {
            get { return (string)this["developerAppKey"]; }
            set { this["developerAppKey"] = value; }
        }
        [ConfigurationProperty("developerAppSecret", IsRequired = false)]
        public string DeveloperAppSecret
        {
            get { return (string)this["developerAppSecret"]; }
            set { this["developerAppSecret"] = value; }
        }
    }
}
