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
    public class OfficeLeaveConfiguration : BaseConfiguration<OfficeLeave>, IEntityTypeConfiguration<OfficeLeave>
    {
        public void Configure(EntityTypeBuilder<OfficeLeave> builder)
        {
            builder.ToTable("OfficeLeave");
            base.Configure(builder);
           // builder.HasKey(x =>  new { x.OfficeId, x.LeaveId });
        }
    }
}
