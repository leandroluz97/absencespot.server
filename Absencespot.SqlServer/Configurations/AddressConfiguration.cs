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
    public class AddressConfiguration : BaseConfiguration<Address>, IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.Property(a => a.AddressLine1).HasMaxLength(256);
            builder.Property(a => a.AddressLine2).HasMaxLength(256);
            builder.Property(a => a.AddressLine3).HasMaxLength(256);
            builder.Property(a => a.City).HasMaxLength(256);
            builder.Property(a => a.PostalCode).HasMaxLength(256);
            builder.Property(a => a.Country).HasMaxLength(50);
            builder.Property(a => a.CountryCode).HasMaxLength(3);
            base.Configure(builder);
        }
    }
}
