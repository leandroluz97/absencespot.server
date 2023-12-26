using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Absencespot.SqlServer;
using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.UnitOfWork.Repositories
{
    public class TrackRecordRepository : BaseRepository<TrackRecord>, ITrackRecordRepository
    {
        public TrackRecordRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<TrackRecord>?> FindByUserIdAsync(int userId, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TrackRecord> source = AsQueryable(options)
                .Where(x => x.UserId == userId);

            var result = await ToListAsync(source, cancellationToken)
                .ConfigureAwait(false);
            return result;
        }
    }
}
