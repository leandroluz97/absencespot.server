using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Absencespot;

namespace Absencespot.Business.Abstractions
{
    public interface ICompanyService
    {
        Task<Dtos.Company> CreateAsync(Guid subscriptionId, Dtos.Company company, CancellationToken cancellationToken);
        Task<Dtos.Company> GetByIdAsync(Guid companyId, CancellationToken cancellationToke);
        Task<Dtos.Company> UpdateAsync(Guid companyId, Dtos.Company company, CancellationToken cancellationToke);
        Task DeleteAsync(Guid companyId, CancellationToken cancellationToke);
    }
}
