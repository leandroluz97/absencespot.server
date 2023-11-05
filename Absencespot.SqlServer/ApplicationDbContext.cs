

using Absencespot.Domain;
using Absencespot.Domain.Enums;
using Absencespot.SqlServer.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Absencespot.SqlServer
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Ignore<DisplayType>();
            //modelBuilder.Ignore<LocationType>();
            //modelBuilder.Ignore<ProviderType>();
            //modelBuilder.Ignore<RoleType>();
            //modelBuilder.Ignore<StatusType>();
            //modelBuilder.Ignore<SubscriptionType>();
            modelBuilder.ApplyConfiguration(new RequestConfiguration());
            modelBuilder.ApplyConfiguration(new AbsenceConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationConfiguration());
            modelBuilder.ApplyConfiguration(new LeaveConfiguration());
            modelBuilder.ApplyConfiguration(new OfficeConfiguration());
            modelBuilder.ApplyConfiguration(new OfficeLeaveConfiguration());
            modelBuilder.ApplyConfiguration(new SettingsConfiguration());
            modelBuilder.ApplyConfiguration(new TeamConfiguration());
            modelBuilder.ApplyConfiguration(new TrackRecordConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserTeamConfiguration());
            modelBuilder.ApplyConfiguration(new WorkScheduleConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}