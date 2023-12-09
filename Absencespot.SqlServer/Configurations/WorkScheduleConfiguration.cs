using Absencespot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absencespot.SqlServer.Configurations
{
    public class WorkScheduleConfiguration : BaseConfiguration<WorkSchedule>
    {
        public WorkScheduleConfiguration() { }
        public void Configure(EntityTypeBuilder<WorkSchedule> builder)
        {
            builder.ToTable("WorkSchedule");
            builder.Property(x => x.Name).HasMaxLength(256);

            //builder.Property(ws => ws.WorkDays)
            //        .HasConversion(
            //            v => string.Join(',', v),  // Convert ICollection<string> to string
            //            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());  // Convert string back to ICollection<string>

            base.Configure(builder);
            //builder.HasDiscriminator<string>("WorkSchedule")
            //        .HasValue<WorkScheduleDefault>("WorkScheduleDefault")
            //        .HasValue<WorkScheduleFlexible>("WorkScheduleFlexible");

        }
    }
}
