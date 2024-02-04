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
    public class CompanyConfiguration : BaseConfiguration<Company>, IEntityTypeConfiguration<Company>
    {
        public CompanyConfiguration(){}
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company");
            builder.Property(c => c.Name).HasMaxLength(256).IsRequired();
            builder.Property(c => c.FiscalNumber);

            builder.HasMany<Integration>(c => c.Integrations)
                    .WithOne(i => i.Company)
                    .HasForeignKey(o => o.CompanyId);

            builder.HasMany<User>(c => c.Users)
                    .WithOne(i => i.Company)
                    .HasForeignKey(o => o.CompanyId);

            builder.HasMany<Leave>(c => c.Leaves)
                    .WithOne(i => i.Company)
                    .HasForeignKey(o => o.CompanyId);

            builder.HasMany<Office>(c => c.Offices)
                    .WithOne(i => i.Company)
                    .HasForeignKey(o => o.CompanyId)
                     .OnDelete(DeleteBehavior.ClientCascade);

            builder.HasMany<Team>(c => c.Teams)
                    .WithOne(i => i.Company)
                    .HasForeignKey(o => o.CompanyId);

            builder.HasMany<WorkSchedule>(c => c.WorkSchedules)
                    .WithOne(i => i.Company)
                    .HasForeignKey(o => o.CompanyId);

            builder.HasOne(c => c.Settings)
                    .WithOne(i => i.Company)
                    .HasForeignKey<Settings>(o => o.CompanyId);

            builder.HasOne(c => c.Subcription)
                    .WithOne(i => i.Company)
                    .HasForeignKey<Subscription>(o => o.CompanyId);

            base.Configure(builder);
        }
    }
}
