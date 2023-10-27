using Absencespot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Absencespot.Domain.Enums;

namespace Absencespot.SqlServer.Configurations
{
    public class SettingsConfiguration : BaseConfiguration<Settings>
    {
        public void Configure(EntityTypeBuilder<Settings> builder)
        {
            builder.ToTable("Settings");
            builder.Property(o => o.DisplayMode)
                   .HasConversion(
                            type => type.DisplayName,
                            displayName => DisplayType.FromDisplayName<DisplayType>(displayName)
                    );

            base.Configure(builder);
        }
    }
}
