using Absencespot.Business.Abstractions;
using Absencespot.Domain;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Absencespot.Services
{
    public class RequestService : IRequestService
    {
        private readonly ILogger<RequestService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Domain.User> _userManager;

        public RequestService(ILogger<RequestService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public Task<Dtos.Request> ApproveAsync(Guid companyId, Guid requestId, Dtos.ApproveRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Dtos.Request> CreateAsync(Guid companyId, Dtos.Request request, CancellationToken cancellationToken = default)
        {
            Guid userId = Guid.NewGuid();
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            request.EnsureValidation();

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, RepositoryOptions.AsTracking());
            if (companyDomain == null)
            {
                throw new NotFoundException($"Not found {nameof(companyId)} {companyId}");
            }

            var teamDomain = RequestMapper.ToDomain(request);
            // Load all the request from this year by userId and Leave type
            // Load office montly allowance of the type equal to the Leave type 
            // Check if user still have days left off
            // If so create que request other wise prevent the user from creating request

            // **Validate the following bussiness rules
            // check if user still have left absence days for that partitcular leave
            // check if the absence requested days is from this year or next year
            // check the weekend based on the office working schedule
            // check if its holiday and exclude the days from the being counted if so
            // In case of accumolated absence days from previous year
            //      check if the absence days still valid (base on office accruals)
            //      if not dont count those absence days that have not been taken from previous year

            var queryable = _userManager.Users.AsQueryable();
            queryable = queryable.Where(u => u.Company.GlobalId == companyId && u.GlobalId == userId);
            var userDomain = queryable.ToList().FirstOrDefault();
            if (userDomain == null)
            {
                throw new NotFoundException(nameof(userDomain));
            }

            var officeDomain = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(userDomain.Office.GlobalId);
            if (officeDomain == null)
            {
                throw new NotFoundException(nameof(officeDomain));
            }

            var userAvailableLeaves = await _unitOfWork.AvailableLeaveRepository.FindByUserIdAsync(userDomain.Id);
            userAvailableLeaves = userAvailableLeaves.Where(a => a.Absence.LeaveId == request.LeaveId);
            AvailableLeave? userAvailableLeave = null;

            if (request.StartDate.Year == DateTime.Today.Year && request.EndDate.Year == DateTime.Today.Year)
            {
                userAvailableLeave = userAvailableLeaves.Where(a => a.Period.Year == DateTime.Today.Year).FirstOrDefault();
            }
            else if (request.StartDate.Year > DateTime.Today.Year && request.EndDate.Year > DateTime.Today.Year)
            {
                userAvailableLeave = userAvailableLeaves.Where(a => a.Period.Year == DateTime.Today.AddYears(1).Year).FirstOrDefault();
            }

            if(userAvailableLeave != null)
            {
                var sumOfRequestDays = request.EndDate - request.StartDate;
                if(userAvailableLeave.AvailableDays < sumOfRequestDays.Days)
                {
                    throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                }
            }

            var officeResetMonth = officeDomain.StartDate.Month;
            var officeResetday = officeDomain.StartDate.Day;
            var thisYear = new DateTime(DateTime.Today.Year, officeResetMonth, officeResetday);
            var nextYear = new DateTime(DateTime.Today.AddYears(1).Year, officeResetMonth, officeResetday);

            if(request.StartDate < nextYear && request.EndDate > nextYear)
            {
                var userAvailableLeavesThisYear = userAvailableLeaves.Where(a => a.Period >= thisYear && a.Period <= nextYear);
                foreach (AvailableLeave item in userAvailableLeavesThisYear)
                {
                    if(item.Period >= thisYear)
                    {
                        var requestStartDateTillOfficeResetDate = nextYear - request.StartDate;
                        if( requestStartDateTillOfficeResetDate.Days > item.AvailableDays)
                        {
                            throw new InvalidOperationException(nameof(item.AvailableDays));
                        }
                    }
                }

                var userAvailableLeavesNextYear = userAvailableLeaves.Where(a => a.Period >= nextYear);
                foreach (AvailableLeave item in userAvailableLeavesThisYear)
                {
                    if (item.Period >= nextYear)
                    {
                        var sumOfDaysOfNextYearRequest = request.EndDate - nextYear;
                        if (sumOfDaysOfNextYearRequest.Days > item.AvailableDays)
                        {
                            throw new InvalidOperationException(nameof(item.AvailableDays));
                        }
                    }
                }
            }

            



            _logger.LogInformation($"Created request Id: ");

            return default;

        }

        public Task DeleteAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<Dtos.Request>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dtos.Request> GetByIdAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dtos.Request> RejectAsync(Guid companyId, Guid requestId, RejectRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dtos.Request> UpdateAsync(Guid companyId, Guid requestId, Dtos.Request request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}