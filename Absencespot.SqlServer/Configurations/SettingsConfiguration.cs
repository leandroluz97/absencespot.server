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
            builder.Property(l => l.PrivacyMode).IsRequired();
            builder.Property(l => l.OnBehalfOf).IsRequired();
            builder.Property(l => l.IsLoginFromEmailLinkRequired).IsRequired();
            builder.Property(l => l.DateFormat).IsRequired();
            builder.Property(l => l.TimeZone).IsRequired();
            builder.Property(l => l.DisplayBirthday).IsRequired();
            builder.Property(o => o.DisplayMode)
                   .HasConversion(
                            type => type.DisplayName,
                            displayName => DisplayType.FromDisplayName<DisplayType>(displayName)
                    );

            base.Configure(builder);
        }
    }
}
