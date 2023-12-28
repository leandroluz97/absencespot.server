using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Business.Abstractions
{
    public interface ISettingsService
    {
        Task<Dtos.Settings> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<Dtos.Settings> UpdateAsync(Guid companyId, Dtos.Settings settings, CancellationToken cancellationToken = default);
    }
}
