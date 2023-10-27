using Absencespot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.SqlServer.Configurations
{
    public class LeaveConfiguration : BaseConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            builder.ToTable("Leave");
            builder.HasMany<OfficeLeave>(c => c.OfficesLeaves)
                    .WithOne(i => i.Leave)
                    .HasForeignKey(o => o.LeaveId);

            base.Configure(builder);
        }
    }
}
