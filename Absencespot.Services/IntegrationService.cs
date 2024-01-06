using Absencespot.Business.Abstractions;
using Absencespot.Domain.Enums;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Absencespot.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly ILogger<IntegrationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Domain.User> _userManager;

        public IntegrationService(ILogger<IntegrationService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Dtos.Integration>> GetAllAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            Guid userId = new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2");

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var companyDomain = await LoadCompanyByIdAsync(companyId, RepositoryOptions.AsTracking(), cancellationToken);
            var userDomain = await LoadUserByIdAsync(companyId, userId);

            var queryable = _unitOfWork.IntegrationRepository.AsQueryable();
            queryable = queryable.Where(x => x.CompanyId == companyDomain.Id);
            var integrations = await _unitOfWork.IntegrationRepository.ToListAsync(queryable, cancellationToken);

            if (!integrations.Any())
            {
                List<ProviderType> providerTypes = new List<ProviderType>() { ProviderType.Microsoft, ProviderType.Google };
                foreach (var provider in providerTypes)
                {
                    var integration = await CreateAsync(companyDomain, provider);
                    integrations ??= new List<Domain.Integration>();
                    integrations.ToList().Add(integration);
                }

                _unitOfWork.IntegrationRepository.AddRange(integrations);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            //if(userDomain.Role != "Admin")
            //{
            //    throw new UnauthorizedAccessException();
            //}

            return integrations.Select(IntegrationMapper.ToDto);
        }

        public async Task<Dtos.Integration> UpdateAsync(Guid companyId, Dtos.Integration integration, CancellationToken cancellationToken = default)
        {
            Guid userId = new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2");

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var companyDomain = await LoadCompanyByIdAsync(companyId,cancellationToken: cancellationToken) ;
            var userDomain = await LoadUserByIdAsync(companyId, userId);

            var options = RepositoryOptions.AsTracking();
            var queryable = _unitOfWork.IntegrationRepository.AsQueryable(options);
            queryable = queryable.Where(x => x.CompanyId == companyDomain.Id && x.Provider == integration.Provider);

            var integrationDomain = await _unitOfWork.IntegrationRepository.FirstOrDefaultAsync(queryable, cancellationToken);
            if(integrationDomain == null)
            {
                throw new NotFoundException(nameof(integrationDomain));
            }

            //if(userDomain.Role != "Admin")
            //{
            //    throw new UnauthorizedAccessException();
            //}

            integrationDomain.IsActive = integration.IsActive;

            _unitOfWork.IntegrationRepository.Update(integrationDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return IntegrationMapper.ToDto(integrationDomain);
        }

        private async Task<Domain.User> LoadUserByIdAsync(Guid companyId, Guid userId)
        {
            var queryable = _userManager.Users.AsQueryable();
            queryable = queryable.Where(u => u.Company.GlobalId == companyId && u.GlobalId == userId);
            var user = queryable.ToList().FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException(nameof(userId));
            }

            return user;
        }

        private async Task<Domain.Company> LoadCompanyByIdAsync(Guid companyId, RepositoryOptions options = null, CancellationToken cancellationToken = default)
        {
            Domain.Company? companyDomain;
            if (options == null)
            {
                companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            }
            else
            {
                companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, options, cancellationToken: cancellationToken);
            }

            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            return companyDomain;
        }

        public async Task<Domain.Integration> CreateAsync(Domain.Company company, ProviderType provider, CancellationToken cancellationToken = default)
        {
            var integration = new Dtos.Integration()
            {
                IsActive = false,
                Provider = provider,
            };
            var integrationDomain = IntegrationMapper.ToDomain(integration);
            integrationDomain.Company = company;

            integrationDomain = _unitOfWork.IntegrationRepository.Add(integrationDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return integrationDomain;
        }

    }
}
