using Absencespot.Business.Abstractions;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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

            var officeId = new Guid("CF9D943B-3727-4DDA-A5F9-7BFEF755C493");

            var officeDomain = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(officeId, RepositoryOptions.AsTracking());
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
            availableLeaveQueryable = _unitOfWork.AvailableLeaveRepository.Include(availableLeaveQueryable, x => x.Absence);
            availableLeaveQueryable = _unitOfWork.AvailableLeaveRepository.IncludeThen<Domain.Absence, Domain.Leave>(availableLeaveQueryable, x => x.Leave);
            List<Domain.AvailableLeave> availableLeaves = availableLeaveQueryable.ToList();

            if (!availableLeaves.Any())
            {
                foreach (var absence in officeDomain.Absences)
                {
                    availableLeaves.AddRange(
                        new List<Domain.AvailableLeave>()
                        {
                             new Domain.AvailableLeave()
                             {
                                 Period = thisYear,
                                 AvailableDays = absence.Allowance,
                                 Absence = absence,
                                 User = userDomain,
                             },
                             new Domain.AvailableLeave()
                             {
                                 Period = nextYear,
                                 AvailableDays = absence.Allowance,
                                 Absence = absence,
                                 User = userDomain,
                             },
                        }
                    );
                }
                _unitOfWork.AvailableLeaveRepository.AddRange(availableLeaves);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return availableLeaves.Select(AvailableLeaveMapper.ToDto);

        }
    }
}
