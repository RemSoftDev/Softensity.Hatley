using System;
using System.Configuration;
using System.Web.Mvc;
using DropNet;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Core.Extensions;
using Softensity.Hatley.Web.Models;

namespace Softensity.Hatley.Web.Controllers
{
	public class DropBoxController : AuthorizedUserController
	{
	    public static DropboxConfigurationSection DropboxConfigurationSection =
	        ConfigurationManager.GetSection("CustomConfigurationGroup/DropboxConfiguration") as
	            DropboxConfigurationSection;
		DropNetClient dropNetClient = new DropNetClient(DropboxConfigurationSection.DropboxApiKey, DropboxConfigurationSection.DropboxAppSecret);


		DropNet.Models.UserLogin UserLoginInfo 
		{
			get
			{
				return Session["DropNetUserLogin"]  as DropNet.Models.UserLogin;
			}
			set{
				Session["DropNetUserLogin"] = value;
			}
		}


		public DropBoxController(IUnitOfWork uow)
			: base(uow)
		{

		}

		public static FullServiceInfo BuildServiceInfo(ServiceInfo shortInfo)
		{
			var model = new FullServiceInfo(shortInfo)
			{
				ServiceName = "DropBox",
				ServiceIcon = "~/content/icons/dropbox.png",
				Controller = "DropBox"
			};

			return model;
		}
		public static FullServiceInfo BuildServiceInfo(User user)
		{
			bool connected = user.DropboxAccount != null;

			var model = new FullServiceInfo
				{
					AccountName = connected ? user.DropboxAccount.AccountName : null,
					Connected = connected,
					Enabled = connected && user.DropboxAccount.Enabled,
					ServiceName = "DropBox",
					ServiceIcon = "~/content/icons/dropbox.png",
					Controller = "DropBox",
                    Url = @"https://www.dropbox.com/"
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
            if (CurrentUser.DropboxAccount != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
			UserLoginInfo = dropNetClient.GetToken();
			var url = dropNetClient.BuildAuthorizeUrl(Request.Url + "CallBack");
			return Redirect(url);
		}

		[HttpGet, ExportModelStateToTempData]
		public ActionResult ConnectCallBack()
		{
			try
			{
				dropNetClient.UserLogin = UserLoginInfo;
				var dropboxAccessToken = dropNetClient.GetAccessToken();
				var accountInfo = dropNetClient.AccountInfo();
				CurrentUser.DropboxAccount = new DropboxAccount
				{
					ConnectingDate = DateTime.UtcNow,
					Enabled = true,
					UserSecret = dropboxAccessToken.Secret,
					UserToken = dropboxAccessToken.Token,
					AccountName = accountInfo.display_name
				};
				UnitOfWork.Commit();
			}
			catch
			{
				ModelState.AddModelError("", "Can't connect to DropBox.");
			}
			return RedirectToAction<UserController>(c => c.Index());
		}



		[HttpPost]
		public ActionResult Disconnect()
		{
			UnitOfWork.DropboxRepository.Delete(CurrentUser.Id);
			UnitOfWork.Commit();
			return RedirectToAction<UserController>(c => c.Index());
		}

		[HttpPost]
		public ActionResult SetEnabled(bool enabled)
		{
			if (CurrentUser.DropboxAccount != null)
			{
				CurrentUser.DropboxAccount.Enabled = enabled;
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