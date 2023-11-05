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
        public SubscriptionConfiguration(){}
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("Subscription");
            builder.Property(l => l.UnitPrice).HasPrecision(5, 2).IsRequired();
            builder.Property(o => o.Type);
                   //.HasConversion(
                   // t => t.Name,
                   // t => SubscriptionType.FromDisplayName<SubscriptionType>(t));

            builder.HasMany<Company>(s => s.Companies)
                    .WithOne(c => c.Subcription)
                    .HasForeignKey(o => o.SubcriptionId); 

            base.Configure(builder);

        } 
    }
}
