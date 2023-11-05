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
        public RequestConfiguration(){}
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.ToTable("Request");
            builder.Property(l => l.StartDate).IsRequired();
            builder.Property(l => l.EndDate).IsRequired();
            builder.Property(o => o.Status);
                   //.HasConversion(
                   //        type => type.ToString(),
                   //        name => Enum.TryParse<DisplayType>(name, out DisplayType displayType));

            //builder.HasOne(c => c.OnBehalfOf)
            //        .WithMany(i => i.OnBehalfOs)
            //        .HasForeignKey(o => o.OnBehalfOfId)
            //        .IsRequired(false);

            builder.HasOne(c => c.User)
                    .WithMany(i => i.Requests)
                    .HasForeignKey(o => o.UserId);


            base.Configure(builder);
        }
    }
}