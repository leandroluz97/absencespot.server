using Absencespot.Domain;
using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Repositories
{
    public interface IAvailableLeaveRepository : IBaseRepository<AvailableLeave>
    {
        Task<IEnumerable<AvailableLeave>?> FindByUserIdAsync(int userId, RepositoryOptions? options = null, CancellationToken cancellationToken = default);
    }
}
