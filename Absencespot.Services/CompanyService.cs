using Absencespot.Business.Abstractions;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.UnitOfWork;
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
        public async Task<Company> CreateAsync(Guid subscriptionId, Dtos.Company companyDto)
        {
            if(subscriptionId == default)
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }
            if(companyDto == null)
            {
                throw new ArgumentNullException(nameof(companyDto));
            }
            companyDto.EnsureValidation();

            var subscription = await _unitOfWork.SubscriptionRepository.FindByGlobalIdAsync(subscriptionId);
            if(subscription == null)
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

        public Task DeleteAsync(Guid companyId)
        {
            throw new NotImplementedException();
        }

        public Task<Company> GetByIdAsync(Guid companyId)
        {
            throw new NotImplementedException();
        }

        public Task<Company> UpdateAsync(Company company)
        {
            throw new NotImplementedException();
        }
    }
}
