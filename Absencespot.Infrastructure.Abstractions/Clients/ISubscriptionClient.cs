﻿using Absencespot.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface ISubscriptionClient<T, D, B, C> where T : class where D : class where B : class where C : class
    {
        Task<T> CreateAsync(Guid companyId, D subscription, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid companyId, B subscription, CancellationToken cancellationToken = default);
        Task<T> UpgradeAsync(Guid companyId, C subscription, CancellationToken cancellationToken = default);
        Task CancelAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default);    
        Task<IEnumerable<T>> ListAll(Guid companyId, string customerId, CancellationToken cancellationToken = default);    
    }
}
