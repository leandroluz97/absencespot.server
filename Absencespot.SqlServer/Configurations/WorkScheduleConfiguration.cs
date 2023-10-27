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
    public class WorkScheduleConfiguration : BaseConfiguration<WorkSchedule>
    {
        public void Configure(EntityTypeBuilder<WorkSchedule> builder)
        {
            builder.ToTable("WorkSchedule");
            builder.HasDiscriminator<string>("WorkSchedule")
                    .HasValue<WorkScheduleDefault>("WorkScheduleDefault")
                    .HasValue<WorkSchedulelFlexible>("WorkSchedulelFlexible");

            base.Configure(builder);
        }
    }
}
