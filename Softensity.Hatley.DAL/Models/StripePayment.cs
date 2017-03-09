using System.Collections.Generic;
using System.Linq;
using Stripe;

namespace Softensity.Hatley.DAL.Models
{
    public class StripePayment
    {
        public string PaymentId { get; set; }

        public decimal GetPlanPrice(string planId)
        {
            StripePlan plan = GetPlan(planId);
            decimal result = ((decimal) plan.Amount)/100;
            return result;
        }

        public void CreateSubscription(CustomerInfo customerInfo, string planId)
        {
            string customerId = GetCustomerId(customerInfo);
            StripeSubscriptionService subscriptionService = new StripeSubscriptionService();
            StripeSubscription stripeSubscription = subscriptionService.Create(customerId, planId);
            PaymentId = stripeSubscription.Id;
        }

        public void CancelPayment(string email)
        {
            StripeCustomerService customerService = new StripeCustomerService();
            IEnumerable<StripeCustomer> response = customerService.List();
            string customerId = response.FirstOrDefault(m => m.Email == email).Id;
            StripeSubscriptionService subscriptionService = new StripeSubscriptionService();
            subscriptionService.Cancel(customerId, PaymentId);
            customerService.Delete(customerId);
        }

        private StripePlan GetPlan(string planId)
        {
            StripePlanService service = new StripePlanService();
            StripePlan result = service.Get(planId);
            return result;
        }

        private StripeCustomerCreateOptions CustomerCreating(CustomerInfo model)
        {
            StripeCustomerCreateOptions customer = new StripeCustomerCreateOptions
            {
                Email = model.Email,
                Description = model.FullName,
                Source = new StripeSourceOptions
                {
                    Number = model.CardNumber,
                    ExpirationMonth = model.ExpMonth,
                    ExpirationYear = model.ExpYear,
                    Cvc = model.Cvc,
                    AddressCountry = "US",
                    Object = "card"
                },
            };
            return customer;
        }

        private string GetCustomerId(CustomerInfo model)
        {
            StripeCustomerCreateOptions newCustomer = CustomerCreating(model);
            StripeCustomerService customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Create(newCustomer);
            string result = stripeCustomer.Id;
            return result;
        }
        
    }
}
