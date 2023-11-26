using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class AbsenceMapper
    {
        public static Domain.Absence ToDomain(Dtos.Absence dto)
        {
            return new Domain.Absence()
            {
                Allowance = dto.Allowance,
                MonthlyAccrual =   dto.MonthlyAccrual,
                MonthCarryOverExpiresAfter = dto.MonthCarryOverExpiresAfter,
                MonthMaxCarryOver = dto.MonthMaxCarryOver,
            };
        }


        public static Dtos.Absence ToDto(Domain.Absence domain)
        {
            return new Dtos.Absence()
            {
                Allowance = domain.Allowance,
                MonthCarryOverExpiresAfter = domain.MonthCarryOverExpiresAfter,
                MonthMaxCarryOver = domain.MonthMaxCarryOver,
                MonthlyAccrual = domain.MonthlyAccrual,
            };
        }
    }
}
