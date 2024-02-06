using Absencespot.Business.Abstractions;
using Absencespot.Domain.Enums;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Infrastructure.Abstractions.Clients;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.UnitOfWork;
using Absencespot.Utils;
using Microsoft.Extensions.Logging;

namespace Absencespot.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ILogger<CompanyService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeClient _stripeClient;
        public CompanyService(ILogger<CompanyService> logger, IUnitOfWork unitOfWork, IStripeClient stripeClient)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _stripeClient = stripeClient;
        }
        public async Task<Company> CreateAsync(Dtos.Company companyDto, CancellationToken cancellationToken = default)
        {
            if (companyDto == null)
            {
                throw new ArgumentNullException(nameof(companyDto));
            }
            companyDto.EnsureValidation();

            var stripePrices = await _stripeClient.GetPricesAsync();
            if (stripePrices == null)
            {
                throw new NotFoundException($"Could not find price Id: {companyDto.PlanId}");
            }

            var chosenPrice = stripePrices.FirstOrDefault(p => p.Id == companyDto.PlanId);
            if (chosenPrice == null)
            {
                throw new NotFoundException($"Could not find price Id: {companyDto.PlanId}");
            }

            var customer = new Customer()
            {
                Email = companyDto.EmailContact,
                Name = companyDto.Name,
            };
            var stripeCustomer = await _stripeClient.CreateCustomerAsync(customer);

            var subscription = new CreateSubscription()
            {
                CustomerId = stripeCustomer.Id,
                PriceId = companyDto.PlanId,
            };
            var stripeSubscription = await _stripeClient.CreateAsync(subscription);

            var companyDomain = CompanyMapper.ToDomain(companyDto);
            companyDomain.CustomerId = customer.Id;
            companyDomain = _unitOfWork.CompanyRepository.Add(companyDomain);

            SubscriptionType subscriptionType = SubscriptionType.Free;
            if (chosenPrice.Product.Metadata["Identifier"] == "business")
            {
                subscriptionType = SubscriptionType.Business;
            }
            else if (chosenPrice.Product.Metadata["Identifier"] == "enterprise")
            {
                subscriptionType = SubscriptionType.Enterprise;
            }

            var tier = chosenPrice.Tiers.FirstOrDefault();
            if (tier == null)
            {
                throw new NullReferenceException();
            }

            int startNumberOfUsers = 1;
            decimal unitAmount = (decimal)tier.UnitAmount!;
            var subscriptionDomain = new Domain.Subscription()
            {
                Type = subscriptionType,
                SubscriptionId = stripeSubscription.Id,
                Quantity = startNumberOfUsers,
                UnitPrice = unitAmount,
                Company = companyDomain,
            };  

            _unitOfWork.SubscriptionRepository.Add(subscriptionDomain);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Created company by Id: {companyDomain.GlobalId}");

            return CompanyMapper.ToDto(companyDomain);
        }

        public Task DeleteAsync(Guid companyId, CancellationToken cancellationToke)
        {
            throw new NotImplementedException();
        }

        public async Task<Dtos.Company> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var companyDomain = await LoadByIdAsync(companyId, cancellationToken);

            _logger.LogInformation($"Found company Id:{companyId}");

            return CompanyMapper.ToDto(companyDomain);
        }

        public async Task<Dtos.Company> UpdateAsync(Guid companyId, Company companyDto, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (companyDto == null)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            companyDto.EnsureValidation();

            var companyDomain = await LoadByIdAsync(companyId, cancellationToken);

            companyDomain.Name = companyDto.Name;
            companyDomain.FiscalNumber = companyDto.Name;
            companyDomain.Industry = companyDto.Industry;
            companyDomain.EmailContact = companyDto.EmailContact;

            _logger.LogInformation($"Updated company Id:{companyId}");

            return CompanyMapper.ToDto(companyDomain);
        }

        private async Task<Domain.Company> LoadByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
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
