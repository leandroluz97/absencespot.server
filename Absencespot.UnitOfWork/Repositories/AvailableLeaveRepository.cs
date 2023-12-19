using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Absencespot.SqlServer;

namespace Absencespot.UnitOfWork.Repositories
{
    public class AvailableLeaveRepository : BaseRepository<AvailableLeave>, IAvailableLeaveRepository
    {
        public AvailableLeaveRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
