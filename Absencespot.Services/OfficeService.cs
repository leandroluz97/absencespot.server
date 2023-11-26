using Absencespot.Business.Abstractions;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services
{
    public class OfficeService : IOfficeService
    {
        private readonly ILogger<OfficeService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public OfficeService(ILogger<OfficeService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async  Task<Office> CreateAsync(Guid companyId, Dtos.Office office, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (office == null)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            office.EnsureValidation();

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId);
            if(companyDomain == null)
            {
                throw new NotFoundException($"Not found {companyId}");
            }

            var officeDomain = OfficeMapper.ToDomain(office);
            officeDomain.Company = companyDomain;

            List<Domain.OfficeLeave> leaves = new List<Domain.OfficeLeave>();
            foreach (Guid id in office.AvailableLeaves)
            {
                var leave = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(id);
                if(officeDomain.AvailableLeaves == null)
                {
                    officeDomain.AvailableLeaves = new List<Domain.OfficeLeave>();
                }
                if(leave != null)
                {
                    officeDomain.AvailableLeaves.Add(new Domain.OfficeLeave()
                    {
                        Leave = leave,
                        Office = officeDomain,
                    });
                }
            }

            officeDomain = _unitOfWork.OfficeRepository.Add(officeDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Created office Id {officeDomain.Id}");
            return OfficeMapper.ToDto(officeDomain);
        }

        public Task DeleteAsync(Guid companyId, Guid officeId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Office>> GetAllAsync(Guid companyId, Office office, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Office> GetByIdAsync(Guid companyId, Office office, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Office> UpdateAsync(Guid companyId, Office office, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
