using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Business.Abstractions
{
    public interface ITeamService
    {
        Task<Dtos.Team> GetByIdAsync(Guid companyId, Guid teamId, CancellationToken cancellationToken = default);
        Task<Pagination<Dtos.Team>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<Dtos.Team> CreateAsync(Guid companyId, Dtos.Team team, CancellationToken cancellationToken = default);
        Task<Dtos.Team> UpdateAsync(Guid companyId, Guid teamId, Dtos.Team team, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid companyId, Guid teamId, CancellationToken cancellationToken = default);
    }
}
