﻿using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions.Clients;
using Microsoft.Extensions.Configuration;

namespace Absencespot.Clients
{
    public class StripeClient : IStripeClient
    {
        private readonly string _publicKey;

        public StripeClient(IConfiguration configuration)
        {
            _publicKey = configuration["Stripe:PublishableKey"]!;
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

        public async Task<Stripe.Subscription> CreateAsync(Guid companyId, CreateSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            // Automatically save the payment method to the subscription
            // when the first payment is successful.
            var paymentSettings = new Stripe.SubscriptionPaymentSettingsOptions
            {
                SaveDefaultPaymentMethod = "on_subscription",
                PaymentMethodTypes = new List<string>() { "card" }
            };

            var options = new Stripe.SubscriptionCreateOptions
            {
                Customer = subscription.CustomerId,
                Items = new List<Stripe.SubscriptionItemOptions>
                {
                    new Stripe.SubscriptionItemOptions
                    {
                        Price = subscription.PriceId,
                        Quantity = 1
                    },
                },
                PaymentSettings = paymentSettings,
                PaymentBehavior = "default_incomplete",
            };

            options.AddExpand("latest_invoice.payment_intent");

            var subscriptionService = new Stripe.SubscriptionService();
            var stripeSubscribe = await subscriptionService.CreateAsync(options, cancellationToken: cancellationToken);

            return stripeSubscribe;
        }

        public async Task UpdateAsync(Guid companyId, UpdateSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            var options = new Stripe.SubscriptionItemUpdateOptions
            {
                Price = subscription.PriceId,
                Quantity = subscription.Quantity
            };

            var subscriptionService = new Stripe.SubscriptionItemService();
            var stripeSubscribe = await subscriptionService.UpdateAsync(subscription.subscriptionItemId, options, cancellationToken: cancellationToken);
        }

        public async Task<Stripe.Subscription> UpgradeAsync(Guid companyId, UpgradeSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            var options = new Stripe.SubscriptionUpdateOptions
            {
                Items = new List<Stripe.SubscriptionItemOptions>
                {
                    new Stripe.SubscriptionItemOptions
                    {
                        Id = subscription.SubscriptionItemId,
                        Deleted = true
                    },
                    new Stripe.SubscriptionItemOptions
                    {
                        Price = subscription.PriceId,
                        Quantity = subscription.Quantity
                    },
                },
            };

            var subscriptionService = new Stripe.SubscriptionService();
            var stripeSubscribe = await subscriptionService.UpdateAsync(subscription.SubscriptionId, options, cancellationToken: cancellationToken);

            return stripeSubscribe;
        }

        public async Task<Stripe.Customer> CreateCustomerAsync(Guid companyId, Customer customer, CancellationToken cancellationToken = default)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            customer.EnsureValidation();

            var options = new Stripe.CustomerCreateOptions
            {
                Name = customer.Name,
                Email = customer.Email,
                PreferredLocales = new List<string> { "en" },
            };
            var service = new Stripe.CustomerService();
            var stripeCustomer = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return stripeCustomer;
        }

        public Task Events<D>(D intent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Stripe.Subscription?> GetByIdAsync(Guid companyId, string subscriptionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            var subscriptionService = new Stripe.SubscriptionService();
            var stripeSubscription = await subscriptionService.GetAsync(subscriptionId, cancellationToken: cancellationToken);

            return stripeSubscription;
        }

        public async Task<IEnumerable<Stripe.Price>> GetPricesAsync(CancellationToken cancellationToken = default)
        {
            var priceService = new Stripe.PriceService();
            var productService = new Stripe.ProductService();
            var options = new Stripe.PriceListOptions
            {
                Expand = new List<string> { "data.tiers" } // Expand the price field
            };

            var stripePrices = await priceService.ListAsync(options, cancellationToken: cancellationToken);

            List<Stripe.Price> prices = new List<Stripe.Price>();
            foreach (var price in stripePrices)
            {
                var product = await productService.GetAsync(price.ProductId);
                if (product == null)
                {
                    continue;
                }
                price.Product = product;
                prices.Add(price);
            }
            return prices;
        }

        public async Task<IEnumerable<Stripe.PaymentIntent>> GetPaymentIntentsAsync(string customerId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            var options = new Stripe.PaymentIntentListOptions
            {
                Customer = customerId,
                Limit = 100,
                Expand = new List<string> { "data.invoice" }
            };

            var paymentIntentService = new Stripe.PaymentIntentService();
            var stripePaymentIntents = await paymentIntentService.ListAsync(options, cancellationToken: cancellationToken);

            var invoiceService = new Stripe.InvoiceService();

            List<Stripe.PaymentIntent> paymentIntents = new List<Stripe.PaymentIntent>();
            foreach (var paymentIntent in stripePaymentIntents)
            {
                if (string.IsNullOrWhiteSpace(paymentIntent.InvoiceId))
                {
                    continue;
                }
                var invoice = await invoiceService.GetAsync(paymentIntent.InvoiceId);
                if (invoice == null)
                {
                    continue;
                }
                paymentIntent.Invoice = invoice;
                paymentIntents.Add(paymentIntent);
            }

            return paymentIntents;
        }

        public async Task<IEnumerable<Stripe.Subscription>> ListAll(Guid companyId, string customerId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            var options = new Stripe.SubscriptionListOptions
            {
                Customer = customerId,
                Status = "active",
                Expand = new List<string>(){ "data.latest_invoice.payment_intent" }
            };

            var subscriptionService = new Stripe.SubscriptionService();
            var stripeSubscriptions = await subscriptionService.ListAsync(options, cancellationToken: cancellationToken);

            return stripeSubscriptions;
        }

        public string GetPublishableKeyAsync(CancellationToken cancellationToken = default)
        {
            return _publicKey;
        }
    }
}
