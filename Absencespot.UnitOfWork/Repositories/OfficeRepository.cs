﻿using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Absencespot.SqlServer;
using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.UnitOfWork.Repositories
{
    public class OfficeRepository : BaseRepository<Office>, IOfficeRepository
    {
        public OfficeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Office?> FindByGlobalIdAsync(Guid globalId, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Office> source = AsQueryable(options);
            source = source.Where(s => s.GlobalId == globalId);
            source = Include(source, s => s.Address);
            source = Include(source, s => s.Absences);
            source = IncludeThen<Domain.Absence, Domain.Leave>(source, a => a.Leave);
            source = Include(source, s => s.AvailableLeaves);
            source = IncludeThen<Domain.OfficeLeave, Domain.Leave>(source, s => s.Leave);
            var result = await FirstOrDefaultAsync(source, cancellationToken).ConfigureAwait(false);
            return result;
        }
        public async Task<Office?> FindByIdIncludedAsync(int id, RepositoryOptions? options = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Office> source = AsQueryable(options);
            source = source.Where(s => s.Id == id);
            source = Include(source, s => s.Address);
            source = Include(source, s => s.Absences);
            source = IncludeThen<Domain.Absence, Domain.Leave>(source, a => a.Leave);
            source = Include(source, s => s.AvailableLeaves);
            source = IncludeThen<Domain.OfficeLeave, Domain.Leave>(source, s => s.Leave);
            var result = await FirstOrDefaultAsync(source, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
