using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class WorkScheduleDefaultMapper
    {
        public static Domain.WorkScheduleDefault ToDomain(Dtos.WorkScheduleDefault dto)
        {
            return new Domain.WorkScheduleDefault()
            {
                IsDefault = dto.IsDefault,
                WorkDays = dto.WorkDays,
                Description = dto.Description,
                StartHour = dto.StartHour,
                Name = dto.Name,
            };
        }
        public static Dtos.WorkScheduleDefault ToDto(Domain.WorkScheduleDefault domain)
        {

            return new Dtos.WorkScheduleDefault()
            {
                Id = domain.GlobalId,
                WorkDays = domain.WorkDays,
                Name = domain.Name,
                StartHour = domain.StartHour,
                Description = domain.Description,
                IsDefault = domain.IsDefault,
                EndHour = domain.EndHour,
            };
        }
    }
}

