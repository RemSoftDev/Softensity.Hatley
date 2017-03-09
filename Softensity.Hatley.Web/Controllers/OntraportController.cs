using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Models;

namespace Softensity.Hatley.Web.Controllers
{
    public class OntraportController : AuthorizedUserController
    {
        public OntraportController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public static FullServiceInfo BuildServiceInfo(ServiceInfo shortInfo)
        {
            var model = new FullServiceInfo(shortInfo)
            {
                ServiceName = "Ontraport",
                ServiceIcon = "~/content/icons/ONTRAPORT2.png",
                Controller = "Ontraport"
            };
            return model;
        }

        public static FullServiceInfo BuildServiceInfo(User user)
        {
            bool connected = user.InfusionSoftAccount != null;

            var model = new FullServiceInfo
            {
                AccountName = connected ? user.OntraportAccount.AccountName : null,
                Connected = connected,
                Enabled = connected && user.OntraportAccount.Enabled,
                ServiceName = "Ontraport",
                ServiceIcon = "~/content/icons/ONTRAPORT2.png",
                Controller = "Ontraport"
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
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult Disconnect()
        {
            UnitOfWork.OntraportRepository.Delete(CurrentUser.Id);
            UnitOfWork.Commit();
            return RedirectToAction<UserController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult SetEnabled(bool enabled)
        {
            if (CurrentUser.OntraportAccount != null)
            {
                CurrentUser.OntraportAccount.Enabled = enabled;
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