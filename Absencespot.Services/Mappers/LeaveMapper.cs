using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class LeaveMapper
    {
        public static Domain.Leave ToDomain(Dtos.Leave dto)
        {
            return new Domain.Leave()
            {
                Name = dto.Name,
                Color = dto.Color,
                IsActive = dto.IsActive,
                IsReasonRequired = dto.IsReasonRequired,
                IsApprovalRequired = dto.IsApprovalRequired,
                IsLimitedQuota = dto.IsLimitedQuota,
                Icon = dto.Icon,
                YearlyQuota = dto.YearlyQuota,
            };
        }


        public static Dtos.Leave ToDto(Domain.Leave domain)
        {
            return new Dtos.Leave()
            {
                Name = domain.Name,
                Color = domain.Color,
                IsActive = domain.IsActive,
                IsReasonRequired = domain.IsReasonRequired,
                IsApprovalRequired = domain.IsApprovalRequired,
                IsLimitedQuota = domain.IsLimitedQuota,
                Icon = domain.Icon,
                YearlyQuota = domain.YearlyQuota,
            };
        }
    }
}
