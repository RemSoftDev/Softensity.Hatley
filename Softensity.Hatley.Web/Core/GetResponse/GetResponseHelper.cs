using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using Softensity.Hatley.Common;

namespace Softensity.Hatley.Web.Core
{
    public class GetResponseHelper
    {
        public static GetResponseConfigurationSection GetResponseConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/GetResponseConfiguration") as GetResponseConfigurationSection;
        public static string CheckEntries(string apiKey)
        {
            JsonObject jsonResponse = GetJsonRpc("get_account_info", new object[] { apiKey });

            if (jsonResponse["error"] != null)
            {
                throw new Exception("Credentials for GetResponse are not valid");
            }

            var result = (JsonObject)jsonResponse["result"];
            return (string)result["first_name"] + " " + result["last_name"];
        }

        public static JsonObject GetJsonRpc(string action, object[] param)
        {
            var jsonrequest = new JsonObject();
            jsonrequest["id"] = GetResponseConfigurationSection.id;
            jsonrequest["method"] = action;
            jsonrequest["params"] = param;
            var webRequest = (HttpWebRequest)WebRequest.Create(GetResponseConfigurationSection.GetResponseApiUrl);
            webRequest.Method = "POST";
            TextWriter writer = new StreamWriter(webRequest.GetRequestStream());
            writer.Write(jsonrequest.ToString());
            writer.Close();
            WebResponse response = webRequest.GetResponse();
            var import = new ImportContext();
            JsonReader reader = new JsonTextReader(new StreamReader(response.GetResponseStream()));
            var jsonResponse = import.Import(reader);
            return (JsonObject)jsonResponse;
        }

        public static List<Contact> GetContactsData(string apiKey)
        {
            JsonObject jsonResponse = GetJsonRpc("get_contacts", new object[] { apiKey });
            if (jsonResponse["error"] != null)
            {
                throw new Exception("Credentials for GetResponse are not valid");
            }
            var result = (JsonObject)jsonResponse["result"];
            var list = new List<Contact>();
            var serializer = new JavaScriptSerializer();
            foreach (var contactData in result)
            {
                string contactJson = contactData.Value.ToString();
                var contact = serializer.Deserialize<Contact>(contactJson);
                contact.id = contactData.Name;
                list.Add(contact);
            }
            return list;
        }
    }

    public class Contact
    {
        public string id { get; set; }
        public string ip { get; set; }
        public string name { get; set; }
        public string origin { get; set; }
        public string cycle_day { get; set; }
        public string email { get; set; }
        public string campaign { get; set; }
        public DateTime created_on { get; set; }
        public DateTime? changed_on { get; set; }
    }
}