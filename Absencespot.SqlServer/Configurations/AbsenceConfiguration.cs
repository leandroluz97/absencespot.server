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
    internal class AbsenceConfiguration : BaseConfiguration<Absence>, IEntityTypeConfiguration<Absence>
    {
        public void Configure(EntityTypeBuilder<Absence> builder)
        {
            builder.ToTable("Absence");
            base.Configure(builder);
        }
    }
}
