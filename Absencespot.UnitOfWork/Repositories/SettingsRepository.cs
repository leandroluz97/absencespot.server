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
    public class SettingsRepository : BaseRepository<Settings>, ISettingsRepository
    {
        public SettingsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Settings>? FindByCompanyIdAsync(int companyId, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Settings> source = AsQueryable(options);
            source = source.Where(s => s.Id == companyId);
            var result = await FirstOrDefaultAsync(source, cancellationToken)
                .ConfigureAwait(false);
            return result;
        }
    }
}
