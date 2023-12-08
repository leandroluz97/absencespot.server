using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class TeamMapper
    {
        public static Domain.Team ToDomain(Dtos.Team dto)
        {
            return new Domain.Team()
            {
                Name = dto.Name,
                Description = dto.Description,
                IsAutoApproved = dto.IsAutoApproved,
            };
        }


        public static Dtos.Team ToDto(Domain.Team domain)
        {
            return new Dtos.Team()
            {
                Id = domain.GlobalId,
                Name = domain.Name,
                Description = domain.Description,
                IsAutoApproved = domain.IsAutoApproved,
                Users = domain.Users?.Select(u => new Dtos.BaseUser()
                {
                    Id = u.User.GlobalId,
                    IsManager = domain.Users.Any(user => user.IsManager && user.GlobalId == u.GlobalId),
                    Email = u.User.Email,
                    FirstName = u.User.FirstName,
                    LastName = u.User.LastName,
                })
            };
        }
    }
}
