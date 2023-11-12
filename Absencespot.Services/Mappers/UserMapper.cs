using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public static class UserMapper
    {
        public static Domain.User ToDomain(Dtos.User dto)
        {
            return new Domain.User() { };
        }
        public static Dtos.User ToDto(Domain.User domain)
        {
            return new Dtos.User() { };
        }
    }
}
