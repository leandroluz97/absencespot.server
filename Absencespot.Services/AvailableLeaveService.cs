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
            var officeResetDay = officeDomain.StartDate.Day;
            var startCurrentYear = new DateTime(DateTime.Today.AddYears(-1).Year, officeResetMonth, officeResetDay);
            var endCurrentYear = new DateTime(DateTime.Today.Year, officeResetMonth, officeResetDay).AddDays(-1);
            var startNextYear = new DateTime(DateTime.Today.Year, officeResetMonth, officeResetDay);
            var endNextYear = new DateTime(DateTime.Today.AddYears(1).Year, officeResetMonth, officeResetDay).AddDays(-1);

            if (startNextYear <= DateTime.Now) 
            {
                startCurrentYear = new DateTime(DateTime.Today.Year, officeResetMonth, officeResetDay);
                endCurrentYear = new DateTime(DateTime.Today.AddYears(1).Year, officeResetMonth, officeResetDay).AddDays(-1);
                startNextYear = new DateTime(DateTime.Today.AddYears(1).Year, officeResetMonth, officeResetDay);
                endNextYear = new DateTime(DateTime.Today.AddYears(2).Year, officeResetMonth, officeResetDay).AddDays(-1);
            }

            var availableLeaveQueryable = _unitOfWork.AvailableLeaveRepository.AsQueryable();
            availableLeaveQueryable = availableLeaveQueryable.Where(a => a.StartDate >= startCurrentYear && a.EndDate <= endNextYear);
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
                                 StartDate = startCurrentYear,
                                 EndDate = endCurrentYear,
                                 //AvailableDays = absence.MonthlyAccrual * (startNextYear - DateTime.Now), use month diff
                                 AvailableDays = absence.Allowance,
                                 Absence = absence,
                                 User = userDomain,
                             },
                             new Domain.AvailableLeave()
                             {
                                 StartDate = startNextYear,
                                 EndDate = endNextYear,
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
