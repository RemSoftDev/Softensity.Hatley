using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Softensity.Hatley.Web.Core.ActiveCampaign;

namespace Softensity.Hatley.Web.Core
{
    public class ActiveCampaignHelper
    {
        static WebClient Client = new WebClient();
        public static string CheckEntries(string apiUrl, string apiKey)
        {
            var uri = BuildUrl(apiUrl, apiKey, "account_view");
            var response = Client.DownloadString(uri);
            CheckingResult result = new CheckingResult();
            var serializer = new XmlSerializer(result.GetType());
            using (TextReader reader = new StringReader(response))
            {
                result = serializer.Deserialize(reader) as CheckingResult;
            }
            if (result.ResultCode == 1)
            {
                return result.Account;
            }
            throw new Exception("Credentials for Active Campaign are not valid");
        }

        public static Uri BuildUrl(string apiUrl, string apiKey, string action)
        {
            UriBuilder uriBuilder = new UriBuilder(apiUrl + "/admin/api.php?");
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("api_key=" + apiKey);
            queryBuilder.Append("&api_action=" + action);
            queryBuilder.Append("&api_output=xml");
            uriBuilder.Query = queryBuilder.ToString();
            return uriBuilder.Uri;
        }

        public static SubscribersListResult GetContactsData(string apiUrl, string apiKey)
        {
            CheckEntries(apiUrl,apiKey);
            var uri = BuildUrl(apiUrl, apiKey, "contact_list&ids=all&sort=id&sort_direction=ASC&page=all");
            var response = Client.DownloadString(uri);
            SubscribersListResult serialized = new SubscribersListResult();
            var serializer = new XmlSerializer(serialized.GetType());
            using (TextReader reader = new StringReader(response))
            {
                serialized = serializer.Deserialize(reader) as SubscribersListResult;
            }
            return serialized;
        }
    }
}