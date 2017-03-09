using System.Configuration;

namespace Softensity.Hatley.Common
{
    public class EmailConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("emailHost")]
        public string EmailHost
        {
            get { return (string)this["emailHost"]; }
            set { this["emailHost"] = value; }
        }
        [ConfigurationProperty("emailPort", IsRequired = false)]
        public int EmailPort
        {
            get { return (int)this["emailPort"]; }
            set { this["emailPort"] = value; }
        }
        [ConfigurationProperty("enableSsl", IsRequired = false)]
        public bool EnableSsl
        {
            get { return (bool)this["enableSsl"]; }
            set { this["enableSsl"] = value; }
        }
        [ConfigurationProperty("emailFrom")]
        public string EmailFrom
        {
            get { return (string)this["emailFrom"]; }
            set { this["emailFrom"] = value; }
        }
        [ConfigurationProperty("emailPass")]
        public string EmailPass
        {
            get { return (string)this["emailPass"]; }
            set { this["emailPass"] = value; }
        }
    }

}