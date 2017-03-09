using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using MailChimp;
using MailChimp.Lists;
using MailChimp.Users;
using Softensity.Hatley.DAL.Models;

namespace MailChimpTest.Helpers
{
    public class MC_RestAPI
    {
        public string dc { get; set; }
        public string login_url { get; set; }
        public string api_endpoint { get; set; }
    }

    public class MailChimpHelper : OAuth2
    {
        public MailChimpHelper(Dictionary<string, string> p)
            : base(p)
        {

        }

        public string getLoginURL()
        {
            var p = new Dictionary<string, string>() { { "response_type", "code" }, { "client_id", _variables["client_id"] }, { "redirect_uri", _variables["redirect_uri"] } };
            return getURI(_variables["authorize_uri"], p);
        }

        private MC_RestAPI getMetaData()
        {
            var data = api("metadata", new Dictionary<string, string>());
            var serializer = new JavaScriptSerializer();
            var mc = serializer.Deserialize<MC_RestAPI>(data);
            return mc;
        }

        public MailChimpAccount GetMailChimpAccount()
        {
            var session = getSession();
            var rest_info = getMetaData();
            var api_key = session + "-" + rest_info.dc;

            MailChimpManager mc = new MailChimpManager(api_key);
            UserProfile userProfile = mc.GetUserProfile();

            return new MailChimpAccount()
            {
                AccountName = userProfile.Name,
                ApiKey = api_key,
                Enabled = true,
                ConnectingDate = DateTime.UtcNow
            };
        }

        public static List<MemberInfo> GetAllMembers(string api_key)
        {
            int limit = 100;
            var members = new List<MemberInfo>();
            MailChimpManager mc = new MailChimpManager(api_key);
            ListResult lists = mc.GetLists();
            foreach (var list in lists.Data)
            {
                int j = 0;
                while (true)
                {
                    MembersResult results = mc.GetAllMembersForList(list.Id, "subscribed", j, limit);
                    members.AddRange(results.Data);
                    if (results.Data.Count < limit)
                    {
                        break;
                    }
                    j += limit - 1;
                }
            }
            return members;
        }
    }
}