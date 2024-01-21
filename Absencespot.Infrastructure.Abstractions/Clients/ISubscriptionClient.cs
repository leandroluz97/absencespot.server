using Absencespot.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface ISubscriptionClient<T> where T : class
    {
        Task<T> CreateAsync<D>(Guid companyId, D subscription, CancellationToken cancellationToken = default);
        Task CancelAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default);    
    }
}
