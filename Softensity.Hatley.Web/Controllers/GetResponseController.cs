using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Core;
using Softensity.Hatley.Web.Models;
using Softensity.Hatley.Web.Models.GetResponse;

namespace Softensity.Hatley.Web.Controllers
{
    public class GetResponseController : AuthorizedUserController
    {
        public GetResponseController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public static FullServiceInfo BuildServiceInfo(ServiceInfo shortInfo)
        {
            var model = new FullServiceInfo(shortInfo)
            {
                ServiceName = "GetResponse",
                ServiceIcon = "~/content/icons/getresponse.png",
                Controller = "GetResponse"
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
                ServiceName = "GetResponse",
                ServiceIcon = "~/content/icons/getresponse.png",
                Controller = "GetResponse"
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
            if (CurrentUser.GetResponseAccount != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
            return RedirectToAction<GetResponseController>(c => c.GetResponseSettings());
        }

        [HttpGet]
        public ActionResult GetResponseSettings()
        {
            if (CurrentUser.GetResponseAccount != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetResponseSettings(GetResponseModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string checkEntries = GetResponseHelper.CheckEntries(model.ApiKey);
                    if (checkEntries != null)
                    {
                        var getResponseAccount = new GetResponseAccount();
                        getResponseAccount.ApiKey = model.ApiKey;
                        getResponseAccount.AccountName = checkEntries;
                        getResponseAccount.ConnectingDate = DateTime.UtcNow;
                        getResponseAccount.Enabled = true;
                        CurrentUser.GetResponseAccount = getResponseAccount;
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
            UnitOfWork.GetResponseRepository.Delete(CurrentUser.Id);
            UnitOfWork.Commit();
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult SetEnabled(bool enabled)
        {
            if (CurrentUser.GetResponseAccount != null)
            {
                CurrentUser.GetResponseAccount.Enabled = enabled;
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