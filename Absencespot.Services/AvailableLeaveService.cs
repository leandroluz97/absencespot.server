using Absencespot.Business.Abstractions;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services
{
    public class AvailableLeaveService : IAvailableLeaveService
    {
        private readonly ILogger<AvailableLeaveService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Domain.User> _userManager;

        public AvailableLeaveService(ILogger<AvailableLeaveService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IEnumerable<Dtos.AvailableLeave>> GetAllAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, RepositoryOptions.AsTracking());
            if (companyDomain == null)
            {
                throw new NotFoundException($"Not found {nameof(companyId)} {companyId}");
            }

            var queryable = _userManager.Users.AsQueryable();
            var userDomain = queryable.FirstOrDefault(u => u.Company.GlobalId == companyId && u.GlobalId == userId);
            if (userDomain == null)
            {
                throw new NotFoundException(nameof(userDomain));
            }

            var officeDomain = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(userDomain.Office.GlobalId);
            if (officeDomain == null)
            {
                throw new NotFoundException(nameof(officeDomain));
            }

            var officeResetMonth = officeDomain.StartDate.Month;
            var officeResetday = officeDomain.StartDate.Day;
            var thisYear = new DateTime(DateTime.Today.Year, officeResetMonth, officeResetday);
            var nextYear = new DateTime(DateTime.Today.AddYears(1).Year, officeResetMonth, officeResetday);

            var availableLeaveQueryable = _unitOfWork.AvailableLeaveRepository.AsQueryable();
            availableLeaveQueryable = availableLeaveQueryable.Where(a => a.Period.Year >= thisYear.Year && a.Period.Year <= nextYear.Year);
            List<Domain.AvailableLeave> availableLeaves = availableLeaveQueryable.ToList();

            if (!availableLeaves.Any())
            {
                foreach (var absence in officeDomain.Absences)
                {
                    var availableLeaveDomain = new Domain.AvailableLeave()
                    {
                        Period = thisYear,
                        AvailableDays = absence.Allowance,
                        Absence = absence,
                        User = userDomain,
                    };
                    availableLeaves.Add(availableLeaveDomain);
                }
                _unitOfWork.AvailableLeaveRepository.AddRange(availableLeaves);
               await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return availableLeaves.Select(AvailableLeaveMapper.ToDto);

        }
    }
}
