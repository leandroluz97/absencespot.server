using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public static class OfficeMapper
    {
        public static Domain.Office ToDomain(Dtos.Office dto)
        {
            return new Domain.Office()
            {
                Address = AddressMapper.ToDomain(dto.Address),
                Name = dto.Name,
                IsEmployeeLeaveStartOnJobStartDate = dto.IsEmployeeLeaveStartOnJobStartDate,
                Absences = dto.Absences?.Select(AbsenceMapper.ToDomain).ToList(),
            };
        }


        public static Dtos.Office ToDto(Domain.Office domain)
        {
            return new Dtos.Office()
            {
                Id = domain.GlobalId,
                Address = domain.Address != null ? AddressMapper.ToDto(domain.Address) : null,
                Name= domain.Name,
                IsEmployeeLeaveStartOnJobStartDate = domain.IsEmployeeLeaveStartOnJobStartDate,
                Absences = domain.Absences?.Select(AbsenceMapper.ToDto).ToList(),
                StartDate = domain.StartDate,
                AvailableLeaves  = domain.AvailableLeaves?.Select(a => a.Leave.GlobalId)
            };
        }
    }
}
