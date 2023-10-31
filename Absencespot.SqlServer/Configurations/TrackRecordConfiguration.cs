using Absencespot.Domain;
using Absencespot.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.SqlServer.Configurations
{
    public class TrackRecordConfiguration : BaseConfiguration<TrackRecord>
    {
        public void Configure(EntityTypeBuilder<TrackRecord> builder)
        {
            builder.ToTable("TrackRecord");
            builder.Property(o => o.Location) 
                   .HasConversion(
                            type => type.DisplayName,
                            displayName => LocationType.FromDisplayName<LocationType>(displayName)
                    );

            base.Configure(builder);
        }
    }
}
