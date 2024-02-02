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

        public async Task<IEnumerable<Dtos.ResponseSubscription>> GetAllAsync(Guid companyId, string customerId, string priceId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }
            if (string.IsNullOrWhiteSpace(priceId))
            {
                throw new ArgumentNullException(nameof(priceId));
            }

            var stripeSubscription = await _stripeClient.ListAll(companyId, customerId, cancellationToken);

            if (stripeSubscription == null)
            {
                throw new NotFoundException(nameof(stripeSubscription));
            }

            return stripeSubscription.Select(s =>
                new Dtos.ResponseSubscription()
                {
                    Id = s.Id,
                    ClientSecret = s.LatestInvoice.PaymentIntent.ClientSecret,
                    PriceId = s.Items.Select(i => i.Price.Id).FirstOrDefault()!,
                    Ids = s.Items.Select(i => i.Id)
                });
        }

        public async Task<Dtos.ResponseSubscription> GetAsync(Guid companyId, string subscriptionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            var stripeSubscription = await _stripeClient.GetByIdAsync(companyId, subscriptionId, cancellationToken);

            if (stripeSubscription == null)
            {
                throw new NotFoundException(nameof(stripeSubscription));
            }

            return new Dtos.ResponseSubscription()
            {
                Id = stripeSubscription.Id,
                ClientSecret = stripeSubscription.LatestInvoice.PaymentIntent.ClientSecret,
                Ids = stripeSubscription.Items.Select(i => i.Id)
            };
        }

        public async Task CancelAsync(Guid companyId, string subscriptionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            await _stripeClient.CancelAsync(companyId, subscriptionId, cancellationToken);
        }

        public async Task<Dtos.ResponseSubscription> CreateAsync(Guid companyId, Dtos.CreateSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            var stripeSubscription = await _stripeClient.CreateAsync(companyId, subscription, cancellationToken);
            if (stripeSubscription == null)
            {
                throw new InvalidOperationException();
            }

            return new Dtos.ResponseSubscription()
            {
                Id = stripeSubscription.Id,
                ClientSecret = stripeSubscription.LatestInvoice.PaymentIntent.ClientSecret,
                Ids = stripeSubscription.Items.Select(i => i.Id)
            };
        }

        public async Task UpdateAsync(Guid companyId, Dtos.UpdateSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            await _stripeClient.UpdateAsync(companyId, subscription, cancellationToken);
        }

        public async Task<Dtos.ResponseSubscription> UpgradeAsync(Guid companyId, Dtos.UpgradeSubscription subscription, CancellationToken cancellationToken = default)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            subscription.EnsureValidation();

            var stripeSubscription = await _stripeClient.UpgradeAsync(companyId, subscription, cancellationToken);
            if (stripeSubscription == null)
            {
                throw new InvalidOperationException();
            }

            return new Dtos.ResponseSubscription()
            {
                Id = stripeSubscription.Id,
                Ids = stripeSubscription.Items.Select(i => i.Id),
                PriceId = stripeSubscription.Items.Select(i => i.Price.Id).FirstOrDefault()!
            };
        }

        public async Task<Dtos.Customer> CreateCustomerAsync(Guid companyId, Dtos.Customer customer, CancellationToken cancellationToken = default)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            customer.EnsureValidation();

            var stripeCustomer = await _stripeClient.CreateCustomerAsync(companyId, customer, cancellationToken);
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

            var paymentHistory = await _stripeClient.GetPaymentIntentsAsync(companyDomain.customerId!);

            return paymentHistory.Select(p => new Dtos.ResponsePaymentIntent()
            {
                Lines = p.Invoice.Lines.Select(x => new { Quantity = x.Quantity.Value, priceId = x.Price.Id})!,
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

        public async Task Events(Dtos.CreatePaymentIntent intent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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

        public async Task<IEnumerable<Dtos.Invoice>> GetInvoicesAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var companyDomain = await LoadCompanyByIdAsync(companyId);

            var stripeInvoices = await _stripeClient.GetInvoicesAsync(companyDomain.customerId!, cancellationToken);

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

    }
}
