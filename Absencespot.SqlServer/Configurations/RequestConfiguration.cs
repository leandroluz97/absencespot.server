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
    public class RequestConfiguration : BaseConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.ToTable("Request");
            builder.Property(o => o.Status)
                  .HasConversion(
                           type => type.DisplayName,
                           displayName => StatusType.FromDisplayName<StatusType>(displayName)
                   );

            base.Configure(builder);
        }
    }
}
//builder.HasMany<Request>(c => c.Requests)
//        .WithOne(i => i.Company)
//        .HasForeignKey(o => o.CompanyId);