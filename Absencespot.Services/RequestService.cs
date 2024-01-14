using Absencespot.Business.Abstractions;
using Absencespot.Domain.Enums;
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
        private const int _sumOfWeekDays = 7;

        public RequestService(ILogger<RequestService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Dtos.Request> ApproveAsync(Guid companyId, Guid requestId, Dtos.ApproveRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            request.EnsureValidation();

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var officeDomain = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(request.OfficeId, cancellationToken: cancellationToken);
            if (officeDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var options = RepositoryOptions.AsTracking();
            var requestDomain = await _unitOfWork.RequestRepository.FindByGlobalIdAsync(requestId, options);
            if (requestDomain == null)
            {
                throw new NotFoundException(nameof(requestDomain));
            }

            var approverDomain = await LoadUserByIdAsync(companyId, request.ApproverId);
            var userDomain = await LoadUserByIdAsync(companyId, request.UserId);

            var workscheduleDomain = await _unitOfWork.WorkScheduleRepository.FindByIdAsync((int)userDomain.WorkScheduleId!);
            if (workscheduleDomain == null)
            {
                throw new NotFoundException(nameof(workscheduleDomain));
            }
            var workschedule = WorkScheduleMapper.ToDto(workscheduleDomain);

            var leaveDomain = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(request.LeaveId, RepositoryOptions.AsTracking());
            if (leaveDomain == null)
            {
                throw new NotFoundException(nameof(leaveDomain));
            }

            //TODO: validate user role/permission
            //if(approver.Role != "Approver")
            //{
            //    throw new UnauthorizedAccessException();
            //}

            requestDomain.Approver = approverDomain;
            requestDomain.Status = StatusType.Approved;

            var asTrackingOptions = RepositoryOptions.AsTracking();
            var userAvailableLeaves = await _unitOfWork.AvailableLeaveRepository.FindByUserIdAsync(requestDomain.UserId, asTrackingOptions);
            if (!userAvailableLeaves.Any())
            {
                throw new NotFoundException(nameof(userAvailableLeaves));
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

            var userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => requestDomain.StartDate >= a.StartDate && requestDomain.EndDate <= a.EndDate);
            if (userAvailableLeave != null)
            {
                var sumOfRequestDays = GetWorkableDays(requestDomain.StartDate, requestDomain.EndDate, workschedule);
                if (userAvailableLeave.AvailableDays < sumOfRequestDays)
                {
                    //throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                    var absence = officeDomain.Absences.FirstOrDefault(x => x.Leave.GlobalId == leaveDomain.GlobalId);
                    if (absence == null)
                    {
                        throw new NotFoundException(nameof(absence));   
                    }
                    if (absence.MonthMaxCarryOver < requestDomain.EndDate.Month)
                    {
                        throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                    }

                    DateTime startDate = userAvailableLeave.StartDate.AddYears(-1);
                    DateTime endDate = userAvailableLeave.EndDate.AddYears(-1);
                    userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => startDate >= a.StartDate && endDate <= a.EndDate);
                    if (userAvailableLeave == null)
                    {
                        throw new NotFoundException(nameof(userAvailableLeave));
                    }
                    if (userAvailableLeave.AvailableDays < sumOfRequestDays)
                    {
                        throw new InvalidOperationException(nameof(userAvailableLeave));
                    }
                }
                userAvailableLeave.AvailableDays -= sumOfRequestDays;
                _unitOfWork.AvailableLeaveRepository.Update(userAvailableLeave);
            }

            if (requestDomain.StartDate < startNextYear && requestDomain.EndDate >= startNextYear)
            {
                userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => requestDomain.StartDate >= a.StartDate && a.EndDate < startNextYear);
                if (userAvailableLeave != null)
                {
                    var requestStartDateTillOfficeResetDate = GetWorkableDays(requestDomain.StartDate, startNextYear, workschedule);
                    if (requestStartDateTillOfficeResetDate > userAvailableLeave.AvailableDays)
                    {
                        var absence = officeDomain.Absences.FirstOrDefault(x => x.Leave.GlobalId == leaveDomain.GlobalId);
                        if (absence == null)
                        {
                            throw new NotFoundException(nameof(absence));
                        }
                        if (officeDomain.StartDate.AddMonths(Convert.ToInt32(absence.MonthMaxCarryOver)).Month < requestDomain.EndDate.Month)
                        {
                            throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                        }

                        DateTime startDate = userAvailableLeave.StartDate.AddYears(-1);
                        DateTime endDate = userAvailableLeave.EndDate.AddYears(-1);
                        userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => startDate >= a.StartDate && endDate <= a.EndDate);
                        if (userAvailableLeave == null)
                        {
                            throw new NotFoundException(nameof(userAvailableLeave));
                        }
                        if (userAvailableLeave.AvailableDays < requestStartDateTillOfficeResetDate)
                        {
                            throw new InvalidOperationException(nameof(userAvailableLeave));
                        }
                    }
                    userAvailableLeave.AvailableDays -= requestStartDateTillOfficeResetDate;
                    _unitOfWork.AvailableLeaveRepository.Update(userAvailableLeave);
                }

                userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => a.EndDate >= startNextYear && a.EndDate <= endNextYear);
                if (userAvailableLeave != null)
                {
                    var sumOfDaysOfNextYearRequest = GetWorkableDays(startNextYear, requestDomain.EndDate, workschedule);
                    if (sumOfDaysOfNextYearRequest > userAvailableLeave.AvailableDays)
                    {
                        var absence = officeDomain.Absences.FirstOrDefault(x => x.Leave.GlobalId == leaveDomain.GlobalId);
                        if (absence == null)
                        {
                            throw new NotFoundException(nameof(absence));
                        }
                        if (officeDomain.StartDate.AddMonths(Convert.ToInt32(absence.MonthMaxCarryOver)).Month < requestDomain.EndDate.Month)
                        {
                            throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                        }

                        DateTime startDate = userAvailableLeave.StartDate.AddYears(-1);
                        DateTime endDate = userAvailableLeave.EndDate.AddYears(-1);
                        userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => startDate >= a.StartDate && endDate <= a.EndDate);
                        if (userAvailableLeave == null)
                        {
                            throw new NotFoundException(nameof(userAvailableLeave));
                        }
                        if (userAvailableLeave.AvailableDays < sumOfDaysOfNextYearRequest)
                        {
                            throw new InvalidOperationException(nameof(userAvailableLeave));
                        }
                    }
                    userAvailableLeave.AvailableDays -= sumOfDaysOfNextYearRequest;
                    _unitOfWork.AvailableLeaveRepository.Update(userAvailableLeave);

                }
            }

            _unitOfWork.RequestRepository.Update(requestDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Approve request Id: {requestId}");

            return RequestMapper.ToDto(requestDomain);
        }


        public async Task<Dtos.Request> CreateAsync(Guid companyId, Dtos.Request request, CancellationToken cancellationToken = default)
        {
            Guid userId = new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2");
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

            var queryable = _userManager.Users.AsQueryable();
            queryable = queryable.Where(u => u.Company.GlobalId == companyId && u.GlobalId == userId);
            var userDomain = queryable.ToList().FirstOrDefault();
            if (userDomain == null)
            {
                throw new NotFoundException(nameof(userDomain));
            }

            var officeDomain = await _unitOfWork.OfficeRepository.FindByIdIncludedAsync((int)userDomain.OfficeId);
            if (officeDomain == null)
            {
                throw new NotFoundException(nameof(officeDomain));
            }

            var leaveDomain = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(request.LeaveId, RepositoryOptions.AsTracking());
            if (leaveDomain == null)
            {
                throw new NotFoundException(nameof(leaveDomain));
            }

            var userAvailableLeaves = await _unitOfWork.AvailableLeaveRepository.FindByUserIdAsync(userDomain.Id);
            if (userAvailableLeaves == null || !userAvailableLeaves.Any())
            {
                throw new NotFoundException(nameof(userAvailableLeaves));
            }

            var workscheduleDomain = await _unitOfWork.WorkScheduleRepository.FindByIdAsync((int)userDomain.WorkScheduleId!);
            if (workscheduleDomain == null)
            {
                throw new NotFoundException(nameof(workscheduleDomain));
            }
            var workschedule = WorkScheduleMapper.ToDto(workscheduleDomain);

            userAvailableLeaves = userAvailableLeaves.Where(a => a.Absence.Leave.GlobalId == request.LeaveId);
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


            var userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => request.StartDate >= a.StartDate && request.EndDate <= a.EndDate);
            if (userAvailableLeave != null)
            {
                var totalRequestedDays = GetWorkableDays(request.StartDate, request.EndDate, workschedule);
                if (userAvailableLeave.AvailableDays < totalRequestedDays)
                {
                    var absence = officeDomain.Absences.FirstOrDefault(x => x.Leave.GlobalId == leaveDomain.GlobalId);
                    if (absence == null)
                    {
                        throw new NotFoundException(nameof(absence));
                    }
                    if (officeDomain.StartDate.AddMonths(Convert.ToInt32(absence.MonthMaxCarryOver)).Month < request.EndDate.Month)
                    {
                        throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                    }

                    DateTime startDate = userAvailableLeave.StartDate.AddYears(-1);
                    DateTime endDate = userAvailableLeave.EndDate.AddYears(-1);
                    userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => startDate >= a.StartDate && endDate <= a.EndDate);
                    if (userAvailableLeave == null)
                    {
                        throw new NotFoundException(nameof(userAvailableLeave));
                    }
                    if (userAvailableLeave.AvailableDays < totalRequestedDays)
                    {
                        throw new NotFoundException(nameof(userAvailableLeave));
                    }
                }
            }

            if (request.StartDate < startNextYear && request.EndDate >= startNextYear)
            {
                userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => request.StartDate >= a.StartDate && a.EndDate < startNextYear);
                if (userAvailableLeave != null)
                {
                    if (userAvailableLeave.EndDate < startNextYear)
                    {
                        var totalRequestedDays = GetWorkableDays(request.StartDate, startNextYear, workschedule);
                        if (totalRequestedDays > userAvailableLeave.AvailableDays)
                        {
                            //throw new InvalidOperationException(nameof(item.AvailableDays));
                            var absence = officeDomain.Absences.FirstOrDefault(x => x.Leave.GlobalId == leaveDomain.GlobalId);
                            if (absence == null)
                            {
                                throw new NotFoundException(nameof(absence));
                            }
                            if (officeDomain.StartDate.AddMonths(Convert.ToInt32(absence.MonthMaxCarryOver)).Month < startNextYear.Month)
                            {
                                throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                            }

                            DateTime startDate = userAvailableLeave.StartDate.AddYears(-1);
                            DateTime endDate = userAvailableLeave.EndDate.AddYears(-1);
                            var backupAvailableLeave = userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => startDate >= a.StartDate && endDate <= a.EndDate);
                            if (backupAvailableLeave == null)
                            {
                                throw new NotFoundException(nameof(backupAvailableLeave));
                            }
                            if (backupAvailableLeave.AvailableDays < totalRequestedDays)
                            {
                                throw new NotFoundException(nameof(backupAvailableLeave));
                            }
                        }
                    }
                }

                userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => a.EndDate >= startNextYear && a.EndDate <= endNextYear);
                if (userAvailableLeave != null)
                {
                    if (userAvailableLeave.EndDate >= startNextYear)
                    {
                        var sumOfDaysOfNextYearRequest = GetWorkableDays(startNextYear, request.EndDate, workschedule);
                        if (sumOfDaysOfNextYearRequest > userAvailableLeave.AvailableDays)
                        {
                            var absence = officeDomain.Absences.FirstOrDefault(x => x.Leave.GlobalId == leaveDomain.GlobalId);
                            if (absence == null)
                            {
                                throw new NotFoundException(nameof(absence));
                            }
                            if (officeDomain.StartDate.AddMonths(Convert.ToInt32(absence.MonthMaxCarryOver)).Month < startNextYear.Month)
                            {
                                throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                            }

                            DateTime startDate = userAvailableLeave.StartDate.AddYears(-1);
                            DateTime endDate = userAvailableLeave.EndDate.AddYears(-1);
                            var backupAvailableLeave = userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => startDate >= a.StartDate && endDate <= a.EndDate);
                            if (backupAvailableLeave == null)
                            {
                                throw new NotFoundException(nameof(backupAvailableLeave));
                            }
                            if (backupAvailableLeave.AvailableDays < sumOfDaysOfNextYearRequest)
                            {
                                throw new NotFoundException(nameof(backupAvailableLeave));
                            }
                        }
                    }
                }
            }

            var requestDomain = RequestMapper.ToDomain(request);
            requestDomain.Leave = leaveDomain;
            requestDomain.User = userDomain;
            requestDomain.OnBehalfOf = userDomain;

            requestDomain = _unitOfWork.RequestRepository.Add(requestDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Created request with Id: {requestDomain.GlobalId}");

            return RequestMapper.ToDto(requestDomain);

        }

        public async Task DeleteAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default)
        {
            Guid userId = new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2");
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (requestId == default)
            {
                throw new ArgumentNullException(nameof(requestId));
            }
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, RepositoryOptions.AsTracking());
            if (companyDomain == null)
            {
                throw new NotFoundException($"Not found {nameof(companyId)} {companyId}");
            }

            var requestDomain = await _unitOfWork.RequestRepository.FindByGlobalIdAsync(requestId, RepositoryOptions.AsTracking());
            if (requestDomain == null)
            {
                throw new NotFoundException($"Not found {nameof(requestId)} {requestId}");
            }

            if (requestDomain.Status != StatusType.Pending)
            {
                throw new ConflictException(nameof(requestDomain.Status));
            }

            if (requestDomain.User.GlobalId != userId)
            {
                throw new UnauthorizedAccessException(nameof(requestDomain.User.GlobalId));
            }
            _logger.LogInformation($"Deleted requestId:{requestId}");

            _unitOfWork.RequestRepository.Remove(requestDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<Pagination<Dtos.Request>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (pageNumber < 1)
            {
                throw new ArgumentException(nameof(pageNumber));
            }
            if (pageSize < 1 || pageSize > 200)
            {
                throw new ArgumentException(nameof(pageSize));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var queryable = _unitOfWork.RequestRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(q => q.User.Company.GlobalId == companyId);

            //if(user == "User")
            //{
            //    queryable = queryable.Where(q => q.User.GlobalId == Guid.Empty);
            //}

            var totalOffices = queryable.Count();
            queryable = queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            queryable = _unitOfWork.RequestRepository.Include(queryable, x => x.Leave);
            queryable = _unitOfWork.RequestRepository.Include(queryable, x => x.OnBehalfOf);
            queryable = _unitOfWork.RequestRepository.Include(queryable, x => x.User);
            queryable = _unitOfWork.RequestRepository.Include(queryable, x => x.Approver);
            var offices = await _unitOfWork.RequestRepository.ToListAsync(queryable, cancellationToken);

            _logger.LogInformation($"Get office pageSize: {pageSize}, pageNumber: {pageNumber}");

            return new Pagination<Dtos.Request>()
            {
                TotalRecords = totalOffices,
                TotalPages = (int)Math.Ceiling((decimal)totalOffices / (decimal)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = offices.Select(RequestMapper.ToDto)
            };
        }

        public async Task<Dtos.Request> GetByIdAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default)
        {
            Guid userId = new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2");
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            if (requestId == default)
            {
                throw new ArgumentNullException(nameof(requestId));
            }

            var requestDomain = await _unitOfWork.RequestRepository.FindByGlobalIdAsync(requestId, cancellationToken: cancellationToken);
            if (requestDomain == null)
            {
                throw new NotFoundException(nameof(requestDomain));
            }

            //if (requestDomain.User.GlobalId != userId)
            //{
            //    throw new UnauthorizedAccessException(nameof(requestDomain));
            //}

            _logger.LogInformation($"Found request Id: {requestId}");

            return RequestMapper.ToDto(requestDomain);
        }

        public async Task<Dtos.Request> RejectAsync(Guid companyId, Guid requestId, Dtos.RejectRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            request.EnsureValidation();

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var options = RepositoryOptions.AsTracking();
            var requestDomain = await _unitOfWork.RequestRepository.FindByGlobalIdAsync(requestId, options);
            if (requestDomain == null)
            {
                throw new NotFoundException(nameof(requestDomain));
            }

            var queryable = _userManager.Users.AsQueryable();
            queryable = queryable.Where(u => u.Company.GlobalId == companyId && u.GlobalId == request.ApproverId);
            var approver = queryable.ToList().FirstOrDefault();
            if (approver == null)
            {
                throw new NotFoundException(nameof(approver));
            }

            requestDomain.Approver = approver;
            requestDomain.Status = StatusType.Rejected;

            _logger.LogInformation($"Rejected request Id: {requestId}");

            _unitOfWork.RequestRepository.Update(requestDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestMapper.ToDto(requestDomain);
        }

        public async Task<Dtos.Request> UpdateAsync(Guid companyId, Guid requestId, Dtos.Request request, CancellationToken cancellationToken = default)
        {
            Guid userId = Guid.Empty;
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            request.EnsureValidation();

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var options = RepositoryOptions.AsNoTracking();
            var requestDomain = await _unitOfWork.RequestRepository.FindByGlobalIdAsync(requestId, options);
            if (requestDomain == null)
            {
                throw new NotFoundException(nameof(requestDomain));
            }

            var queryable = _userManager.Users.AsQueryable();
            queryable = queryable.Where(u => u.Company.GlobalId == companyId && u.GlobalId == userId);
            var userDomain = queryable.ToList().FirstOrDefault();
            if (userDomain == null)
            {
                throw new NotFoundException(nameof(userDomain));
            }

            if (userDomain.Company.GlobalId != companyDomain.GlobalId)
            {
                throw new UnauthorizedAccessException(nameof(userDomain));
            }

            if (userDomain.Id != requestDomain.UserId)
            {
                throw new UnauthorizedAccessException(nameof(userDomain));
            }

            requestDomain.StartDate = request.StartDate;
            requestDomain.EndDate = request.EndDate;
            requestDomain.Note = request.Note;
            requestDomain.OnBehalfOf = userDomain;

            _logger.LogInformation($"Updated request Id: {requestId}");

            _unitOfWork.RequestRepository.Update(requestDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestMapper.ToDto(requestDomain);
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

        private double GetWorkableDays(DateTime startDate, DateTime endDate, Dtos.WorkSchedule workschedule)
        {
            double nonWorkableDays = 0;
            var sumOfRequestDays = endDate - startDate;
            var workDays = workschedule.WorkDays!.Select(Enum.Parse<DayOfWeek>);

            if (workschedule.IsDefault)
            {
                for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
                {
                    if (!workDays.Contains(currentDate.DayOfWeek))
                    {
                        nonWorkableDays++;
                    }
                }
            }
            else
            {
                var totalWeeksOfVacation = sumOfRequestDays.TotalDays / _sumOfWeekDays;
                var totalWorkDaysInWeek = (double)workschedule!.TotalWorkDays;
                nonWorkableDays = totalWeeksOfVacation * (_sumOfWeekDays - totalWorkDaysInWeek);
            }

            return sumOfRequestDays.TotalDays - nonWorkableDays;
        }
    }
}