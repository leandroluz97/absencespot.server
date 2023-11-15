using Absencespot.Business.Abstractions;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions;
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
        public CompanyService(ILogger<CompanyService> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Company> CreateAsync(Dtos.Company companyDto, CancellationToken cancellationToken = default)
        {
            if (companyDto == null)
            {
                throw new ArgumentNullException(nameof(companyDto));
            }
            companyDto.EnsureValidation();

            var subscription = await _unitOfWork.SubscriptionRepository.FindByGlobalIdAsync(
                companyDto.SubscriptionId, 
                RepositoryOptions.AsTracking(), 
                cancellationToken);

            if (subscription == null)
            {
                throw new NotFoundException($"Could not find {nameof(subscription)}");
            }

            var companyDomain = CompanyMapper.ToDomain(companyDto);
            companyDomain.Subcription = subscription;

            companyDomain = _unitOfWork.CompanyRepository.Add(companyDomain);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created company by Id:");

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
            if(companyId == default)
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
