using Absencespot.Business.Abstractions;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Infrastructure.Abstractions.Clients;
using Absencespot.Services.Exceptions;
using Microsoft.Extensions.Configuration;
using Stripe;


namespace Absencespot.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Infrastructure.Abstractions.Clients.IStripeClient _stripeClient;
        public SubscriptionService(IConfiguration configuration, Infrastructure.Abstractions.Clients.IStripeClient stripeClient, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _stripeClient = stripeClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Dtos.ResponseSubscription>> GetAllAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var companyDomain = await LoadCompanyByIdAsync(companyId);

            var stripeSubscription = await _stripeClient.ListAll(companyDomain.CustomerId!, cancellationToken);

            if (stripeSubscription == null)
            {
                throw new NotFoundException(nameof(stripeSubscription));
            }

            return stripeSubscription.Select(s =>
                new Dtos.ResponseSubscription()
                {
                    Id = s.Id,
                    ClientSecret = s?.LatestInvoice?.PaymentIntent?.ClientSecret,
                    Ids = s?.Items?.Select(i => i.Id),
                    PriceId = s?.Items?.Select(i => i.Price.Id).FirstOrDefault()!,
                    Status = s.Status,
                    PaymentMethodId = s?.DefaultPaymentMethodId,
                    CanceledAt = s?.CanceledAt,
                }
            );
        }

        public async Task<Dtos.ResponseSubscription> GetAsync(Guid companyId, string subscriptionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            var stripeSubscription = await _stripeClient.GetByIdAsync(subscriptionId, cancellationToken);

            if (stripeSubscription == null)
            {
                throw new NotFoundException(nameof(stripeSubscription));
            }

            return new Dtos.ResponseSubscription()
            {
                Id = stripeSubscription.Id,
                ClientSecret = stripeSubscription.LatestInvoice.PaymentIntent.ClientSecret,
                Ids = stripeSubscription.Items.Select(i => i.Id),
                PriceId = stripeSubscription.Items.Select(i => i.Price.Id).FirstOrDefault()!,
                Status = stripeSubscription.Status,
                PaymentMethodId = stripeSubscription.DefaultPaymentMethodId,
                CanceledAt = stripeSubscription.CanceledAt,
            };
        }

        public async Task CancelAsync(Guid companyId, string subscriptionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            await _stripeClient.CancelAsync(subscriptionId, cancellationToken);
        }

        public async Task<Dtos.ResponseSubscription> CreateAsync(Guid companyId, Dtos.CreateSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            var stripeSubscription = await _stripeClient.CreateAsync(subscription, cancellationToken);
            if (stripeSubscription == null)
            {
                throw new InvalidOperationException();
            }

            return new Dtos.ResponseSubscription()
            {
                Id = stripeSubscription.Id,
                ClientSecret = stripeSubscription.LatestInvoice.PaymentIntent.ClientSecret,
                Ids = stripeSubscription.Items.Select(i => i.Id),
                PriceId = stripeSubscription.Items.Select(i => i.Price.Id).FirstOrDefault()!,
                Status = stripeSubscription.Status,
                PaymentMethodId = stripeSubscription.DefaultPaymentMethodId,
                CanceledAt = stripeSubscription.CanceledAt,
            };
        }

        public async Task UpdateAsync(Guid companyId, Dtos.UpdateSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            await _stripeClient.UpdateAsync(subscription, cancellationToken);
        }

        public async Task<Dtos.ResponseSubscription> UpgradeAsync(Guid companyId, Dtos.UpgradeSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            var stripeSubscription = await _stripeClient.UpgradeAsync(subscription, cancellationToken);
            if (stripeSubscription == null)
            {
                throw new InvalidOperationException();
            }

            return new Dtos.ResponseSubscription()
            {
                Id = stripeSubscription.Id,
                ClientSecret = stripeSubscription.LatestInvoice.PaymentIntent.ClientSecret,
                Ids = stripeSubscription.Items.Select(i => i.Id),
                PriceId = stripeSubscription.Items.Select(i => i.Price.Id).FirstOrDefault()!,
                Status = stripeSubscription.Status,
                PaymentMethodId = stripeSubscription.DefaultPaymentMethodId,
                CanceledAt = stripeSubscription.CanceledAt,
            };
        }

        public async Task<Dtos.Customer> CreateCustomerAsync(Guid companyId, Dtos.Customer customer, CancellationToken cancellationToken = default)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            customer.EnsureValidation();

            var stripeCustomer = await _stripeClient.CreateCustomerAsync(customer, cancellationToken);
            if (stripeCustomer == null)
            {
                throw new InvalidOperationException(nameof(stripeCustomer));
            }

            return new Dtos.Customer()
            {
                Id = stripeCustomer.Id,
                Name = stripeCustomer.Name,
                Email = stripeCustomer.Email,
            };
        }

        public async Task<IEnumerable<Dtos.ResponsePaymentIntent>> GetPaymentIntentsAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var companyDomain = await LoadCompanyByIdAsync(companyId);

            var paymentHistory = await _stripeClient.GetPaymentIntentsAsync(companyDomain.CustomerId!);

            return paymentHistory.Select(p => new Dtos.ResponsePaymentIntent()
            {
                Lines = p.Invoice.Lines.Select(x => new { Quantity = x.Quantity.Value, priceId = x.Price.Id })!,
                Amount = p.Amount,
                Currency = p.Currency,
                PaymentMethod = p.PaymentMethodTypes,
                Tax = p.Invoice?.Tax,
                CreatedAt = p.Invoice?.Created,
                DueDate = p.Invoice?.DueDate,
                InvoiceNumber = p.Invoice?.Number,
                Total = p.Invoice?.Total,
                TotalExcludingTax = p.Invoice?.TotalExcludingTax,
                Status = p.Invoice?.Status,
            });
        }

        public async Task<IEnumerable<Dtos.Price>> GetPricesAsync(CancellationToken cancellationToken = default)
        {
            var stripePrices = await _stripeClient.GetPricesAsync(cancellationToken);
            if (stripePrices == null)
            {
                throw new NotFoundException(nameof(stripePrices));
            }

            return stripePrices.Select(p => new Dtos.Price()
            {
                Id = p.Id,
                UnitAmount = (decimal)p.Tiers.FirstOrDefault()?.UnitAmount!,
                Product = new Dtos.Product()
                {
                    Description = p.Product?.Description,
                    Metadata = p.Product?.Metadata,
                    Name = p.Product.Name,
                },
                Currency = p.Currency,
            });
        }

        public async Task<Dtos.Price> GetPriceByIdAsync(string priceId, CancellationToken cancellationToken = default)
        {
            if (priceId == null)
            {
                throw new ArgumentNullException(nameof(priceId));
            }

            var price = await _stripeClient.GetPriceByIdAsync(priceId);
            if (price == null)
            {
                throw new NotFoundException(nameof(price));
            }

            return new Dtos.Price()
            {
                Id = price.Id,
                UnitAmount = (decimal)price.Tiers.FirstOrDefault()?.UnitAmount!,
                Product = new Dtos.Product()
                {
                    Description = price.Product?.Description,
                    Metadata = price.Product?.Metadata,
                    Name = price.Product.Name,
                },
                Currency = price.Currency,
            };
        }

        public async Task<IEnumerable<Dtos.Invoice>> GetInvoicesAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var companyDomain = await LoadCompanyByIdAsync(companyId);

            var stripeInvoices = await _stripeClient.GetInvoicesAsync(companyDomain.CustomerId!, cancellationToken);

            if (stripeInvoices == null)
            {
                throw new NotFoundException(nameof(stripeInvoices));
            }

            return stripeInvoices.Select(i => new Dtos.Invoice()
            {
                Status = i.Status,
                Currency = i.Currency,
                CreatedAt = i.Created,
                DueDate = i.DueDate,
                Description = i.Description,
                Number = i.Number,
                Subtotal = i.Subtotal,
                SubtotalExcludingTax = i.SubtotalExcludingTax,
                Tax = i.Tax,
                Total = i.Total,
                TotalExcludingTax = i.TotalExcludingTax,
                //TotalTaxAmounts = i.TotalTaxAmounts,
                PaymentMethodBrand = i.PaymentIntent?.PaymentMethod?.Card?.Brand,
                //TaxRates = i.DefaultTaxRates             
            });
        }

        public async Task<Dtos.ResponsePublishableKey> GetPublishableKeyAsync(CancellationToken cancellationToken = default)
        {
            var publicKey = _stripeClient.GetPublishableKeyAsync(cancellationToken);
            return new Dtos.ResponsePublishableKey()
            {
                Key = publicKey
            };
        }

        public async Task<Dtos.ResponsePaymentMethod> GetPaymentMethodAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var companyDomain = await LoadCompanyByIdAsync(companyId);

            var stripeSubscriptions = await _stripeClient.ListAll(companyDomain.CustomerId!, cancellationToken);
            if (stripeSubscriptions == null || !stripeSubscriptions.Any())
            {
                throw new NotFoundException(nameof(stripeSubscriptions));
            }

            var stripeSubscription = stripeSubscriptions.ElementAt(0);

            var stripePaymentMethod = await _stripeClient.GetPaymentMethodAsync(stripeSubscription.DefaultPaymentMethodId!, cancellationToken);
            if (stripePaymentMethod == null)
            {
                throw new NotFoundException(nameof(stripePaymentMethod));
            }

            var today = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var expirationDate = new DateTime((int)stripePaymentMethod.Card.ExpYear, (int)stripePaymentMethod.Card.ExpMonth, 1);

            return new Dtos.ResponsePaymentMethod()
            {
                Id = stripePaymentMethod.Id,
                Brand = stripePaymentMethod.Card.Brand,
                IsExpired = today > expirationDate,
                Number = stripePaymentMethod.Card.Last4,
            };
        }

        public async Task<Dtos.ResponseSetupIntent> CreateSetupIntentAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var companyDomain = await LoadCompanyByIdAsync(companyId);

            var setupIntent = await _stripeClient.CreateSetupIntentAsync(companyDomain.CustomerId!);
            if (setupIntent == null)
            {
                throw new ArgumentNullException(nameof(setupIntent));
            }

            return new Dtos.ResponseSetupIntent()
            {
                ClientSecret = setupIntent.ClientSecret,
            };
        }
        public async Task AttachCustomerPaymentMethodAsync(Guid companyId, string paymentMethodId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
            {
                throw new ArgumentNullException(nameof(paymentMethodId));
            }
            var companyDomain = await LoadCompanyByIdAsync(companyId);

            await _stripeClient.AttachCustomerPaymentMethodAsync(companyDomain.CustomerId!, paymentMethodId);

        }

        private async Task<Domain.Company> LoadCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }
            return companyDomain;
        }

        public async Task Events(Stripe.Event events, CancellationToken cancellationToken = default)
        {
            //https://github.com/stripe-samples/subscription-use-cases/blob/main/usage-based-subscriptions/client/script.js
            PaymentIntent? paymentIntent;
            switch (events.Type)
            {
                case Stripe.Events.InvoicePaymentSucceeded:
                    paymentIntent = events.Data.Object as PaymentIntent;
                    Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
                    // handlePaymentIntentSucceeded(paymentIntent);
                    break;
                case Stripe.Events.InvoicePaymentFailed:
                    paymentIntent = events.Data.Object as PaymentIntent;
                    Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
                    // handlePaymentIntentSucceeded(paymentIntent);
                    break;
                default:
                    break;
            }

            throw new NotImplementedException();
        }

    }
}
