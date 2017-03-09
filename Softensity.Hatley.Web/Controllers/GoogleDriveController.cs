using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DropNet;
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
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Core.Extensions;
using Softensity.Hatley.Web.GoogleDrive;
using Softensity.Hatley.Web.Models;

namespace Softensity.Hatley.Web.Controllers
{
    public class GoogleDriveController : AuthorizedUserController
    {
        public static AppFlowMetadata AppFlow = new AppFlowMetadata();

        public UserCredential GoogleDriveCredential
        {
            get { return (UserCredential)Session["GoogleDriveCredential"]; }
            set { Session["GoogleDriveCredential"] = value; }
        }

        public GoogleDriveController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public static FullServiceInfo BuildServiceInfo(ServiceInfo shortInfo)
        {
            var model = new FullServiceInfo(shortInfo)
            {
                ServiceName = "Google Drive",
                ServiceIcon = "~/content/icons/google-drive.png",
                Controller = "GoogleDrive"
            };
            return model;
        }

        public static FullServiceInfo BuildServiceInfo(User user)
        {
            bool connected = user.GoogleDriveAccount != null;

            var model = new FullServiceInfo
            {
                AccountName = connected ? user.GoogleDriveAccount.AccountName : null,
                Connected = connected,
                Enabled = connected && user.GoogleDriveAccount.Enabled,
                ServiceName = "Google Drive",
                ServiceIcon = "~/content/icons/google-drive.png",
                Controller = "GoogleDrive",
                Url = @"https://drive.google.com/"
            };

            return model;
        }

        private ActionResult PartialTile()
        {
            var model = BuildServiceInfo(CurrentUser);
            return PartialView("_ServiceInfo", model);
        }

        [HttpPost]
        public async Task<ActionResult> Connect(CancellationToken cancellationToken)
        {
            if (CurrentUser.GoogleDriveAccount != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
            var result = await new AuthorizationCodeMvcApp(this, AppFlow).AuthorizeAsync(cancellationToken);
            if (result.Credential == null)
            {
                return Redirect(result.RedirectUri);
            }
            else
            {
                CancellationToken taskCancellationToken = new CancellationToken();
                var token = result.Credential.Token;
                var credential = new UserCredential(AppFlow.Flow, CurrentUser.StringId, new TokenResponse
                {
                    AccessToken = token.AccessToken,
                    ExpiresInSeconds = token.ExpiresInSeconds,
                    Issued = token.Issued,
                    RefreshToken = token.RefreshToken,
                    Scope = token.Scope,
                    TokenType = token.TokenType
                });
                await credential.RefreshTokenAsync(taskCancellationToken);
                await credential.RevokeTokenAsync(taskCancellationToken);
                result = await new AuthorizationCodeMvcApp(this, AppFlow).AuthorizeAsync(taskCancellationToken);
                return Redirect(result.RedirectUri);
            }
        }

        [HttpGet, ExportModelStateToTempData]
        public async Task<ActionResult> ConnectCallBack(AuthorizationCodeResponseUrl authorizationCode,
            CancellationToken taskCancellationToken)
        {
            try
            {
                var userId = CurrentUser.StringId;
                var returnUrl = Request.Url.ToString();
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?"));
                var token =
                    await
                        AppFlow.Flow.ExchangeCodeForTokenAsync(userId, authorizationCode.Code, returnUrl,
                            taskCancellationToken);
                var api = new Oauth2Service(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = new UserCredential(AppFlow.Flow, userId, token)
                });
                var userInfo = api.Userinfo.Get().Execute();
                CurrentUser.GoogleDriveAccount = new GoogleDriveAccount
                {
                    ConnectingDate = DateTime.UtcNow,
                    Enabled = true,
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    Issued = token.Issued,
                    Scope = token.Scope,
                    TokenType = token.TokenType,
                    ExpiresInSeconds = token.ExpiresInSeconds,
                    AccountName = userInfo.Name
                };
                UnitOfWork.Commit();
            }
            catch
            {
                ModelState.AddModelError("", "Can't connect to Google Drive.");
            }
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public async Task<ActionResult> Disconnect(CancellationToken taskCancellationToken)
        {
            try
            {
                if (DateTime.UtcNow.Subtract(CurrentUser.GoogleDriveAccount.ConnectingDate).TotalSeconds + 300 >
                        CurrentUser.GoogleDriveAccount.ExpiresInSeconds)
                {
                    CurrentUser.GoogleDriveAccount = GoogleDriveApi.RefreshUser(CurrentUser.StringId, CurrentUser.GoogleDriveAccount);
                    UnitOfWork.Commit();
                }

                var token = CurrentUser.GoogleDriveAccount;
                var credential = new UserCredential(AppFlow.Flow, CurrentUser.StringId, new TokenResponse
                {
                    AccessToken = token.AccessToken,
                    ExpiresInSeconds = token.ExpiresInSeconds,
                    Issued = token.Issued,
                    RefreshToken = token.RefreshToken,
                    Scope = token.Scope,
                    TokenType = token.TokenType
                });

                await credential.RevokeTokenAsync(taskCancellationToken);
            }
            catch { }
            UnitOfWork.GoogleDriveRepository.Delete(CurrentUser.Id);
            UnitOfWork.Commit();
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult SetEnabled(bool enabled)
        {
            if (CurrentUser.GoogleDriveAccount != null)
            {
                CurrentUser.GoogleDriveAccount.Enabled = enabled;
                UnitOfWork.Commit();
            }
            if (Request.IsAjaxRequest())
            {
                return PartialTile();
            }
            else
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
        }
    }
}