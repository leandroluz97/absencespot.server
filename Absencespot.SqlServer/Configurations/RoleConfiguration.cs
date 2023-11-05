using Absencespot.Domain;
using Absencespot.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Absencespot.SqlServer.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public RoleConfiguration(){}
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("AspNetRoles");
            builder.HasKey(x => x.Id);
            builder.HasAlternateKey(x => x.GlobalId);
            builder.Property(x => x.GlobalId).HasDefaultValueSql("NEWID()");
            builder.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.Name).HasMaxLength(256);
            builder.Property(u => u.NormalizedName).HasMaxLength(256);

            builder.HasMany<IdentityUserRole<int>>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
            builder.HasMany<IdentityRoleClaim<int>>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            builder.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
        }
    }
}
