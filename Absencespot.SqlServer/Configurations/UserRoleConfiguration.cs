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
    public class UserRoleConfiguration : BaseConfiguration<UserRole>, IEntityTypeConfiguration<UserRole>
    {
        public UserRoleConfiguration(){}
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRole");
            base.Configure(builder);
            builder.HasKey(x => new { x.UserId, x.RoleId });
        }
    }
}
