using Absencespot.Domain;
using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Repositories
{
    public interface ISettingsRepository : IBaseRepository<Settings>
    {
        Task<Settings> FindByCompanyIdAsync(int companyId, RepositoryOptions? options = null, CancellationToken cancellationToken = default);
    }
}
