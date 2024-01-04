using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class RequestMapper
    {
        public static Domain.Request ToDomain(Dtos.Request dto)
        {
            return new Domain.Request() 
            { 
                EndDate = dto.EndDate,
                StartDate = dto.StartDate,
                Note = dto.Note,
                Status =  dto.Status,
            };
        }
        public static Dtos.Request ToDto(Domain.Request domain)
        {
            return new Dtos.Request() 
            { 
                StartDate = domain.StartDate,
                EndDate = domain.EndDate,
                Note = domain.Note,
                Status = domain.Status,
                LeaveId = domain.Leave.GlobalId,
                ApproverId = domain.Approver.GlobalId,
                OnBehalfOfId = domain.OnBehalfOf.GlobalId,
                UserId = domain.User.GlobalId,
                OfficeId = domain.User.Office.GlobalId,
            };
        }
    }
}
