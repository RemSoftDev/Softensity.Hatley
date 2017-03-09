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
using InfusionSoft.Custom;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.Web.Core;
using Softensity.Hatley.Web.Core.Extensions;
using Softensity.Hatley.Web.Models;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.Web.Controllers
{
    public class InfusionSoftController : AuthorizedUserController
    {
        public static InfusionSoftConfigurationSection InfusionSoftConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/InfusionSoftConfiguration") as InfusionSoftConfigurationSection;
        private readonly string DeveloperAppKey = InfusionSoftConfigurationSection.DeveloperAppKey;
        private readonly string DeveloperAppSecret = InfusionSoftConfigurationSection.DeveloperAppSecret;

        public InfusionSoftController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public static FullServiceInfo BuildServiceInfo(ServiceInfo shortInfo)
        {
            var model = new FullServiceInfo(shortInfo)
            {
                ServiceName = "InfusionSoft",
                ServiceIcon = "~/content/icons/infusionsoft.png",
                Controller = "InfusionSoft"
            };

            return model;
        }
        public static FullServiceInfo BuildServiceInfo(User user)
        {
            bool connected = user.InfusionSoftAccount != null;

            var model = new FullServiceInfo
            {
                AccountName = connected ? user.InfusionSoftAccount.AccountName : null,
                Connected = connected,
                Enabled = connected && user.InfusionSoftAccount.Enabled,
                ServiceName = "InfusionSoft",
                ServiceIcon = "~/content/icons/infusionsoft.png",
                Controller = "InfusionSoft"
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
            if (CurrentUser.InfusionSoftAccount != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
            string authorizeUrlFormat = "https://signin.infusionsoft.com/app/oauth/authorize?scope=full&redirect_uri={0}&response_type=code&client_id={1}";
            var callbackUrl = Request.Url + "CallBack";
            Session["InfusionSoftcallbackUrl"] = callbackUrl;
            return Redirect(string.Format(authorizeUrlFormat, Server.UrlEncode(callbackUrl), DeveloperAppKey));
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
                var callbackUrl = (string)Session["InfusionSoftcallbackUrl"];
                var helper = new InfusionSoftHelper();
                var tokenData = helper.RequestAccessToken(code, Server.UrlEncode(callbackUrl));
                CurrentUser.InfusionSoftAccount = tokenData;
                UnitOfWork.Commit();
            }
            catch
            {
                ModelState.AddModelError("", "Can't connect to InfusionSoft.");
            }
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult Disconnect()
        {

            UnitOfWork.InfusionSoftRepository.Delete(CurrentUser.Id);
            UnitOfWork.Commit();
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult SetEnabled(bool enabled)
        {
            if (CurrentUser.InfusionSoftAccount != null)
            {
                CurrentUser.InfusionSoftAccount.Enabled = enabled;
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