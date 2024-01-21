using Absencespot.Business.Abstractions;
using Absencespot.Dtos;
using Absencespot.Services.Exceptions;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IConfiguration _configuration;
        public SubscriptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Dtos.ResponseSubscription> GetAsync(Guid companyId, string customerId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            var options = new SubscriptionSearchOptions
            {
                Query = "status:'active' AND customer:'customerId'",
            };
            var subscriptionService = new Stripe.SubscriptionService();

            var stripeSubscriptions = await subscriptionService.SearchAsync(options, cancellationToken: cancellationToken);
            var stripeSubscription = stripeSubscriptions.FirstOrDefault();

            if (stripeSubscription == null)
            {
                return null;
            }

            return new Dtos.ResponseSubscription()
            {
                Id = stripeSubscription.Id,
            };
        }

        public async Task CancelAsync(Guid companyId, string subscriptionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            var subscriptionService = new Stripe.SubscriptionService();
            await subscriptionService.CancelAsync(subscriptionId, cancellationToken: cancellationToken);
        }

        public async Task CreateAsync(Guid companyId, Dtos.CreateSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            var options = new SubscriptionCreateOptions
            {
                Customer = subscription.CustomerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions { Price = subscription.PriceId },
                },
            };
            var service = new Stripe.SubscriptionService();
            await service.CreateAsync(options, cancellationToken: cancellationToken);
        }

        public async Task<Dtos.Customer> CreateCustomerAsync(Guid companyId, Dtos.Customer customer, CancellationToken cancellationToken = default)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            customer.EnsureValidation();

            var options = new CustomerCreateOptions
            {
                Name = customer.Name,
                Email = customer.Email,
                PaymentMethod = "card"
            };
            var service = new CustomerService();
            var stripeCustomer = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return new Dtos.Customer()
            {
                Id = stripeCustomer.Id,
                Name = stripeCustomer.Name,
                Email = stripeCustomer.Email,
            };
        }

        public async Task<Dtos.ResponsePaymentIntent> CreatePaymentIntentAsync(Guid companyId, Dtos.CreatePaymentIntent intent, CancellationToken cancellationToken = default)
        {
            if (intent == null)
            {
                throw new ArgumentNullException(nameof(intent));
            }
            intent.EnsureValidation();

            var priceService = new PriceService();
            var stripePrice = await priceService.GetAsync(intent.PriceId, cancellationToken: cancellationToken);
            if (stripePrice == null)
            {
                throw new NotFoundException(nameof(stripePrice));
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = stripePrice.UnitAmount * 1,
                Customer = intent.CustomerId,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var paymentService = new PaymentIntentService();
            var paymentIntent = await paymentService.CreateAsync(options, cancellationToken: cancellationToken);

            return new Dtos.ResponsePaymentIntent()
            {
                ClientSecret = paymentIntent.ClientSecret
            };
        }

        public async Task Events(Dtos.CreatePaymentIntent intent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Dtos.Price>> GetPricesAsync(CancellationToken cancellationToken = default)
        {
            var priceService = new PriceService();
            var stripePrices = await priceService.SearchAsync(cancellationToken: cancellationToken);
            if (stripePrices == null)
            {
                throw new NotFoundException(nameof(stripePrices));
            }

            return stripePrices.Select(p => new Dtos.Price()
            {
                Id = p.Id,
                UnitAmount = (decimal)p?.UnitAmountDecimal,
            });
        }

        public async Task<Dtos.ResponsePublishableKey> GetPublishableKeyAsync(CancellationToken cancellationToken = default)
        {
            return new Dtos.ResponsePublishableKey()
            {
                Key = _configuration["Stripe:PublishableKey"]!   
            };
        }

    }
}
