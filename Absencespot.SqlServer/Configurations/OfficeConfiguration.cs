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
    public class OfficeConfiguration : BaseConfiguration<Office>
    {
        public void Configure(EntityTypeBuilder<Office> builder)
        {
            builder.ToTable("Office");
            builder.HasMany<OfficeLeave>(c => c.AvailableLeaves)
                    .WithOne(i => i.Office)
                    .HasForeignKey(o => o.OfficeId);

            //builder.HasMany<Absence>(c => c.AvailableLeaves)
            //        .WithOne(i => i.Office)
            //        .HasForeignKey(o => o.OfficeId);

            builder.HasOne(c => c.Address)
                    .WithOne(i => i.Office)
                    .HasForeignKey<Address>(o => o.OfficeId);

            base.Configure(builder);
        }
    }
}
