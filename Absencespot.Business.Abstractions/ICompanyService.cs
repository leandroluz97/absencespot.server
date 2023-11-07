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
        Task<Dtos.Company> CreateAsync(Guid subscriptionId, Dtos.Company company);
        Task<Dtos.Company> GetByIdAsync(Guid companyId);
        Task<Dtos.Company> UpdateAsync(Dtos.Company company);
        Task DeleteAsync(Guid companyId);
    }
}
