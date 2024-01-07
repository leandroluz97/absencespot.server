using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Absencespot.SqlServer;
using Absencespot.Utils;

namespace Absencespot.UnitOfWork.Repositories
{
    public class RequestRepository : BaseRepository<Request>, IRequestRepository
    {
        public RequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Request?> FindByGlobalIdAsync(Guid globalId, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Request> source = AsQueryable(options);
            source = source.Where(s => s.GlobalId == globalId);
            source = Include(source, s => s.Leave);
            source = Include(source, s => s.OnBehalfOf);
            source = Include(source, s => s.User);
            source = IncludeThen<Domain.User, Domain.Office>(source, o => o.Office);
            var result = await FirstOrDefaultAsync(source, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
