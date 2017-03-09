using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Models;

namespace Softensity.Hatley.Web.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private ILog logger = LogManager.GetLogger(typeof(AuthController));
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        public AuthController(IUnitOfWork uowInstance)
        {
            unitOfWork = uowInstance;
            unitOfWork.UserManager.UserTokenProvider = new DataProtectorTokenProvider<User, Guid>(Startup.DataProtectionProvider.Create("EmailConfirmation"))
            {
                TokenLifespan = TimeSpan.FromHours(24)
            };
        }



        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel data)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = unitOfWork.UserManager.FindByEmail(data.Email);
                    if (user != null)
                    {
                        if (user.EmailConfirmed)
                        {
                            if (unitOfWork.UserManager.CheckPassword(user, data.Password))
                            {
                                await SignInAsync(user, data.RememberMe);
                                return RedirectToAction<UserController>(h => h.Index());
                            }
                            else
                            {
                                ModelState.AddModelError("Password", "Invalid password!");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Email is not confirmed.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid email or password.");
                    }
                    return View(data);
                }
                return View(data);
            }
            catch (Exception ex)
            {
                logger.Error("Problems while loginning", ex);
                ModelState.AddModelError("", ex);
                return View(data);
            }
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registration(AccountInformationModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(data);
                }
                var user = new User
                {
                    PhoneNumber = data.Phone,
                    FullName = data.FullName,
                    UserName = data.Email,
                    Email = data.Email
                };
                var result = unitOfWork.UserManager.Create(user, data.Password);
                if (result.Succeeded)
                {
                    unitOfWork.UserManager.AddToRole(user.Id, "User");
                    string code = await unitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code }, Request.Url.Scheme);
                    //string emailBody = "Welcome to our backup service! You almost completed registration on our site. Please click <a href=\"" + callbackUrl + "\">here</a>";
                    IdentityMessage message = new IdentityMessage
                    {
                        Body = "Welcome to our backup service! You almost completed registration on our site. Please click <a href=\"" + callbackUrl + "\">here</a>",
                        Destination = user.Email,
                        Subject = "Welcome to List Defender!"
                    };

                    EmailService emailService = new EmailService();
                    await emailService.SendAsync(message);
                    //await unitOfWork.UserManager.SendEmailAsync(user.Id, "Welcome to Datatumbler!", emailBody);
                    return View("CheckEmail");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(data);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Problems while registring", ex);
                ModelState.AddModelError("", "Error:" + ex.Message);
                return View(data);
            }
        }

        public async Task<ActionResult> ConfirmEmail(Guid userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            IdentityResult result = await unitOfWork.UserManager.ConfirmEmailAsync(userId, code);
            var identity = await unitOfWork.UserManager.CreateIdentityAsync(unitOfWork.UserManager.FindById(userId), DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
            return RedirectToAction<AuthorizeNetController>(c => c.Payment());
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotModel model)
        {
            if (ModelState.IsValid)
            {
                var user = unitOfWork.UserManager.FindByEmail(model.Email);
                if (user == null || user.EmailConfirmed == false)
                {
                    return View("ForgotPasswordConfirmation");
                }
                string code = await unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Auth", new {userId = user.Id, code = code}, protocol: Request.Url.Scheme);
                await unitOfWork.UserManager.SendEmailAsync(user.Id, "Reset password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction<AuthController>(c => c.ForgotPasswordConfirmation());
            }
            return View(model);
        }

        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public ActionResult ResetPassword(Guid userId, string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await unitOfWork.UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return RedirectToAction<AuthController>(c => c.ResetPasswordConfirmation());
            }
            var result = await unitOfWork.UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction<AuthController>(c => c.ResetPasswordConfirmation());
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View();
        }

        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction<AuthController>(a => a.Login());
        }


        private async Task SignInAsync(User user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var identity = await unitOfWork.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
    }
}