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
    public class IntegrationConfiguration : BaseConfiguration<Integration>, IEntityTypeConfiguration<Integration>
    {
        public IntegrationConfiguration(){}
        public void Configure(EntityTypeBuilder<Integration> builder)
        {
            builder.ToTable("Integration");
            builder.Property(o => o.Provider);
                   //.HasConversion(
                   //         type => type.ToString(),
                   //         displayName => ProviderType.FromDisplayName<ProviderType>(displayName)
                   // );
           
            base.Configure(builder);
        }
    }
}
