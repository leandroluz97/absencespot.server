using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Business.Abstractions
{
    public interface ISubscriptionService
    {
        Task<Dtos.ResponsePublishableKey> GetPublishableKeyAsync(CancellationToken cancellationToken = default);
        Task CreateAsync(Guid companyId, Dtos.CreateSubscription subscription, CancellationToken cancellationToken = default);
        Task CancelAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default);
        Task<Dtos.ResponseSubscription> GetAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default);
        Task<Dtos.Customer> CreateCustomerAsync(Guid companyId, Dtos.Customer customer, CancellationToken cancellationToken = default);
        Task<IEnumerable<Dtos.Price>> GetPricesAsync(CancellationToken cancellationToken = default);
        Task<Dtos.ResponsePaymentIntent> CreatePaymentIntentAsync(Guid companyId, Dtos.CreatePaymentIntent intent, CancellationToken cancellationToken = default);
        Task Events(Dtos.CreatePaymentIntent intent, CancellationToken cancellationToken = default);
    }
}
