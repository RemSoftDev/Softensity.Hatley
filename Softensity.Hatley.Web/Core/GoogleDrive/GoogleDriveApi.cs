using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Controllers;

namespace Softensity.Hatley.Web.GoogleDrive
{
    public class GoogleDriveApi
    {
        private UserCredential _userCredential;

        public GoogleDriveApi(UserCredential userCredential)
        {
            _userCredential = userCredential;
        }

        public static AppFlowMetadata AppFlow = new AppFlowMetadata();

        public static GoogleDriveAccount RefreshUser(string userId, GoogleDriveAccount googleDriveAccount)
        {
            var credential = new UserCredential(AppFlow.Flow, userId, new TokenResponse
            {
                AccessToken = googleDriveAccount.AccessToken,
                ExpiresInSeconds = googleDriveAccount.ExpiresInSeconds,
                Issued = googleDriveAccount.Issued,
                RefreshToken = googleDriveAccount.RefreshToken,
                Scope = googleDriveAccount.Scope,
                TokenType = googleDriveAccount.TokenType
            });
            Task<bool> refresh = credential.RefreshTokenAsync(new CancellationToken());
            Task.WaitAll(refresh);
            return new GoogleDriveAccount
            {
                ConnectingDate = DateTime.UtcNow,
                Enabled = googleDriveAccount.Enabled,
                AccessToken = credential.Token.AccessToken,
                RefreshToken = credential.Token.RefreshToken,
                Issued = credential.Token.Issued,
                Scope = credential.Token.Scope,
                TokenType = credential.Token.TokenType,
                ExpiresInSeconds = credential.Token.ExpiresInSeconds,
                AccountName = googleDriveAccount.AccountName
            };
        }

        public void Upload(string fileName, byte[] bytes)
        {
            if (_userCredential != null)
            {
                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = _userCredential,
                    ApplicationName = "Datatumbler App"
                });
                Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
                body.Title = fileName;
                body.Description = "A test document";
                body.MimeType = "application/zip";

                var stream = new System.IO.MemoryStream(bytes);
                FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, "text/plain");
                request.Upload();
                if (request.ResponseBody == null)
                {
                    throw new Exception("User remove access to aplication.");
                }
            }
        }
    }
}