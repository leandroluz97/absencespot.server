using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Absencespot.SqlServer;
using Absencespot.Utils;
using Microsoft.EntityFrameworkCore;

namespace Absencespot.UnitOfWork.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public override async Task<Company?> FindByGlobalIdAsync(Guid globalId, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Company> source = AsQueryable(options)
                .Where(s => s.GlobalId == globalId)
                .Include(c => c.Subcription);
           
            var result = await FirstOrDefaultAsync(source, cancellationToken)
                .ConfigureAwait(false);
            return result;
        }
    }
}
