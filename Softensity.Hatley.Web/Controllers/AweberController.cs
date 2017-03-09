using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Aweber;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.Web.Core.Extensions;
using Softensity.Hatley.Web.Models;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.Web.Controllers
{
    public class AweberController : AuthorizedUserController
    {

        public AweberController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public static AweberConfigurationSection AweberConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/AweberConfiguration") as AweberConfigurationSection;
        private readonly string ConsumerKey = AweberConfigurationSection.ConsumerKey;
        private readonly string ConsumerSecret = AweberConfigurationSection.ConsumerSecret;

        public static FullServiceInfo BuildServiceInfo(ServiceInfo shortInfo)
        {
            var model = new FullServiceInfo(shortInfo)
            {
                ServiceName = "Aweber",
                ServiceIcon = "~/content/icons/aweber.png",
                Controller = "Aweber"
            };

            return model;
        }
        public static FullServiceInfo BuildServiceInfo(User user)
        {
            bool connected = user.AweberAccount != null;

            var model = new FullServiceInfo
            {
                AccountName = connected ? user.AweberAccount.AccountName : null,
                Connected = connected,
                Enabled = connected && user.AweberAccount.Enabled,
                ServiceName = "Aweber",
                ServiceIcon = "~/content/icons/aweber.png",
                Controller = "Aweber"
            };

            return model;
        }

        private ActionResult PartialTile()
        {
            var model = BuildServiceInfo(CurrentUser);
            return PartialView("_ServiceInfo", model);
        }

        public ActionResult Connect()
        {
            if (CurrentUser.AweberAccount != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
            var api = new API(ConsumerKey, ConsumerSecret)
            {
                CallbackUrl = Request.Url + "CallBack"
            };
            api.get_request_token();
            System.Web.HttpContext.Current.Session.Add("OAuthToken", api.OAuthToken);
            System.Web.HttpContext.Current.Session.Add("OAuthTokenSecret", api.OAuthTokenSecret);
            api.authorize();
            return null;
        }

        [HttpGet, ExportModelStateToTempData]
        public ActionResult ConnectCallBack()
        {
            try
            {
                var api = new API(ConsumerKey, ConsumerSecret)
                {
                    OAuthToken = (string)Session["OAuthToken"],
                    OAuthTokenSecret = (string)Session["OAuthTokenSecret"],
                    OAuthVerifier = Request["oauth_verifier"]
                };
                api.get_access_token();
                CurrentUser.AweberAccount = new AweberAccount
                {
                    AccessToken = api.OAuthToken,
                    Enabled = true,
                    TokenSecret = api.OAuthTokenSecret,
                    AccountName = String.Format("Account Id-{0}", api.getAccount().id),
                    ConnectingDate = DateTime.UtcNow
                };
                UnitOfWork.Commit();
            }
            catch
            {
                ModelState.AddModelError("", "Can't connect to Aweber.");
            }
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult Disconnect()
        {
            UnitOfWork.AweberRepository.Delete(CurrentUser.Id);
            UnitOfWork.Commit();
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult SetEnabled(bool enabled)
        {
            if (CurrentUser.AweberAccount != null)
            {
                CurrentUser.AweberAccount.Enabled = enabled;
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