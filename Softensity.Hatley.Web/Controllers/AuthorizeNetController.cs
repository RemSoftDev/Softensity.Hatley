using System;
using System.Configuration;
using System.Web.Mvc;
using log4net;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Models.AuthorizeNet;

namespace Softensity.Hatley.Web.Controllers
{
    [Authorize]
    public class AuthorizeNetController : AuthorizedUserController
    {
        private StripePayment payment = new StripePayment();
        //private static StripeConfigurationSection stripeConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/StripeConfiguration") as StripeConfigurationSection;
        //private string planId = stripeConfigurationSection.PlanId;
        private string planId = ConfigurationManager.AppSettings["planId"];// "default id";
        //public static AuthorizeNetConfigurationSection AuthorizeNetConfigurationSection = ConfigurationManager.GetSection("CustomConfigurationGroup/AuthorizeNetConfiguration") as AuthorizeNetConfigurationSection;
        private ILog logger = LogManager.GetLogger(typeof(AuthorizeNetController));
        public AuthorizeNetController(IUnitOfWork uow)
            : base(uow)
        {
        }
        //SubscriptionGateway gate = new SubscriptionGateway(AuthorizeNetConfigurationSection.ApiLogin, AuthorizeNetConfigurationSection.TransactionKey);
        public ActionResult Payment()
        {
            if (CurrentUser.Payment != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
            return View(new PaymentInformationModel()
            {
                Price = payment.GetPlanPrice(planId)
            });
        }

        [HttpPost]
        public ActionResult Payment(PaymentInformationModel data)
        {
            if (CurrentUser.Payment != null)
            {
                return RedirectToAction<UserController>(c => c.Index());
            }
            if (ModelState.IsValid)
            {
                //var subscription = SubscriptionRequest.CreateMonthly(CurrentUser.Email, "Hatley",
                //    AuthorizeNetConfigurationSection.Price);
                //subscription.CardNumber = data.CartNumber;
                //subscription.CardCode = data.CardCode;
                //subscription.CardExpirationMonth = (int)data.ExpirationMonth;
                //subscription.CardExpirationYear = (int)data.ExpirationYear;
                //subscription.StartsOn = DateTime.UtcNow;
                //subscription.WithBillingAddress(new Address()
                //{
                //    First = data.FirstName,
                //    Last = data.LastName,
                //    Phone = CurrentUser.PhoneNumber
                //});

                //ISubscriptionRequest payment = null;
                CustomerInfo info = new CustomerInfo
                {
                    CardNumber = data.CartNumber,
                    Cvc = data.CardCode,
                    Email = CurrentUser.Email,
                    ExpMonth = data.ExpirationMonth.ToString(),
                    ExpYear = data.ExpirationYear.ToString(),
                    FullName = CurrentUser.FullName
                };
                try
                {
                    payment.CreateSubscription(info, planId);
                    CurrentUser.Payment = new Payment
                    {
                        PaymentId = payment.PaymentId,
                        PaymentDateUTC = DateTime.UtcNow,
                        CardNumber = data.CartNumber.Substring(data.CartNumber.Length-4)
                    };
                }
                catch (Exception e)
                {
                    logger.Error("Error while saving payment", e);
                    ModelState.AddModelError("", "Invalid payment information!");
                    return View(data);
                }

                
                UnitOfWork.Commit();
                return RedirectToAction<UserController>(c => c.Index());
            }
            return View(data);
        }

        public ActionResult CancelPayment()
        {
            if (CurrentUser.Payment != null)
            {
                //gate.CancelSubscription(CurrentUser.Payment.PaymentId);
                payment.PaymentId = CurrentUser.Payment.PaymentId;
                payment.CancelPayment(CurrentUser.Email);
                UnitOfWork.PaymentRepository.Delete(CurrentUser.Id);
                UnitOfWork.Commit();
            }
            return RedirectToAction<AuthorizeNetController>(c => c.Payment());
        }
    }
}