using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Business.Abstractions
{
    public interface IOfficeService
    {
        Task<Dtos.Office> GetByIdAsync(Guid companyId, Dtos.Office office, CancellationToken cancellationToken = default);
        Task<Pagination<Dtos.Office>> GetAllAsync(Guid companyId, Dtos.Office office, CancellationToken cancellationToken = default);
        Task<Dtos.Office> CreateAsync(Guid companyId, Dtos.Office office, CancellationToken cancellationToken = default);
        Task<Dtos.Office> UpdateAsync(Guid companyId, Dtos.Office office, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid companyId, Guid officeId, CancellationToken cancellationToken = default);
    }
}
