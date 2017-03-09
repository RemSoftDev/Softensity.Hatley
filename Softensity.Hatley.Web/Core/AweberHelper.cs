using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Aweber;
using Aweber.Entity;
using Softensity.Hatley.Common;

namespace Softensity.Hatley.Web.Core
{
    public static class AweberHelper
    {
        public static AweberConfigurationSection AweberConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/AweberConfiguration") as AweberConfigurationSection;
        public static List<Subscriber> GetContactsFromAweber(string oAuthToken, string oAuthTokenSecret)
        {
            API api = new API(AweberConfigurationSection.ConsumerKey, AweberConfigurationSection.ConsumerSecret);
            api.OAuthToken = oAuthToken;
            api.OAuthTokenSecret = oAuthTokenSecret;
            Account account = api.getAccount();
            List<Subscriber> subscribers = account.lists().entries[0].subscribers().entries;
            return subscribers;
        }
    }
}