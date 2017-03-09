using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.Common
{
    public class AweberConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("consumerKey", IsRequired = false)]
        public string ConsumerKey
        {
            get { return (string)this["consumerKey"]; }
            set { this["consumerKey"] = value; }
        }
        [ConfigurationProperty("consumerSecret", IsRequired = false)]
        public string ConsumerSecret
        {
            get { return (string)this["consumerSecret"]; }
            set { this["consumerSecret"] = value; }
        }
    }
}