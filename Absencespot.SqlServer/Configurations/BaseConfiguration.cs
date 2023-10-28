using Absencespot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.SqlServer.Configurations
{
    public class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasAlternateKey(x => x.GlobalId);
            builder.Property(x => x.GlobalId).HasDefaultValueSql("NEWID()");
            builder.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
