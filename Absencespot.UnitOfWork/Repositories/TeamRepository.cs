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
    public class TeamRepository : BaseRepository<Team>, ITeamRepository
    {
        public TeamRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Team?> FindByGlobalIdAsync(Guid globalId, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Team> source = AsQueryable(options);
            source = source.Where(s => s.GlobalId == globalId);
            source = Include(source, s => s.Users);
            source = IncludeThen<Domain.UserTeam, Domain.User>(source, s => s.User);
            var result = await FirstOrDefaultAsync(source, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
