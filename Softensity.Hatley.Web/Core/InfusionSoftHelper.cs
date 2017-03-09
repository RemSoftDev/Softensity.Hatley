using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Controllers;

namespace Softensity.Hatley.Web.Core
{
    public class TokenData
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public int Expires_In { get; set; }
        public string Token_Type { get; set; }
        public string Scope { get; set; }
    }

    public class InfusionSoftHelper
    {
        public static InfusionSoftConfigurationSection InfusionSoftConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/InfusionSoftConfiguration") as InfusionSoftConfigurationSection;
        private readonly string DeveloperAppKey = InfusionSoftConfigurationSection.DeveloperAppKey;
        private readonly string DeveloperAppSecret = InfusionSoftConfigurationSection.DeveloperAppSecret;
        private readonly string tokenUrl = "https://api.infusionsoft.com/token";

        public InfusionSoftAccount RequestAccessToken(string code, string callbackUrl)
        {
            string tokenDataFormat = "code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code";
            string dataString = string.Format(tokenDataFormat, code, DeveloperAppKey, DeveloperAppSecret, callbackUrl);
            return GetAccessToken(dataString);
        }

        public InfusionSoftAccount RefreshAccessToken(string refreshToken)
        {
            string tokenDataFormat = "refresh_token={0}&grant_type=refresh_token";
            string dataString = string.Format(tokenDataFormat, refreshToken);
            return GetAccessToken(dataString, true);
        }

        private InfusionSoftAccount GetAccessToken(string dataString, bool authorized = false)
        {
            HttpWebRequest request = HttpWebRequest.Create(tokenUrl) as HttpWebRequest;
            request.Method = "POST";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            if (authorized)
            {
                request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Base64Encode(DeveloperAppKey + ":" + DeveloperAppSecret));
            }
            var dataBytes = Encoding.UTF8.GetBytes(dataString);
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(dataBytes, 0, dataBytes.Length);
            }
            string resultJSON = string.Empty;
            using (WebResponse response = request.GetResponse())
            {
                var sr = new StreamReader(response.GetResponseStream());
                resultJSON = sr.ReadToEnd();
                sr.Close();
            }
            var jsonSerializer = new JavaScriptSerializer();
            var tokenData = jsonSerializer.Deserialize<TokenData>(resultJSON);
            return new InfusionSoftAccount
            {
                AccessToken = tokenData.Access_Token,
                Enabled = true,
                ExpirationTime = tokenData.Expires_In,
                RefreshToken = tokenData.Refresh_Token,
                AccountName = tokenData.Scope.Replace("full|", ""),
                BegginingTime = DateTime.UtcNow
            };
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}