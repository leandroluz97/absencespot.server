using Absencespot.Infrastructure.Abstractions.Clients;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Clients.Stripe
{
    public class StripeClient: ISubscriptionClient
    {
        public StripeClient()
        {

        }

        public void CreateSubscription(int seat)
        {
            //var options = new SubscriptionCreateOptions
            //{
            //    Customer = "cus_4fdAW5ftNQow1a",
            //    Items = new List<SubscriptionItemOptions>
            //    {
            //        new SubscriptionItemOptions { Price = "price_1OZNFpA0D3nIAUzqndEcS9Mj", Quantity = seat },
            //    },
            //};
            //var service = new SubscriptionService();
            //service.Create(options);
            //var options = new PriceCreateOptions
            //{
            //    Nickname = "Standard Cost Per 5 Users",
            //    TransformQuantity = new PriceTransformQuantityOptions { DivideBy = 5, Round = "up" },
            //    UnitAmount = 1000,
            //    Currency = "usd",
            //    Recurring = new PriceRecurringOptions { Interval = "month", UsageType = "licensed" },
            //    Product = "{{PRODUCTIVITY_SUITE_ID}}",
            //};
            //var service = new PriceService();
            //service.Create(options);
        }
    }
}
