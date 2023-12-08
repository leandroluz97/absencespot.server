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
                LeaveId = domain.Office.GlobalId,
                Allowance = Math.Round(domain.Allowance, 2),
                MonthCarryOverExpiresAfter = Math.Round(domain.MonthCarryOverExpiresAfter, 2),
                MonthMaxCarryOver = Math.Round(domain.MonthMaxCarryOver, 2),
                MonthlyAccrual = Math.Round(domain.MonthlyAccrual, 2),
            };
        }
    }
}
