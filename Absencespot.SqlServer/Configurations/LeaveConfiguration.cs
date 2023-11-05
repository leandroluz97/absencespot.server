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
        public LeaveConfiguration(){}
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            builder.ToTable("Leave");
            builder.Property(l => l.Name).HasMaxLength(100);
            builder.Property(l => l.Color).HasMaxLength(7);
            builder.Property(l => l.IsActive).IsRequired();
            builder.Property(l => l.YearlyQuota).HasPrecision(3,2).IsRequired(false);

            builder.HasMany<OfficeLeave>(c => c.OfficesLeaves)
                    .WithOne(i => i.Leave)
                    .HasForeignKey(o => o.LeaveId)
                    .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasMany<Request>(c => c.Requests)
                    .WithOne(i => i.Leave)
                    .HasForeignKey(o => o.LeaveId)
                    .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasMany<Absence>(c => c.Absences)
                    .WithOne(i => i.Leave)
                    .HasForeignKey(o => o.LeaveId)
                    .OnDelete(DeleteBehavior.ClientCascade);

            base.Configure(builder);
        }
    }
}
