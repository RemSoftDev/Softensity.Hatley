using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Models;
using Softensity.Hatley.Web.Core;
using Softensity.Hatley.Web.Models.ActiveCampaign;

namespace Softensity.Hatley.Web.Controllers
{
    public class ActiveCampaignController : AuthorizedUserController
    {
        public ActiveCampaignController(IUnitOfWork iUnitOfWork)
            : base(iUnitOfWork)
        {
        }

        public static FullServiceInfo BuildServiceInfo(ServiceInfo shortInfo)
        {
            var model = new FullServiceInfo(shortInfo)
            {
                ServiceName = "Active Campaign",
                ServiceIcon = "~/content/icons/active-campaign.png",
                Controller = "ActiveCampaign"
            };

            return model;
        }
        public static FullServiceInfo BuildServiceInfo(User user)
        {
            bool connected = user.ActiveCampaignAccount != null;

            var model = new FullServiceInfo
            {
                AccountName = connected ? user.ActiveCampaignAccount.AccountName : null,
                Connected = connected,
                Enabled = connected && user.ActiveCampaignAccount.Enabled,
                ServiceName = "Active Campaign",
                ServiceIcon = "~/content/icons/active-campaign.png",
                Controller = "ActiveCampaign"
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
            if (CurrentUser.ActiveCampaignAccount != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
            return RedirectToAction<ActiveCampaignController>(c => c.ActiveCampaignSettings());
        }

        public ActionResult ActiveCampaignSettings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ActiveCampaignSettings(ActiveCampaignModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string checkEntries = ActiveCampaignHelper.CheckEntries(model.ApiUrl, model.ApiKey);
                    if (checkEntries != null)
                    {
                        var activeCampaignAccount = new ActiveCampaignAccount();
                        activeCampaignAccount.ApiUrl = model.ApiUrl;
                        activeCampaignAccount.ApiKey = model.ApiKey;
                        activeCampaignAccount.AccountName = checkEntries;
                        activeCampaignAccount.ConnectingDate = DateTime.UtcNow;
                        activeCampaignAccount.Enabled = true;
                        CurrentUser.ActiveCampaignAccount = activeCampaignAccount;
                        UnitOfWork.Commit();
                        return RedirectToAction<UserController>(c => c.Index());
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "Wrong data!");
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Disconnect()
        {
            UnitOfWork.ActiveCampaignRepository.Delete(CurrentUser.Id);
            UnitOfWork.Commit();
            return RedirectToAction<UserController>(c => c.Index());
        }

        public void GetData()
        {
            ActiveCampaignHelper.GetContactsData(CurrentUser.ActiveCampaignAccount.ApiUrl, CurrentUser.ActiveCampaignAccount.ApiKey);
        }
        [HttpPost]
        public ActionResult SetEnabled(bool enabled)
        {
            if (CurrentUser.ActiveCampaignAccount != null)
            {
                CurrentUser.ActiveCampaignAccount.Enabled = enabled;
                UnitOfWork.Commit();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialTile();
            }
            return RedirectToAction<UserController>(c => c.Index());
        }

    }
}