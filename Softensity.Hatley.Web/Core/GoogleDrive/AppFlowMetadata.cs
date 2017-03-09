using System;
using System.Configuration;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Util.Store;
using Softensity.Hatley.Common;
using Softensity.Hatley.Web.Controllers;

namespace Softensity.Hatley.Web.GoogleDrive
{
    public class AppFlowMetadata : FlowMetadata
    {
        public static GoogleConfigurationSection GoogleConfigurationSection =
            ConfigurationManager.GetSection("CustomConfigurationGroup/GoogleConfiguration") as
                GoogleConfigurationSection;

        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = GoogleConfigurationSection.GoogleClientId,
                    ClientSecret = GoogleConfigurationSection.GoogleClientSecret
                },

                Scopes = new[] { DriveService.Scope.Drive, Oauth2Service.Scope.UserinfoEmail },
                DataStore =
                    new FileDataStore(AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "\\GoogleAPI")
            });

        public override string GetUserId(Controller controller)
        {
            var cntrl = controller as AuthorizedUserController;
            return cntrl.CurrentUser.StringId;
        }

        public override string AuthCallback
        {
            get { return @"/GoogleDrive/ConnectCallBack"; }
        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}