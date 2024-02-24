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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary key
            builder.HasKey(u => u.Id);

            //Custom properties
            builder.HasAlternateKey(x => x.GlobalId);
            builder.Property(x => x.GlobalId).HasDefaultValueSql("NEWID()");
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Indexes for "normalized" username and email, to allow efficient lookups
            builder.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
            builder.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");

            // Maps to the AspNetUsers table
            builder.ToTable("AspNetUsers");

            // A concurrency token for use with the optimistic concurrency checking
            builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            // Limit the size of columns to use efficient database types
            builder.Property(u => u.UserName).HasMaxLength(256);
            builder.Property(u => u.CalendarId);
            builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
            builder.Property(u => u.Email).HasMaxLength(256);
            builder.Property(u => u.NormalizedEmail).HasMaxLength(256);


            builder.HasMany<UserTeam>(u => u.Teams)
                 .WithOne(u => u.User)
                 .HasForeignKey(o => o.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Requests)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust the delete behavior as needed

            builder.HasMany(u => u.Approved)
                .WithOne(r => r.Approver)
                .HasForeignKey(r => r.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.OnBehalfOfs)
                .WithOne(r => r.OnBehalfOf)
                .HasForeignKey(r => r.OnBehalfOfId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.AvailableLeaves)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // The relationships between User and other entity types
            // Note that these relationships are configured with no navigation properties

            //// Each User can have many UserClaims
            //builder.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

            //// Each User can have many UserLogins
            //builder.HasMany<UserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

            //// Each User can have many UserTokens
            //builder.HasMany<TUserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

            //// Each User can have many entries in the UserRole join table
            //builder.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        }
    }
}
