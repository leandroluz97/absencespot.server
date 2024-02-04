using Absencespot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absencespot.SqlServer.Configurations
{
    public class SubscriptionConfiguration : BaseConfiguration<Subscription>
    {
        public SubscriptionConfiguration(){}
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("Subscription");
            builder.Property(l => l.UnitPrice)
                   .HasPrecision(5, 2)
                   .IsRequired();
            builder.Property(o => o.Type);
            //builder.HasOne<Company>(s => s.Company)
            //       .WithOne(c => c.Subcription)
            //       .HasForeignKey(s => );

            base.Configure(builder);
        } 
    }
}
