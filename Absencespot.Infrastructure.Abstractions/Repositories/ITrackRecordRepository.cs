using Absencespot.Domain;
using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Repositories
{
    public interface ITrackRecordRepository : IBaseRepository<TrackRecord>
    {
        Task<IEnumerable<TrackRecord>?> FindByUserIdAsync(int userId, RepositoryOptions? options = null, CancellationToken cancellationToken = default);
    }
}
