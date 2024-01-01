using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class TrackRecordMapper
    {
        public static Domain.TrackRecord ToDomain(Dtos.TrackRecord dto)
        {
            return new Domain.TrackRecord() 
            {
                Date = dto.Date,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                BreakStart = dto.BreakStart,
                BreakEnd = dto.BreakEnd,
                Location = dto.Location,
                Note = dto.Note,
            };
        }
        public static Dtos.TrackRecord ToDto(Domain.TrackRecord domain)
        {
            return new Dtos.TrackRecord() 
            {
                Id = domain.GlobalId,
                Date = domain.Date,
                CheckIn = domain.CheckIn,
                CheckOut = domain.CheckOut,
                BreakStart = domain.BreakStart, 
                BreakEnd = domain.BreakEnd, 
                Location = domain.Location,
                Note = domain.Note,
            };
        }
    }
}

