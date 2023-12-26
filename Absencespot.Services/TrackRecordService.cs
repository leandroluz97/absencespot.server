using Absencespot.Business.Abstractions;
using Absencespot.Domain.Enums;
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
    public class TrackRecordService : ITrackRecordService
    {
        private readonly ILogger<TrackRecordService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Domain.User> _userManager;

        public TrackRecordService(ILogger<TrackRecordService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<Dtos.TrackRecord> CreateAsync(Guid companyId, Dtos.TrackRecord trackRecord, CancellationToken cancellationToken = default)
        {
            Guid userId = Guid.Empty;
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (trackRecord == null)
            {
                throw new ArgumentNullException(nameof(trackRecord));
            }
            trackRecord.EnsureValidation();

            await LoadCompanyByIdAsync(companyId, cancellationToken);

            var userDomain = await LoadUserByIdAsync(companyId, userId);

            var trackRecordDomain = TrackRecordMapper.ToDomain(trackRecord);
            trackRecordDomain.User = userDomain;

            trackRecordDomain = _unitOfWork.TrackRecordRepository.Add(trackRecordDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Created track record Id {trackRecordDomain.Id}");
            return TrackRecordMapper.ToDto(trackRecordDomain);
        }

        public Task DeleteAsync(Guid companyId, Guid trackRecordId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Pagination<Dtos.TrackRecord>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            Guid userId = Guid.NewGuid();
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

            var companyDomain = await LoadCompanyByIdAsync(companyId, cancellationToken);
            var userDomain = await LoadUserByIdAsync(userId, companyId);

            //var queryable = _unitOfWork.TrackRecordRepository.AsQueryable(RepositoryOptions.AsNoTracking());

            //if(User.Role == RoleType.USER)
            //{
            //    queryable = queryable.Where(l => l.User.GlobalId == userDomain.GlobalId);
            //}
            //else
            //{
            //    queryable = queryable.Where(l => l.User.Company.Id == companyDomain.Id);
            //}

            var queryable = _unitOfWork.TrackRecordRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(l => l.User.GlobalId == userDomain.GlobalId);

            var totalOffices = queryable.Count();
            queryable = queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var trackRecords = await _unitOfWork.TrackRecordRepository.ToListAsync(queryable, cancellationToken);

            _logger.LogInformation($"Get track records pageSize: {pageSize}, pageNumber: {pageNumber}");

            return new Pagination<Dtos.TrackRecord>()
            {
                TotalRecords = totalOffices,
                TotalPages = (int)Math.Ceiling((decimal)totalOffices / (decimal)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = trackRecords.Select(TrackRecordMapper.ToDto)
            };
        }

        public async Task<TrackRecord> GetByIdAsync(Guid companyId, Guid trackRecordId, CancellationToken cancellationToken = default)
        {
            Guid userId = Guid.NewGuid();

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            await LoadCompanyByIdAsync(companyId, cancellationToken);
            var userDomain = await LoadUserByIdAsync(companyId, userId);

            if (trackRecordId == default)
            {
                throw new ArgumentNullException(nameof(trackRecordId));
            }

            var trackRecordDomain = await _unitOfWork.TrackRecordRepository.FindByGlobalIdAsync(trackRecordId);
            if (trackRecordDomain == null)
            {
                throw new NotFoundException(nameof(trackRecordDomain));
            }

            //if(userDomain.Role == "User")
            //{
            //    if (trackRecordDomain.UserId != userDomain.Id)
            //    {
            //        throw new UnauthorizedAccessException();
            //    }
            //}

            if (trackRecordDomain.UserId != userDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            _logger.LogInformation($"Found trackRecord Id: {trackRecordId}");
            return TrackRecordMapper.ToDto(trackRecordDomain);
        }

        public async Task<Dtos.TrackRecordMetrics> GetMetricsByUserIdAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var companyDomain = await LoadCompanyByIdAsync(companyId, cancellationToken);
            var userDomain = await LoadUserByIdAsync(companyId, userId);

            var queryable = _unitOfWork.TrackRecordRepository.AsQueryable();
            DateTime currentDate = DateTime.Now;
            var startDateOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endDateOfCurrentMonth = startDateOfCurrentMonth.AddMonths(1).AddDays(-1);
            queryable = queryable.Where(x => x.Date >= startDateOfCurrentMonth || x.Date <= endDateOfCurrentMonth);
            var trackRecords = await _unitOfWork.TrackRecordRepository.ToListAsync(queryable, cancellationToken);

            if(!userDomain.WorkScheduleId.HasValue)
            {
                throw new InvalidOperationException(nameof(userDomain.WorkScheduleId));
            }

            int workscheduleId = (int)userDomain.WorkScheduleId;
            var workSchedule = await _unitOfWork.WorkScheduleRepository.FindByIdAsync(workscheduleId, cancellationToken);
            if(workSchedule == null)
            {
                throw new NotFoundException(nameof(workSchedule));
            }

            double requiredMonthlyDays = 0; 
            double trackTimeCurrentMonth = 0; 
            double overtimeTrackTimeCurrentMonth = 0; 

            if (workSchedule.IsDefault)
            {
                var startHour = workSchedule.StartHour.HasValue ? workSchedule.StartHour.Value.Hour : 0;
                var endHour = workSchedule.EndHour.HasValue ? workSchedule.EndHour.Value.Hour : 0;
                requiredMonthlyDays = (endHour - startHour) * workSchedule.WorkDays!.Split(",").Length;
            }
            else
            {
                requiredMonthlyDays = (double)workSchedule.TotalWorkDays! * (double)workSchedule.Hours!;
            }

            foreach (var record in trackRecords)
            {
                var trackedHours = record.CheckOut.Hour - record.CheckIn.Hour;
                trackedHours -= record.BreakEnd.Hour - record.BreakStart.Hour;
                trackTimeCurrentMonth += trackedHours;
            }
            overtimeTrackTimeCurrentMonth = trackTimeCurrentMonth - requiredMonthlyDays;

            return new TrackRecordMetrics()
            {
                OvertimeTrackTimeCurrentMonth = overtimeTrackTimeCurrentMonth > 0 ? overtimeTrackTimeCurrentMonth : 0,
                RequiredMonthlyDays = requiredMonthlyDays,
                TrackTimeCurrentMonth  = trackTimeCurrentMonth
            };
        }

        public async  Task<Dtos.TrackRecord> UpdateAsync(Guid companyId, Guid trackRecordId, Dtos.TrackRecord trackRecord, CancellationToken cancellationToken = default)
        {
            Guid userId = Guid.NewGuid();
            var options = RepositoryOptions.AsTracking();
            if (trackRecord == null)
            {
                throw new ArgumentNullException(nameof(trackRecord));
            }

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            
            await LoadCompanyByIdAsync(companyId, cancellationToken);

            var userDomain = await LoadUserByIdAsync(companyId, userId);

            if (trackRecordId == default)
            {
                throw new ArgumentNullException(nameof(trackRecordId));
            }
            var trackRecordDomain = await _unitOfWork.TrackRecordRepository.FindByGlobalIdAsync(trackRecordId, options, cancellationToken: cancellationToken);
            if (trackRecordDomain == null)
            {
                throw new NotFoundException(nameof(trackRecordDomain));
            }

            if (trackRecordDomain.UserId != userDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            trackRecordDomain.BreakStart = trackRecord.BreakStart;
            trackRecordDomain.BreakEnd = trackRecord.BreakEnd;
            trackRecordDomain.CheckIn = trackRecord.CheckIn;
            trackRecordDomain.CheckOut = trackRecord.CheckOut;
            trackRecordDomain.Location = trackRecord.Location;
            trackRecordDomain.Note = trackRecord.Note;

            _unitOfWork.TrackRecordRepository.Update(trackRecordDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return TrackRecordMapper.ToDto(trackRecordDomain);
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
        
        private async Task<Domain.Company> LoadCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            return companyDomain;
        }
    }
}
