using Stripe;
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
        Task<Dtos.ResponsePaymentMethod> GetPaymentMethodAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<Dtos.ResponseSubscription> CreateAsync(Guid companyId, Dtos.CreateSubscription subscription, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid companyId, Dtos.UpdateSubscription subscription, CancellationToken cancellationToken = default);
        Task<Dtos.ResponseSubscription> UpgradeAsync(Guid companyId, Dtos.UpgradeSubscription subscription, CancellationToken cancellationToken = default);
        Task CancelAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default);
        Task<Dtos.ResponseSubscription> GetAsync(Guid companyId, string subscriptionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Dtos.ResponseSubscription>> GetAllAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<Dtos.Customer> CreateCustomerAsync(Guid companyId, Dtos.Customer customer, CancellationToken cancellationToken = default);
        Task<IEnumerable<Dtos.Price>> GetPricesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Dtos.Invoice>> GetInvoicesAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Dtos.ResponsePaymentIntent>> GetPaymentIntentsAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task Events(Stripe.Event events, CancellationToken cancellationToken = default);
    }
}
