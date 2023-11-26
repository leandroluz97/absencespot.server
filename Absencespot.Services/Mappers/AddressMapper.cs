using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class AddressMapper
    {
        public static Domain.Address ToDomain(Dtos.Address dto)
        {
            return new Domain.Address()
            {
                City = dto.City,
                Country = dto.Country,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                AddressLine3 = dto.AddressLine3,
                CountryCode = dto.CountryCode,
            };
        }


        public static Dtos.Address ToDto(Domain.Address domain)
        {
            return new Dtos.Address()
            {
                City = domain.City,
                Country = domain.Country,
                AddressLine1 = domain.AddressLine1,
                AddressLine2 = domain.AddressLine2,
                AddressLine3 = domain.AddressLine3,
                CountryCode = domain.CountryCode,
            };
        }
    }
}
