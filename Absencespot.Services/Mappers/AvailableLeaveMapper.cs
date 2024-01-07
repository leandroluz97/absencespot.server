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
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };
        }
        public static Dtos.AvailableLeave ToDto(Domain.AvailableLeave domain)
        {
            return new Dtos.AvailableLeave() 
            { 
                AvailableDays = domain.AvailableDays,
                StartDate = domain.StartDate,
                EndDate = domain.EndDate,
                AbsenceId = domain.Absence.GlobalId,
                LeaveId = domain.Absence.Leave.GlobalId,
            };
        }
    }
}
