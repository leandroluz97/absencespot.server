using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public static class Subscription
    {
        public static Domain.Subscription ToDomain(Dtos.Subscription dto)
        {
            return new Domain.Subscription() 
            { 
       
            };
        }
        public static Dtos.Subscription ToDto(Domain.Subscription domain)
        {
            return new Dtos.Subscription() { };
        }
    }
}
