using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class WorkScheduleFlexibleMapper
    {
        public static Domain.WorkScheduleFlexible ToDomain(Dtos.WorkScheduleFlexible dto)
        {
            return new Domain.WorkScheduleFlexible()
            {
                Name = dto.Name,
                Description = dto.Description,
                Hours = dto.Hours,
                IsDefault = dto.IsDefault,
                TotalWorkDays = dto.WorkDays,
            };
        }
        public static Dtos.WorkScheduleFlexible ToDto(Domain.WorkScheduleFlexible domain)
        {
            return new Dtos.WorkScheduleFlexible()
            {
                Id = domain.GlobalId,
                Name = domain.Name,
                Description = domain.Description,
                Hours = domain.Hours,
                IsDefault = domain.IsDefault,
                WorkDays = domain.TotalWorkDays,
            };
        }
    }
}
