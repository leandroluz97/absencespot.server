using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Absencespot.Dtos;
using Stripe;


namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface IStripeClient : ISubscriptionClient<Stripe.Subscription, CreateSubscription, UpdateSubscription, UpgradeSubscription>
    {
        public string GetPublishableKeyAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Stripe.Price>> GetPricesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Stripe.Invoice>> GetInvoicesAsync(string customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Stripe.PaymentIntent>> GetPaymentIntentsAsync(string customerId, CancellationToken cancellationToken = default);
        Task<Stripe.PaymentMethod> GetPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken = default);
        Task<Stripe.Customer> CreateCustomerAsync(Dtos.Customer customer, CancellationToken cancellationToken = default);
        Task Events<D>(D intent, CancellationToken cancellationToken = default);    
    }
}
