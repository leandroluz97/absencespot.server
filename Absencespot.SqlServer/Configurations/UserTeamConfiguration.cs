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
    public class UserTeamConfiguration : BaseConfiguration<UserTeam>, IEntityTypeConfiguration<UserTeam>
    {
        public UserTeamConfiguration(){}
        public void Configure(EntityTypeBuilder<UserTeam> builder)
        {
            builder.ToTable("UserTeam");
            builder.HasAlternateKey(x => x.GlobalId);
            builder.Property(x => x.GlobalId).HasDefaultValueSql("NEWID()");
            builder.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasKey(x => new { x.UserId, x.TeamId });

            //builder.HasOne(ut => ut.User)
            //.WithMany(i => i.Teams)
            //.HasForeignKey(o => o.TeamId)
            //.OnDelete(DeleteBehavior.Restrict);
        }
    }
}
