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
    public class AvailableLeaveConfiguration : BaseConfiguration<AvailableLeave>, IEntityTypeConfiguration<AvailableLeave>
    {
        public AvailableLeaveConfiguration() { }
        public void Configure(EntityTypeBuilder<AvailableLeave> builder)
        {
            builder.ToTable("AvailableLeave");
            builder.Property(a => a.AvailableDays).HasPrecision(3, 2);
            base.Configure(builder);
        }
    }
}
