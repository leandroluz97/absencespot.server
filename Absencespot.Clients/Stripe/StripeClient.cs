using Absencespot.Dtos;
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
            };

            var options = new Stripe.SubscriptionCreateOptions
            {
                Customer = subscription.CustomerId,
                Items = new List<Stripe.SubscriptionItemOptions>
                {
                    new Stripe.SubscriptionItemOptions 
                    { 
                        Price = subscription.PriceId, 
                        Quantity = 3  
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
            var stripePrices = await priceService.ListAsync(cancellationToken: cancellationToken);
            return stripePrices;
        }

        public async Task<IEnumerable<Stripe.Subscription>> ListAll(Guid companyId, string customerId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            var options = new Stripe.SubscriptionSearchOptions
            {
                Query = $"status:'active' AND customer:'{customerId}'",
            };
            var subscriptionService = new Stripe.SubscriptionService();
            var stripeSubscriptions = await subscriptionService.SearchAsync(options, cancellationToken: cancellationToken);
  
            return stripeSubscriptions;
        }

        public string GetPublishableKeyAsync(CancellationToken cancellationToken = default)
        {
            return _publicKey;
        }
    }
}
