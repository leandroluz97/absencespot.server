﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Absencespot.Dtos;
using Stripe;


namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface IStripeClient : ISubscriptionClient<Stripe.Subscription, CreateSubscription>
    {
        public string GetPublishableKeyAsync(CancellationToken cancellationToken = default);
        Task<Stripe.Customer> CreateCustomerAsync(Guid companyId, Dtos.Customer customer, CancellationToken cancellationToken = default);
        Task<IEnumerable<Stripe.Price>> GetPricesAsync(CancellationToken cancellationToken = default);
        Task Events<D>(D intent, CancellationToken cancellationToken = default);    
    }
}