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
            if (leave == null)
            {
                throw new ArgumentNullException(nameof(leave));
            }
            leave.EnsureValidation();

            var leaveDomain = LeaveMapper.ToDomain(leave);

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

            var queryable = _unitOfWork.LeaveRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(l => l.Company.GlobalId == companyId);

            var totalLeave = queryable.Count();

            queryable = queryable.Skip(pageNumber * pageSize).Take(pageSize);
            var leaves = await _unitOfWork.LeaveRepository.ToListAsync(queryable, cancellationToken);

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
            if (leaveId == default)
            {
                throw new ArgumentNullException(nameof(leaveId));
            }

            var leaveDomain = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(leaveId);
            if(leaveDomain == null)
            {
                throw new NotFoundException(nameof(leaveDomain));
            }

            return LeaveMapper.ToDto(leaveDomain);
        }

        public async Task<Leave> UpdateAsync(Guid companyId, Guid leaveId, Leave leave, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
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

            var leaveDomain = await _unitOfWork.LeaveRepository.FindByGlobalIdAsync(leaveId);
            if( leaveDomain == null)
            {
                throw new NotFoundException(nameof(leaveDomain));
            }

            return LeaveMapper.ToDto(leaveDomain);
        }
    }
}
