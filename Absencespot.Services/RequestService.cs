using Absencespot.Business.Abstractions;
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

        public Task<Request> ApproveAsync(Guid companyId, Guid requestId, ApproveRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public  async Task<Request> CreateAsync(Guid companyId, Dtos.Request request, CancellationToken cancellationToken = default)
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
            var requestQueryable = _unitOfWork.RequestRepository.AsQueryable();
            requestQueryable = requestQueryable.Where(r => r.User.GlobalId == userId && r.User.Company.GlobalId == companyId);
            //requestQueryable = requestQueryable.Where(r =>  r.EndDate >= officeDomain.StartDate.);

            var day = officeDomain.StartDate.Day;
            var month = officeDomain.StartDate.Month;
            var maxMonthCarryOverExpire = (int)officeDomain.Absences.FirstOrDefault(x => x.Leave.Id == request.LeaveId).MonthCarryOverExpiresAfter;

            DateTime? range1;
            if(officeDomain.CreatedAt.Year < DateTime.Now.Year)
            {
               range1 = new DateTime(DateTime.Now.Year, month, day).AddMonths(maxMonthCarryOverExpire);
            }
            else
            {
                range1 = new DateTime(DateTime.Now.Year, month, day);
            }

            _logger.LogInformation($"Created request Id: ");

            //officeDomain.StartDate

            // requestQueryable = requestQueryable.Where(r => r.StartDate  );


        }

        public Task DeleteAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<Request>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Request> GetByIdAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Request> RejectAsync(Guid companyId, Guid requestId, RejectRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Request> UpdateAsync(Guid companyId, Guid requestId, Request request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}