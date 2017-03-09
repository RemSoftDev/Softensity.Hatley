using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.Common
{
    public class GoogleConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("googleClientId", IsRequired = false)]
        public string GoogleClientId
        {
            get { return (string)this["googleClientId"]; }
            set { this["googleClientId"] = value; }
        }
        [ConfigurationProperty("googleClientSecret", IsRequired = false)]
        public string GoogleClientSecret
        {
            get { return (string)this["googleClientSecret"]; }
            set { this["googleClientSecret"] = value; }
        }
    }
}
