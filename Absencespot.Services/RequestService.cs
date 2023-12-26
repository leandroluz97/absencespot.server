﻿using Absencespot.Business.Abstractions;
using Absencespot.Domain;
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

        public RequestService(ILogger<RequestService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Dtos.Request> ApproveAsync(Guid companyId, Guid requestId, Dtos.ApproveRequest request, CancellationToken cancellationToken = default)
        {
            if(request == null)
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
            queryable = queryable.Where(u => u.Company.GlobalId == companyId && u.GlobalId == request.ApproverId);
            var approver= queryable.ToList().FirstOrDefault();
            if (approver == null)
            {
                throw new NotFoundException(nameof(approver));
            }

            //TODO: validate user role/permission
            //if(approver.Role != "Approver")
            //{
            //    throw new UnauthorizedAccessException();
            //}

            requestDomain.Approver = approver;
            requestDomain.Status = request.Status;

            _logger.LogInformation($"Rejected request Id: {requestId}");

            _unitOfWork.RequestRepository.Update(requestDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestMapper.ToDto(requestDomain);
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
            if (userAvailableLeaves == null)
            {
                throw new NotFoundException(nameof(userAvailableLeaves));
            }

            var officeResetMonth = officeDomain.StartDate.Month;
            var officeResetday = officeDomain.StartDate.Day;
            var thisYear = new DateTime(DateTime.Today.Year, officeResetMonth, officeResetday);
            var nextYear = new DateTime(DateTime.Today.AddYears(1).Year, officeResetMonth, officeResetday);

            userAvailableLeaves = userAvailableLeaves.Where(a => a.Absence.LeaveId == request.LeaveId);
            Domain.AvailableLeave? userAvailableLeave = null;

            if (request.StartDate.Year == thisYear.Year && request.EndDate.Year == thisYear.Year)
            {
                userAvailableLeave = userAvailableLeaves.Where(a => a.Period.Year == DateTime.Today.Year).FirstOrDefault();
            }
            else if (request.StartDate.Year == nextYear.Year && thisYear.Year == nextYear.Year)
            {
                userAvailableLeave = userAvailableLeaves.Where(a => a.Period.Year == DateTime.Today.AddYears(1).Year).FirstOrDefault();
            }

            if (userAvailableLeave != null)
            {
                var sumOfRequestDays = request.EndDate - request.StartDate;
                if (userAvailableLeave.AvailableDays < sumOfRequestDays.Days)
                {
                    throw new InvalidOperationException(nameof(userAvailableLeave.AvailableDays));
                }
            }

            if (request.StartDate < nextYear && request.EndDate > nextYear)
            {
                var userAvailableLeavesThisYear = userAvailableLeaves.Where(a => a.Period >= thisYear && a.Period <= nextYear);
                foreach (Domain.AvailableLeave item in userAvailableLeavesThisYear)
                {
                    if (item.Period >= thisYear)
                    {
                        var requestStartDateTillOfficeResetDate = nextYear - request.StartDate;
                        if (requestStartDateTillOfficeResetDate.Days > item.AvailableDays)
                        {
                            throw new InvalidOperationException(nameof(item.AvailableDays));
                        }
                    }
                }

                var userAvailableLeavesNextYear = userAvailableLeaves.Where(a => a.Period >= nextYear);
                foreach (Domain.AvailableLeave item in userAvailableLeavesThisYear)
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

            var requestDomain = RequestMapper.ToDomain(request);

            requestDomain = _unitOfWork.RequestRepository.Add(requestDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Created request with Id: {requestDomain.GlobalId}");

            return RequestMapper.ToDto(requestDomain);

        }

        public async Task DeleteAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default)
        {
            Guid userId = Guid.NewGuid();
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

            if(requestDomain.Status != StatusType.Pending)
            {
                throw new ConflictException(nameof(requestDomain.Status));
            }

            if(requestDomain.User.GlobalId != userId)
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
            Guid userId = Guid.NewGuid();
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

            if(requestDomain.User.GlobalId != userId)
            {
                throw new UnauthorizedAccessException(nameof(requestDomain));
            }

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

            var options = RepositoryOptions.AsNoTracking();
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
            requestDomain.Status = request.Status;

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

            if(userDomain.Company.GlobalId != companyDomain.GlobalId)
            {
                throw new UnauthorizedAccessException(nameof(userDomain));
            }

            if(userDomain.Id != requestDomain.UserId)
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
    }
}