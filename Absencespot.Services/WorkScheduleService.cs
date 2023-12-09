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
    public class WorkScheduleService : IWorkScheduleService
    {
        private readonly ILogger<WorkScheduleService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public WorkScheduleService(ILogger<WorkScheduleService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Dtos.WorkSchedule> CreateAsync(Guid companyId, Dtos.WorkSchedule workSchedule, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (workSchedule == null)
            {
                throw new ArgumentNullException(nameof(workSchedule));
            }
            workSchedule.EnsureValidation();

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, RepositoryOptions.AsTracking());
            if (companyDomain == null)
            {
                throw new NotFoundException($"Not found {companyId}");
            }

            var queryable = _unitOfWork.WorkScheduleRepository.AsQueryable();
            queryable = queryable.Where(q => q.Company.GlobalId == companyId && q.Name.ToLower() == workSchedule.Name.ToLower());
            var workScheduleDomain = await _unitOfWork.WorkScheduleRepository.FirstOrDefaultAsync(queryable, cancellationToken);
            if (workScheduleDomain is not null)
            {
                throw new ConflictException(nameof(workSchedule));
            }

            workScheduleDomain = WorkScheduleMapper.ToDomain(workSchedule);
            workScheduleDomain.Company = companyDomain;
            workScheduleDomain = _unitOfWork.WorkScheduleRepository.Add(workScheduleDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Created workSchedule Id {workScheduleDomain.GlobalId}");

            return WorkScheduleMapper.ToDto(workScheduleDomain);
        }

        public async Task DeleteAsync(Guid companyId, Guid workScheduleId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (workScheduleId == default)
            {
                throw new ArgumentNullException(nameof(workScheduleId));
            }

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var WorkScheduleDomain = await _unitOfWork.WorkScheduleRepository.FindByGlobalIdAsync(workScheduleId);
            if (WorkScheduleDomain == null)
            {
                throw new NotFoundException(nameof(WorkScheduleDomain));
            }

            if (WorkScheduleDomain.CompanyId != companyDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            _unitOfWork.WorkScheduleRepository.Remove(WorkScheduleDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Deleted WorkSchedule Id: {workScheduleId}");
        }

        public async Task<Pagination<Dtos.WorkSchedule>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
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

            var queryable = _unitOfWork.WorkScheduleRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(l => l.Company.GlobalId == companyId);

            var totalWorkSchedules = queryable.Count();
            queryable = queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var workSchedules = await _unitOfWork.WorkScheduleRepository.ToListAsync(queryable, cancellationToken);


            _logger.LogInformation($"Get workschedule pageSize: {pageSize}, pageNumber: {pageNumber}");

            return new Pagination<Dtos.WorkSchedule>()
            {
                TotalRecords = totalWorkSchedules,
                TotalPages = (int)Math.Ceiling((decimal)totalWorkSchedules / (decimal)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = workSchedules.Select(WorkScheduleMapper.ToDto)
            };
        }

        public async Task<Dtos.WorkSchedule> GetByIdAsync(Guid companyId, Guid workScheduleId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (workScheduleId == default)
            {
                throw new ArgumentNullException(nameof(workScheduleId));
            }

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var workScheduleDomain = await _unitOfWork.WorkScheduleRepository.FindByGlobalIdAsync(workScheduleId);
            if (workScheduleDomain == null)
            {
                throw new NotFoundException(nameof(workScheduleDomain));
            }

            _logger.LogInformation($"Found WorkSchedule Id: {workScheduleId}");

            return WorkScheduleMapper.ToDto(workScheduleDomain);
        }

        public async Task<Dtos.WorkSchedule> UpdateAsync(Guid companyId, Guid workScheduleId, Dtos.WorkSchedule workSchedule, CancellationToken cancellationToken = default)
        {
            if (workSchedule == null)
            {
                throw new ArgumentNullException(nameof(workSchedule));
            }

            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            if (workScheduleId == default)
            {
                throw new ArgumentNullException(nameof(workScheduleId));
            }
            var workScheduleDomain = await _unitOfWork.WorkScheduleRepository.FindByGlobalIdAsync(workScheduleId, cancellationToken: cancellationToken);
            if (workScheduleDomain == null)
            {
                throw new NotFoundException(nameof(workScheduleDomain));
            }

            if (workScheduleDomain.CompanyId != companyDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            if (workScheduleDomain.IsDefault)
            {
                workScheduleDomain.Name = workSchedule.Name;
                workScheduleDomain.Description = workSchedule.Description;
                workScheduleDomain.EndHour = workSchedule.EndHour;
                workScheduleDomain.StartHour = workSchedule.StartHour;
                workScheduleDomain.WorkDays = string.Join(",", workSchedule.WorkDays);
            }
            else
            {
                workScheduleDomain.Name = workSchedule.Name;
                workScheduleDomain.Description = workSchedule.Description;
                workScheduleDomain.Hours = workSchedule.Hours;
                workScheduleDomain.TotalWorkDays = workSchedule.TotalWorkDays;
            }

            _unitOfWork.WorkScheduleRepository.Update(workScheduleDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Updated WorkSchedule Id: {workScheduleId}");

            return WorkScheduleMapper.ToDto(workScheduleDomain);
        }
    }
}
