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
    internal class AbsenceConfiguration : BaseConfiguration<Absence>, IEntityTypeConfiguration<Absence>
    {
        public void Configure(EntityTypeBuilder<Absence> builder)
        {
            builder.ToTable("Absence");
            builder.Property(a => a.Allowance).HasPrecision(3, 2);
            builder.Property(a => a.MonthlyAccrual).HasPrecision(3, 2);
            builder.Property(a => a.MonthCarryOverExpiresAfter).HasPrecision(3, 2);
            builder.Property(a => a.MonthMaxCarryOver).HasPrecision(3, 2);
            base.Configure(builder);
        }
    }
}
