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
    public class LeaveService : ILeaveService
    {
        private readonly ILogger<LeaveService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public LeaveService(ILogger<LeaveService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Dtos.Leave> CreateAsync(Guid companyId, Dtos.Leave leave, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(
                companyId,
                RepositoryOptions.AsTracking(),
                cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }
            if (leave == null)
            {
                throw new ArgumentNullException(nameof(leave));
            }
            leave.EnsureValidation();

            var queryable = _unitOfWork.LeaveRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(
                q => q.Company.GlobalId == companyId &&
                q.Name.ToLower() == leave.Name.ToLower()
                );

            var foundLeave = await _unitOfWork.LeaveRepository.FirstOrDefaultAsync(queryable);
            if (foundLeave != null)
            {
                throw new ConflictException(foundLeave.Name);
            }
            var leaveDomain = LeaveMapper.ToDomain(leave);
            leaveDomain.Company = companyDomain;

            foreach (var id in leave.OfficeIds)
            {
                var office = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(id, RepositoryOptions.AsTracking());
                if (office == null)
                {
                    throw new NotFoundException(nameof(office));
                }
                if (leaveDomain.OfficesLeaves == null)
                {
                    leaveDomain.OfficesLeaves = new List<Domain.OfficeLeave>();
                }
                leaveDomain.OfficesLeaves.Add(new Domain.OfficeLeave()
                {
                    Office = office,
                    Leave = leaveDomain
                });
            }

            leaveDomain = _unitOfWork.LeaveRepository.Add(leaveDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Saved leave {leaveDomain.GlobalId}");

            return LeaveMapper.ToDto(leaveDomain);
        }

        public async Task<Pagination<Leave>> GetAllAsync(
            Guid companyId,
            int pageNumber = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }
            //Guid officeId = Guid.Empty;

            var queryable = _unitOfWork.LeaveRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(l => l.Company.GlobalId == companyId);
            //queryable = queryable.Where(l => l.Company.GlobalId == companyId && l.OfficesLeaves.Any(of => of.Office.GlobalId == officeId));

            var totalLeave = queryable.Count();

            queryable = queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            queryable = _unitOfWork.LeaveRepository.Include(queryable, x => x.OfficesLeaves);
            queryable = _unitOfWork.LeaveRepository.IncludeThen<Domain.OfficeLeave, Domain.Office>(queryable, x => x.Office);
            var leaves = await _unitOfWork.LeaveRepository.ToListAsync(queryable, cancellationToken);

            _logger.LogInformation($"Get leaves pageSize: {pageSize}, pageNumber: {pageNumber}");

            return new Pagination<Dtos.Leave>()
            {
                TotalRecords = totalLeave,
                TotalPages = (int)Math.Ceiling((decimal)totalLeave / (decimal)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = leaves.Select(LeaveMapper.ToDto)
            };
        }

        public async Task<Leave> GetByIdAsync(Guid companyId, Guid leaveId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            if (leaveId == default)
            {
                throw new ArgumentNullException(nameof(leaveId));
            }

            var leaveDomain = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(leaveId);
            if (leaveDomain == null)
            {
                throw new NotFoundException(nameof(leaveDomain));
            }

            _logger.LogInformation($"Found leave Id: {leaveId}");

            return LeaveMapper.ToDto(leaveDomain);
        }

        public async Task<Leave> UpdateAsync(Guid companyId, Guid leaveId, Leave leave, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }
            if (leaveId == default)
            {
                throw new ArgumentNullException(nameof(leaveId));
            }
            if (leave == null)
            {
                throw new ArgumentNullException(nameof(leave));
            }
            leave.EnsureValidation();

            var queryable = _unitOfWork.LeaveRepository.AsQueryable(RepositoryOptions.AsTracking());
            queryable = queryable.Where(x => x.Company.GlobalId == companyId && x.GlobalId == leaveId);
            queryable =  _unitOfWork.LeaveRepository.Include(queryable, x => x.OfficesLeaves);
            queryable = _unitOfWork.LeaveRepository.IncludeThen<Domain.OfficeLeave, Domain.Office>(queryable, x => x.Office);
            var leaveDomain = await _unitOfWork.LeaveRepository.FirstOrDefaultAsync(queryable, cancellationToken);

            if (leaveDomain == null)
            {
                throw new ArgumentNullException(nameof(leaveDomain));
            }

            foreach (Domain.OfficeLeave officeLeave in leaveDomain?.OfficesLeaves)
            {
                if (!leave.OfficeIds.Any(id => id == officeLeave.Office?.GlobalId))
                {
                    leaveDomain.OfficesLeaves.Remove(officeLeave);
                }
            }

            foreach (Guid id in leave.OfficeIds)
            {
                if (!leaveDomain.OfficesLeaves.Any(of => of.Office.GlobalId == id))
                {
                    var office = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(id, RepositoryOptions.AsTracking());
                    if (office == null)
                    {
                        throw new NotFoundException(nameof(office));
                    }
                    leaveDomain.OfficesLeaves ??= new List<Domain.OfficeLeave>();
                    leaveDomain.OfficesLeaves.Add(new Domain.OfficeLeave
                    {
                        Office = office,
                        Leave = leaveDomain
                    });
                }
            }

            leaveDomain.Name = leave.Name;
            leaveDomain.Color = leave.Color;
            leaveDomain.Icon = leave.Icon;
            leaveDomain.IsActive = leave.IsActive;
            leaveDomain.IsReasonRequired = leave.IsReasonRequired;
            leaveDomain.IsLimitedQuota = leave.IsLimitedQuota;
            leaveDomain.YearlyQuota = leave.YearlyQuota;

            _unitOfWork.LeaveRepository.Update(leaveDomain);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Updated leave Id: {leaveId}");

            return LeaveMapper.ToDto(leaveDomain);
        }


        public async Task DeleteAsync(Guid companyId, Guid leaveId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var leaveDomain = await LoadById(leaveId, RepositoryOptions.AsNoTracking());

            _unitOfWork.LeaveRepository.Remove(leaveDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Deleted leave Id: {leaveId}");
        }

        private async Task<Domain.Leave> LoadById(Guid leaveId, RepositoryOptions options, CancellationToken cancellationToken = default)
        {
            if (leaveId == default)
            {
                throw new ArgumentNullException(nameof(leaveId));
            }

            Domain.Leave? leaveDomain;
            if (options == null)
            {
                leaveDomain = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(leaveId, RepositoryOptions.AsNoTracking(), cancellationToken);
            }
            else
            {
                leaveDomain = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(leaveId, RepositoryOptions.AsTracking(), cancellationToken);
            }
            if (leaveDomain == null)
            {
                throw new NotFoundException(nameof(leaveDomain));
            }
            return leaveDomain;
        }
    }
}
