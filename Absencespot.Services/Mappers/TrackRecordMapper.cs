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
            return new Domain.TrackRecord() { };
        }
        public static Dtos.TrackRecord ToDto(Domain.TrackRecord domain)
        {
            return new Dtos.TrackRecord() { };
        }
    }
}
