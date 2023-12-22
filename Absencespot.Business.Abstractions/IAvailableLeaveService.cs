using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Business.Abstractions
{
    public interface IAvailableLeaveService
    {
        Task<IEnumerable<Dtos.AvailableLeave>> GetAllAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default);
    }
}
