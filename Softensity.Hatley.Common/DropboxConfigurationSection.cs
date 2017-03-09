using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softensity.Hatley.Common
{
    public class DropboxConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("dropboxApiKey", IsRequired = false)]
        public string DropboxApiKey
        {
            get { return (string)this["dropboxApiKey"]; }
            set { this["dropboxApiKey"] = value; }
        }
        [ConfigurationProperty("dropboxAppSecret", IsRequired = false)]
        public string DropboxAppSecret
        {
            get { return (string)this["dropboxAppSecret"]; }
            set { this["dropboxAppSecret"] = value; }
        }
    }
}
