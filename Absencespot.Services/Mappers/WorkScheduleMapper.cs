using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class WorkScheduleMapper
    {
        public static Domain.WorkSchedule ToDomain(Dtos.WorkSchedule dto)
        {
            return new Domain.WorkSchedule()
            {
                Name = dto.Name,
                Description = dto.Description,
                IsDefault = dto.IsDefault,
                StartHour = dto.StartHour,
                WorkDays = dto.WorkDays != null ? string.Join(",", dto.WorkDays) : null,
                EndHour = dto.EndHour,
                Hours = dto.Hours,
                TotalWorkDays = dto.TotalWorkDays,
            };
        }
        public static Dtos.WorkSchedule ToDto(Domain.WorkSchedule domain)
        {
            return new Dtos.WorkSchedule()
            {
                Id = domain.GlobalId,
                Name = domain.Name,
                Description = domain.Description,
                IsDefault = domain.IsDefault,
                StartHour = domain.StartHour,
                WorkDays = domain.WorkDays?.Split(","),
                EndHour = domain.EndHour,
                Hours = domain.Hours,
                TotalWorkDays = domain.TotalWorkDays,
            };

        }
    }
}

