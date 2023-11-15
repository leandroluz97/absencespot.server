using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public static class CompanyMapper
    {
        public static Domain.Company ToDomain(Dtos.Company dto)
        {
            return new Domain.Company()
            {
                Name = dto.Name,
                FiscalNumber = string.IsNullOrWhiteSpace(dto.FiscalNumber) ? null : dto.FiscalNumber,
                EmailContact = dto.EmailContact,
                Industry = dto.Industry,
                IsActive = dto.IsActive,
            };
        }
        public static Dtos.Company ToDto(Domain.Company domain)
        {
            return new Dtos.Company()
            {
                SubscriptionId = domain.Subcription.GlobalId,
                Id = domain.GlobalId,
                Name = domain.Name,
                FiscalNumber = string.IsNullOrWhiteSpace(domain.FiscalNumber) ? null : domain.FiscalNumber,
                EmailContact = domain.EmailContact,
                Industry = domain.Industry,
                IsActive = domain.IsActive,
            };
        }
    }
}
