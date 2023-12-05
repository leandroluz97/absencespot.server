using Absencespot.Business.Abstractions;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.Utils;
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
        public async Task<Dtos.Office> CreateAsync(Guid companyId, Dtos.Office office, CancellationToken cancellationToken = default)
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

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, RepositoryOptions.AsTracking());
            if (companyDomain == null)
            {
                throw new NotFoundException($"Not found {companyId}");
            }

            var officeDomain = OfficeMapper.ToDomain(office);
            officeDomain.Company = companyDomain;

            List<Domain.OfficeLeave> officeLeaves = new List<Domain.OfficeLeave>();
            List<Domain.Absence> absences = new List<Domain.Absence>();
            foreach (Guid id in office.AvailableLeaves)
            {
                var leave = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(id, RepositoryOptions.AsTracking());
                if (leave != null)
                {
                    officeLeaves.Add(new Domain.OfficeLeave()
                    {
                        Leave = leave,
                        Office = officeDomain,
                    });

                    var absence = office.Absences.FirstOrDefault(x => x.LeaveId == leave.GlobalId);
                    if (absence != null )
                    {
                        absences.Add(new Domain.Absence()
                        {
                            MonthCarryOverExpiresAfter = absence.MonthCarryOverExpiresAfter,
                            MonthlyAccrual = absence.MonthlyAccrual,
                            Allowance = absence.Allowance,
                            MonthMaxCarryOver = absence.MonthMaxCarryOver,
                            Leave = leave,
                            Office = officeDomain,
                        });
                    }
                }
            }

            officeDomain.AvailableLeaves = officeLeaves;
            officeDomain.Absences = absences;

            officeDomain = _unitOfWork.OfficeRepository.Add(officeDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Created office Id {officeDomain.Id}");
            return OfficeMapper.ToDto(officeDomain);
        }

        public async Task DeleteAsync(Guid companyId, Guid officeId, CancellationToken cancellationToken = default)
        {
            if(companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (officeId == default)
            {
                throw new ArgumentNullException(nameof(officeId));
            }

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var officeDomain = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(officeId);
            if (officeDomain == null)
            {
                throw new NotFoundException(nameof(officeDomain));
            }

            if(officeDomain.CompanyId != companyDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            _unitOfWork.OfficeRepository.Remove(officeDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<Pagination<Office>> GetAllAsync(
            Guid companyId,
            int pageNumber = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default)
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
            
            var queryable = _unitOfWork.OfficeRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(l => l.Company.GlobalId == companyId);
            
            var totalOffices= queryable.Count();
            queryable = queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            queryable = _unitOfWork.OfficeRepository.Include(queryable, x => x.Address);
            var offices = await _unitOfWork.OfficeRepository.ToListAsync(queryable, cancellationToken);

            _logger.LogInformation($"Get office pageSize: {pageSize}, pageNumber: {pageNumber}");

            return new Pagination<Dtos.Office>()
            {
                TotalRecords = totalOffices,
                TotalPages = (int)Math.Ceiling((decimal)totalOffices / (decimal)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = offices.Select(OfficeMapper.ToDto)
            };
        }

        public async Task<Dtos.Office> GetByIdAsync(Guid companyId, Guid officeId, CancellationToken cancellationToken = default)
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

            if (officeId == default)
            {
                throw new ArgumentNullException(nameof(officeId));
            }

            var officeDomain = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(officeId);
            if (officeDomain == null)
            {
                throw new NotFoundException(nameof(officeDomain));
            }

            _logger.LogInformation($"Found leave Id: {officeId}");

            return OfficeMapper.ToDto(officeDomain);
        }

        public async Task<Dtos.Office> UpdateAsync(Guid companyId, Guid officeId, Dtos.Office office, CancellationToken cancellationToken = default)
        {
            if(office == null)
            {
                throw new ArgumentNullException(nameof(office));
            }
            office.EnsureValidation();

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            if (officeId == default)
            {
                throw new ArgumentNullException(nameof(officeId));
            }
            var officeDomain = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(officeId);
            if (officeDomain == null)
            {
                throw new NotFoundException(nameof(officeDomain));
            }

            if (officeDomain.CompanyId != companyDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            officeDomain.Name = office.Name;
            officeDomain.IsEmployeeLeaveStartOnJobStartDate = office.IsEmployeeLeaveStartOnJobStartDate;
            officeDomain.StartDate = office.StartDate;
            officeDomain.Address = AddressMapper.ToDomain(office.Address);

            return OfficeMapper.ToDto(officeDomain);
        }
    }
}
