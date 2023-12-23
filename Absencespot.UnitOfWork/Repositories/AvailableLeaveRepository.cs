using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Absencespot.SqlServer;
using Absencespot.Utils;

namespace Absencespot.UnitOfWork.Repositories
{
    public class AvailableLeaveRepository : BaseRepository<AvailableLeave>, IAvailableLeaveRepository
    {
        public AvailableLeaveRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<AvailableLeave>?> FindByUserIdAsync(int userId, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<AvailableLeave> source = AsQueryable(options)
                .Where(x => x.UserId == userId);

            var result = await ToListAsync(source, cancellationToken)
                .ConfigureAwait(false);
            return result;
        }
    }
}
