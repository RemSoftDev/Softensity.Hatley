using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.Common
{
    public class GetResponseConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("getResponseApiUrl", IsRequired = false)]
        public string GetResponseApiUrl
        {
            get { return (string)this["getResponseApiUrl"]; }
            set { this["getResponseApiUrl"] = value; }
        }
        [ConfigurationProperty("id", IsRequired = false)]
        public string id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }
    }
}