using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailChimp;
using MailChimp.Users;
using MailChimpTest.Helpers;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Core;
using Softensity.Hatley.Web.Core.Extensions;
using Softensity.Hatley.Web.Models;

namespace Softensity.Hatley.Web.Controllers
{
    public class MailChimpController : AuthorizedUserController
    {
        public static MailChimpConfigurationSection InfusionSoftConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/MailChimpConfiguration") as MailChimpConfigurationSection;

        private Dictionary<string, string> _configuration = new Dictionary<string, string>()
            {
                {"client_id", InfusionSoftConfigurationSection.ClientId},
                {"client_secret", InfusionSoftConfigurationSection.ClientSecret},
                {"authorize_uri", "https://login.mailchimp.com/oauth2/authorize"},
                {"access_token_uri", "https://login.mailchimp.com/oauth2/token"},
                {"base_uri", "https://login.mailchimp.com/oauth2/"}
            };

        public MailChimpController(IUnitOfWork uow)
            : base(uow)
        {

        }

        public static FullServiceInfo BuildServiceInfo(ServiceInfo shortInfo)
        {
            var model = new FullServiceInfo(shortInfo)
            {
                ServiceName = "MailChimp",
                ServiceIcon = "~/content/icons/mailchimp.png",
                Controller = "MailChimp"
            };
            return model;
        }

        public static FullServiceInfo BuildServiceInfo(User user)
        {
            bool connected = user.InfusionSoftAccount != null;

            var model = new FullServiceInfo
            {
                AccountName = connected ? user.MailChimpAccount.AccountName : null,
                Connected = connected,
                Enabled = connected && user.MailChimpAccount.Enabled,
                ServiceName = "MailChimp",
                ServiceIcon = "~/content/icons/mailchimp.png",
                Controller = "MailChimp"
            };

            return model;
        }

        private ActionResult PartialTile()
        {
            var model = BuildServiceInfo(CurrentUser);
            return PartialView("_ServiceInfo", model);
        }

        [HttpPost]
        public ActionResult Connect()
        {
            var callbackUrl = Request.Url + "CallBack";
            Session["MailChimpcallbackUrl"] = callbackUrl;
            _configuration.Add("redirect_uri", callbackUrl);
            var mailChimpHelper = new MailChimpHelper(_configuration);
            return Redirect(mailChimpHelper.getLoginURL());
        }

        [HttpGet, ExportModelStateToTempData]
        public ActionResult ConnectCallBack(string code)
        {
            try
            {
                if (String.IsNullOrEmpty(code))
                {
                    throw new Exception();
                }
                var callbackUrl = (string)Session["MailChimpcallbackUrl"];
                _configuration.Add("redirect_uri", callbackUrl);
                _configuration.Add("code", code);
                var mailChimpHelper = new MailChimpHelper(_configuration);
                CurrentUser.MailChimpAccount = mailChimpHelper.GetMailChimpAccount();
                UnitOfWork.Commit();
            }
            catch
            {
                ModelState.AddModelError("", "Can't connect to MailChimp.");
            }
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult Disconnect()
        {
            UnitOfWork.MailChimpRepository.Delete(CurrentUser.Id);
            UnitOfWork.Commit();
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult SetEnabled(bool enabled)
        {
            if (CurrentUser.MailChimpAccount != null)
            {
                CurrentUser.MailChimpAccount.Enabled = enabled;
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