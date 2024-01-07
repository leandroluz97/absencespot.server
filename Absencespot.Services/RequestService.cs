﻿using Absencespot.Business.Abstractions;
using Absencespot.Domain.Enums;
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
            var startNextYear = new DateTime(DateTime.Today.Year, officeResetMonth, officeResetDay);
            var endCurrentYear = new DateTime(DateTime.Today.Year, officeResetMonth, officeResetDay);
            var endNextYear = new DateTime(DateTime.Today.AddYears(1).Year, officeResetMonth, officeResetDay);

            Domain.AvailableLeave? userAvailableLeave = null;

            if (requestDomain.StartDate > startCurrentYear && requestDomain.EndDate <= endCurrentYear)
            {
                userAvailableLeave = userAvailableLeaves.Where(a => a.StartDate >= startCurrentYear && a.EndDate <= endCurrentYear)
                    .OrderBy(a => a.StartDate)
                    .FirstOrDefault();
            }
            else if (requestDomain.StartDate > startNextYear && requestDomain.EndDate <= endNextYear)
            {
                userAvailableLeave = userAvailableLeaves.Where(a => a.StartDate > startNextYear && a.EndDate <= endNextYear)
                    .OrderByDescending(a => a.StartDate)
                    .FirstOrDefault();
            }

            if (userAvailableLeave != null)
            {
                var sumOfRequestDays = requestDomain.EndDate - requestDomain.StartDate;
                if (userAvailableLeave.AvailableDays < sumOfRequestDays.Days)
                {
                    throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                }
            }

            if (requestDomain.StartDate < startNextYear && requestDomain.EndDate > startNextYear)
            {
                var userAvailableLeavesThisYear = userAvailableLeaves.Where(a => a.StartDate >= startCurrentYear && a.EndDate <= startNextYear);
                foreach (Domain.AvailableLeave item in userAvailableLeavesThisYear)
                {
                    if (item.EndDate <= startNextYear)
                    {
                        var sumOfRequestDays = requestDomain.EndDate - requestDomain.StartDate;
                        if (sumOfRequestDays.Days > item.AvailableDays)
                        {
                            throw new InvalidOperationException(nameof(item.AvailableDays));
                        }
                        item.AvailableDays -= sumOfRequestDays.TotalDays;
                        _unitOfWork.AvailableLeaveRepository.Update(item);
                    }
                }

                var userAvailableLeavesNextYear = userAvailableLeaves.Where(a => a.EndDate >= endNextYear);
                foreach (Domain.AvailableLeave item in userAvailableLeavesNextYear)
                {
                    if (item.EndDate > startNextYear)
                    {
                        var sumOfDaysOfNextYearRequest = requestDomain.EndDate - startNextYear;
                        if (sumOfDaysOfNextYearRequest.Days > item.AvailableDays)
                        {
                            throw new InvalidOperationException(nameof(item.AvailableDays));
                        }
                        item.AvailableDays -= sumOfDaysOfNextYearRequest.TotalDays;
                        _unitOfWork.AvailableLeaveRepository.Update(item);
                    }
                }
            }

            _logger.LogInformation($"Approve request Id: {requestId}");

            _unitOfWork.RequestRepository.Update(requestDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

            var officeDomain = await _unitOfWork.OfficeRepository.FindByIdAsync((int)userDomain.OfficeId);
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

            //Domain.AvailableLeave? userAvailableLeave = null;

            //if (request.StartDate >= startCurrentYear && request.EndDate <= endCurrentYear)
            //{        
            //    userAvailableLeave = userAvailableLeaves.Where(a => a.StartDate >= startCurrentYear && a.EndDate <= endCurrentYear)
            //        .OrderBy(a => a.StartDate)
            //        .FirstOrDefault();
            //}
            //else if (request.StartDate >= startNextYear && request.EndDate <= endNextYear)
            //{
            //    userAvailableLeave = userAvailableLeaves.Where(a => a.StartDate > startNextYear && a.EndDate <= endNextYear)
            //        .OrderByDescending(a => a.StartDate)
            //        .FirstOrDefault();
            


            var userAvailableLeave = userAvailableLeaves.FirstOrDefault(a => a.StartDate >= request.StartDate && a.EndDate <= request.EndDate);
            if (userAvailableLeave != null)
            {
                var sumOfRequestDays = request.EndDate - request.StartDate;
                if (userAvailableLeave.AvailableDays < sumOfRequestDays.Days)
                {
                    throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                }
            }

            if (request.StartDate < startNextYear && request.EndDate >= startNextYear)
            {
                var userAvailableLeavesThisYear = userAvailableLeaves.Where(a => request.StartDate >= a.StartDate && a.EndDate < startNextYear);
                foreach (Domain.AvailableLeave item in userAvailableLeavesThisYear)
                {
                    if (item.EndDate < startNextYear)
                    {
                        var requestStartDateTillOfficeResetDate = startNextYear - request.StartDate;
                        if (requestStartDateTillOfficeResetDate.Days > item.AvailableDays)
                        {
                            throw new InvalidOperationException(nameof(item.AvailableDays));
                        }
                    }
                }

                var userAvailableLeavesNextYear = userAvailableLeaves.Where(a => a.EndDate >= startNextYear);
                foreach (Domain.AvailableLeave item in userAvailableLeavesNextYear)
                {
                    if (item.EndDate >= startNextYear)
                    {
                        var sumOfDaysOfNextYearRequest = request.EndDate - startNextYear;
                        if (sumOfDaysOfNextYearRequest.Days > item.AvailableDays)
                        {
                            throw new InvalidOperationException(nameof(item.AvailableDays));
                        }
                    }
                }
            }

            //if (request.StartDate < startNextYear && request.EndDate > startNextYear)
            //{
            //    var userAvailableLeavesThisYear = userAvailableLeaves.Where(a => a.StartDate >= startCurrentYear && a.EndDate <= startNextYear);
            //    foreach (Domain.AvailableLeave item in userAvailableLeavesThisYear)
            //    {
            //        if (item.EndDate <= startNextYear)
            //        {
            //            var requestStartDateTillOfficeResetDate = startNextYear - request.StartDate;
            //            if (requestStartDateTillOfficeResetDate.Days > item.AvailableDays)
            //            {
            //                throw new InvalidOperationException(nameof(item.AvailableDays));
            //            }
            //        }
            //    }

            //    var userAvailableLeavesNextYear = userAvailableLeaves.Where(a => a.EndDate >= endNextYear);
            //    foreach (Domain.AvailableLeave item in userAvailableLeavesNextYear)
            //    {
            //        if (item.EndDate > startNextYear)
            //        {
            //            var sumOfDaysOfNextYearRequest = request.EndDate - startNextYear;
            //            if (sumOfDaysOfNextYearRequest.Days > item.AvailableDays)
            //            {
            //                throw new InvalidOperationException(nameof(item.AvailableDays));
            //            }
            //        }
            //    }
            //}

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
    }
}