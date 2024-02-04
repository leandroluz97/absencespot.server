using Absencespot.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface ISubscriptionClient<T, D, B, C> where T : class where D : class where B : class where C : class
    {
        Task<T> CreateAsync(D subscription, CancellationToken cancellationToken = default);
        Task UpdateAsync(B subscription, CancellationToken cancellationToken = default);
        Task<T> UpgradeAsync(C subscription, CancellationToken cancellationToken = default);
        Task CancelAsync(string customerId, CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(string customerId, CancellationToken cancellationToken = default);    
        Task<IEnumerable<T>> ListAll(string customerId, CancellationToken cancellationToken = default);    
    }
}
