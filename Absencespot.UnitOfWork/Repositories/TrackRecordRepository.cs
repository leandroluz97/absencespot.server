using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Absencespot.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.UnitOfWork.Repositories
{
    public class TrackRecordRepository : BaseRepository<TrackRecord>, ITrackRecordRepository
    {
        public TrackRecordRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
