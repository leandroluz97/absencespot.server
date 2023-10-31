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
    public class TeamConfiguration : BaseConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Team");
            builder.Property(l => l.Name).HasMaxLength(256).IsRequired();
            builder.Ignore(l => l.Managers);

            builder.HasMany<UserTeam>(u => u.Users)
                    .WithOne(u => u.Team)
                    .HasForeignKey(o => o.TeamId);

            //builder.HasMany<UserTeam>(u => u.Managers)
            //       .WithOne(u => u.Team)
            //       .HasForeignKey(u => u.TeamId);

            base.Configure(builder);
        }
    }
}
