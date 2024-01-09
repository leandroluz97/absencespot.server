using Absencespot.Domain;
using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Repositories
{
    public interface IOfficeRepository : IBaseRepository<Office>
    {
        public Task<Office?> FindByIdIncludedAsync(int id, RepositoryOptions? options = null, CancellationToken cancellationToken = default);
    }
}
