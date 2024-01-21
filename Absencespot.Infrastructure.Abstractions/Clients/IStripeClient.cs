using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe;


namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface IStripeClient : ISubscriptionClient<Stripe.Subscription>
    {
        Task<Dtos.ResponsePublishableKey> GetPublishableKeyAsync(CancellationToken cancellationToken = default);
        Task<Stripe.Customer> CreateCustomerAsync<D>(Guid companyId, Dtos.Customer customer, CancellationToken cancellationToken = default);
        Task<IEnumerable<Stripe.Price>> GetPricesAsync(CancellationToken cancellationToken = default);
        Task Events<D>(D intent, CancellationToken cancellationToken = default);    
    }
}
