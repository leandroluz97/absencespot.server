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
    public class SubscriptionConfiguration : BaseConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("Subscription");
            builder.HasMany<Company>(s => s.Companies)
                    .WithOne(c => c.Subcription)
                    .HasForeignKey(o => o.SubcriptionId); 

            builder.Property(o => o.Type)
                   .HasConversion(
                            type => type.DisplayName,
                            displayName => SubscriptionType.FromDisplayName<SubscriptionType>(displayName)
                    );

            base.Configure(builder);
        } 
    }
}
