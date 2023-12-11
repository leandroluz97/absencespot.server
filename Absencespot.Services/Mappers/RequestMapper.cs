using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class RequestMapper
    {
        public static Domain.Request ToDomain(Dtos.Request dto)
        {
            return new Domain.Request() { };
        }
        public static Dtos.Request ToDto(Domain.Request domain)
        {
            return new Dtos.Request() { };
        }
    }
}
