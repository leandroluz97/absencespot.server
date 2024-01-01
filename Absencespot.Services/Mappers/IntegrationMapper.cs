using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class IntegrationMapper
    {
        public static Domain.Integration ToDomain(Dtos.Integration dto)
        {
            return new Domain.Integration() 
            {
                IsActive = dto.IsActive,
                Provider = dto.Provider,
            };
        }
        public static Dtos.Integration ToDto(Domain.Integration domain)
        {
            return new Dtos.Integration() 
            {
                IsActive = domain.IsActive,
                Provider = domain.Provider,
            };
        }
    }
}
