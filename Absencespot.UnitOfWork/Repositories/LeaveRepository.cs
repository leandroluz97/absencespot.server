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
    public class LeaveRepository : BaseRepository<Leave>, ILeaveRepository
    {
        public LeaveRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public override async Task<Leave?> FindByGlobalIdAsync(Guid globalId, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Leave> source = AsQueryable(options);
            source = source.Where(s => s.GlobalId == globalId);
            source = Include(source, s => s.OfficesLeaves);
            source = IncludeThen<Domain.OfficeLeave, Domain.Office>(source, s => s.Office);
            var result = await FirstOrDefaultAsync(source, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
