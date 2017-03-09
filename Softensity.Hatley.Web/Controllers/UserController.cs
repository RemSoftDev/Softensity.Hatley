using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using DropNet;
using DropNet.Exceptions;
using DropNet.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using InfusionSoft.Custom;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Core;
using Softensity.Hatley.Web.Core.Extensions;
using Softensity.Hatley.Web.GoogleDrive;
using Softensity.Hatley.Web.Models;
using Softensity.Hatley.Web.Models.AuthorizeNet;
using Softensity.Hatley.Web.Models.User;
using WebGrease.Css.Extensions;

namespace Softensity.Hatley.Web.Controllers
{
    [Authorize]
    public class UserController : AuthorizedUserController
    {
        public UserController(IUnitOfWork uow)
            : base(uow)
        {

        }
        private ILog logger = LogManager.GetLogger(typeof(UserController));

        //public static AuthorizeNetConfigurationSection AuthorizeNetConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/AuthorizeNetConfiguration") as AuthorizeNetConfigurationSection;

        public static DropboxConfigurationSection DropboxConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/DropboxConfiguration") as DropboxConfigurationSection;
        public static AweberConfigurationSection AweberConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/AweberConfiguration") as AweberConfigurationSection;

        [HttpGet, ImportModelStateFromTempData]
        public ActionResult Index()
        {
            if (CurrentUser.Payment == null)
            {
                return RedirectToAction<AuthorizeNetController>(c => c.Payment());
            }
            return View(BuildUserIndexModel());
        }

        private UserIndexPageModel BuildUserIndexModel()
        {
            return new UserIndexPageModel()
            {
                ActiveCampaign = CurrentUser.ActiveCampaignAccount == null
                                    ? new ServiceInfo() : new ServiceInfo
                                    {
                                        Connected = true,
                                        Enabled = CurrentUser.ActiveCampaignAccount.Enabled,
                                        AccountName = CurrentUser.ActiveCampaignAccount.AccountName
                                    },
                Aweber = CurrentUser.AweberAccount == null
                                    ? new ServiceInfo() : new ServiceInfo
                                    {
                                        Connected = true,
                                        Enabled = CurrentUser.AweberAccount.Enabled,
                                        AccountName = CurrentUser.AweberAccount.AccountName
                                    },

                InfusionSoft = CurrentUser.InfusionSoftAccount == null
                                    ? new ServiceInfo() : new ServiceInfo
                                    {
                                        Connected = true,
                                        Enabled = CurrentUser.InfusionSoftAccount.Enabled,
                                        AccountName = CurrentUser.InfusionSoftAccount.AccountName
                                    },
                MailChimp = CurrentUser.MailChimpAccount == null
                                    ? new ServiceInfo() : new ServiceInfo
                                    {
                                        Connected = true,
                                        Enabled = CurrentUser.MailChimpAccount.Enabled,
                                        AccountName = CurrentUser.MailChimpAccount.AccountName
                                    },
                Ontraport = CurrentUser.OntraportAccount == null
                                    ? new ServiceInfo() : new ServiceInfo
                                    {
                                        Connected = true,
                                        Enabled = CurrentUser.OntraportAccount.Enabled,
                                        AccountName = CurrentUser.OntraportAccount.AccountName
                                    },
                GetResponse = CurrentUser.GetResponseAccount == null
                                    ? new ServiceInfo() : new ServiceInfo
                                    {
                                        Connected = true,
                                        Enabled = CurrentUser.GetResponseAccount.Enabled,
                                        AccountName = CurrentUser.GetResponseAccount.AccountName
                                    },

                DropBox = DropBoxController.BuildServiceInfo(CurrentUser),
                GoogleDrive = GoogleDriveController.BuildServiceInfo(CurrentUser),
                AvailableBackup = CanCurrentUserBackup(),
                Schedule = new ScheduleModel(CurrentUser.LastBackupDate, CurrentUser.NextBackupDate, CurrentUser.BackupPeriod, CurrentUser.Time, (DayOfWeek?)CurrentUser.DayOfWeek, CurrentUser.DayOfMonth)
            };
        }

        public ActionResult Settings()
        {
            var data = new SettingsModel
            {
                CardNumber = "XXXX-XXXX-XXXX-" + CurrentUser.Payment.CardNumber,
                SubscriptionDate = CurrentUser.Payment.PaymentDateUTC.ToString(@"MM/dd/yy", CultureInfo.InvariantCulture),
                Status = CurrentUser.Payment.Status,
                Backup = CurrentUser.Backups.OrderByDescending(x => x.TimeOfBackup).ToList(),
                InfoModel = new InfoModel()
                {
                    Email = CurrentUser.Email,
                    FullName = CurrentUser.FullName,
                    Phone = CurrentUser.PhoneNumber
                }
            };
            return View(data);
        }

        [HttpPost]
        public ActionResult Settings(InfoModel data)
        {
            if (ModelState.IsValid)
            {
                CurrentUser.FullName = data.FullName;
                CurrentUser.PhoneNumber = data.Phone;
                UnitOfWork.Commit();
                data.Done = true;
            }
            return PartialView("_SettingsForm", data);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(PasswordModel data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await UnitOfWork.UserManager.ChangePasswordAsync(CurrentUser.Id, data.OldPassword, data.NewPassword);
                    if (result.Succeeded)
                    {
                        data.Done = true;
                    }
                    else
                    {
                        ModelState.AddModelError("OldPassword", "Invalid password");
                    }
                }
                return PartialView("_SettingsPasswordForm", data);
            }
            catch (Exception e)
            {
                logger.Error("Settings not saved", e);
                ModelState.AddModelError("", "Error:" + e.Message);
                return PartialView("_SettingsPasswordForm", data);
            }
        }

        [HttpPost]
        public ActionResult Information()
        {
            var user = CurrentUser;
            var model = new BackupDataModel { UserId = user.Id };

            if (user.InfusionSoftAccount != null && user.InfusionSoftAccount.Enabled)
            {
                model.Sources.Add(AccountEnum.InfusionSoft.ToString());
            }
            if (user.ActiveCampaignAccount != null && user.ActiveCampaignAccount.Enabled)
            {
                model.Sources.Add(AccountEnum.ActiveCampaign.ToString());
            }
            if (user.AweberAccount != null && user.AweberAccount.Enabled)
            {
                model.Sources.Add(AccountEnum.Aweber.ToString());
            }
            if (user.MailChimpAccount != null && user.MailChimpAccount.Enabled)
            {
                model.Sources.Add(AccountEnum.MailChimp.ToString());
            }
            if (user.OntraportAccount != null && user.OntraportAccount.Enabled)
            {
                model.Sources.Add(AccountEnum.Ontraport.ToString());
            }
            if (user.GetResponseAccount != null && user.GetResponseAccount.Enabled)
            {
                model.Sources.Add(AccountEnum.GetResponse.ToString());
            }

            if (user.DropboxAccount != null && user.DropboxAccount.Enabled)
            {
                model.Receivers.Add(AccountEnum.Dropbox.ToString());
            }
            if (user.GoogleDriveAccount != null && user.GoogleDriveAccount.Enabled)
            {
                model.Receivers.Add(AccountEnum.GoogleDrive.ToString());
            }
            return Json(model);
        }

        [HttpGet]
        public ActionResult Schedule()
        {
            return
                Json(
                    new ScheduleModel(CurrentUser.LastBackupDate, CurrentUser.NextBackupDate, CurrentUser.BackupPeriod, CurrentUser.Time, (DayOfWeek?)CurrentUser.DayOfWeek, CurrentUser.DayOfMonth),
                    JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Schedule(ScheduleEnum scheduleType = ScheduleEnum.None, int time = 0, int timezone = 0, DayOfWeek dayOfWeek = DayOfWeek.Monday, int dayOfMonth = 1)
        {
            CurrentUser.BackupPeriod = (int)scheduleType;
            switch (scheduleType)
            {
                case ScheduleEnum.Daily:
                    CurrentUser.Time = time;
                    CurrentUser.DayOfWeek = null;
                    CurrentUser.DayOfMonth = null;
                    break;
                case ScheduleEnum.Weekly:
                    CurrentUser.Time = time;
                    CurrentUser.DayOfWeek = (int)dayOfWeek;
                    CurrentUser.DayOfMonth = null;
                    break;
                case ScheduleEnum.Monthly:
                    CurrentUser.Time = time;
                    CurrentUser.DayOfWeek = null;
                    CurrentUser.DayOfMonth = dayOfMonth;
                    break;
                case ScheduleEnum.None:
                default:
                    CurrentUser.Time = null;
                    CurrentUser.DayOfWeek = null;
                    CurrentUser.DayOfMonth = null;
                    break;
            }
            CurrentUser.NextBackupDate = ScheduleHelper.GetNextDate(scheduleType, time, timezone, dayOfWeek, dayOfMonth);
            UnitOfWork.Commit();
            return
                Json(
                    new ScheduleModel(CurrentUser.LastBackupDate, CurrentUser.NextBackupDate, CurrentUser.BackupPeriod, CurrentUser.Time, (DayOfWeek?)CurrentUser.DayOfWeek, CurrentUser.DayOfMonth));

        }

        [HttpGet]
        public ActionResult BackupInformation()
        {
            var model =
                CurrentUser.Backups.OrderByDescending(x => x.TimeOfBackup)
                    .Select(x => new BackupInformationModel(x))
                    .ToArray();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}