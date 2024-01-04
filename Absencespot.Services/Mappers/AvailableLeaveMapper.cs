using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class AvailableLeaveMapper
    {
        public static Domain.AvailableLeave ToDomain(Dtos.AvailableLeave dto)
        {
            return new Domain.AvailableLeave() 
            { 
                AvailableDays = dto.AvailableDays,
                Period = dto.Period,
            };
        }
        public static Dtos.AvailableLeave ToDto(Domain.AvailableLeave domain)
        {
            return new Dtos.AvailableLeave() 
            { 
                AvailableDays = domain.AvailableDays,
                Period = domain.Period,
                AbsenceId = domain.Absence.GlobalId,
                LeaveId = domain.Absence.Leave.GlobalId,
            };
        }
    }
}
