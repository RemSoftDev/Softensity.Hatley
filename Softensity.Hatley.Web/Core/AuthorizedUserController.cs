using System;
using System.Web.Mvc;
using System.Web;
using Microsoft.AspNet.Identity;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;

namespace Softensity.Hatley.Web.Controllers
{
    [Authorize]
    public abstract class AuthorizedUserController : BaseController
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected AuthorizedUserController(IUnitOfWork iUnitOfWork)
        {
            UnitOfWork = iUnitOfWork;
        }

        private User currentUser;
        private bool isUserInitialized;
        public User CurrentUser
        {
            get
            {
                if (!isUserInitialized && Request.IsAuthenticated)
                {
                    currentUser = UnitOfWork.UserManager.FindById(Guid.Parse(User.Identity.GetUserId()));
                    if (currentUser == null)
                    {
                        HttpContext.GetOwinContext().Authentication.SignOut(User.Identity.AuthenticationType);
                    }
                    else
                    {
                        isUserInitialized = true;
                    }
                }
                return currentUser;
            }
        }

        public bool CanCurrentUserBackup()
        {
            var googleDriveAccount = CurrentUser != null && CurrentUser.GoogleDriveAccount != null &&
                                     CurrentUser.GoogleDriveAccount.Enabled;
            var dropboxAccount = CurrentUser != null && CurrentUser.DropboxAccount != null &&
                                 CurrentUser.DropboxAccount.Enabled;
            var aweberAccount = CurrentUser != null && CurrentUser.AweberAccount != null &&
                                CurrentUser.AweberAccount.Enabled;
            var activeCampaignAccount = CurrentUser != null && CurrentUser.ActiveCampaignAccount != null &&
                                        CurrentUser.ActiveCampaignAccount.Enabled;
            var infusionSoftAccount = CurrentUser != null && CurrentUser.InfusionSoftAccount != null &&
                                      CurrentUser.InfusionSoftAccount.Enabled;

            return (googleDriveAccount || dropboxAccount) &&
                   (aweberAccount || activeCampaignAccount || infusionSoftAccount);
        }

    }
}
