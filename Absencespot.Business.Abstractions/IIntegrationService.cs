using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Business.Abstractions
{
    public interface IIntegrationService
    {
        Task<IEnumerable<Dtos.Integration>> GetAllAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<Dtos.Integration> UpdateAsync(Guid companyId, Dtos.Integration integration, CancellationToken cancellationToken = default);
    }
}
