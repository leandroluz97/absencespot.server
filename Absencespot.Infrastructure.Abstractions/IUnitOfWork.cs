using Absencespot.Infrastructure.Abstractions.Repositories;

namespace Absencespot.Infrastructure.Abstractions
{
    public interface IUnitOfWork
    {
        ICompanyRepository CompanyRepository { get; }
        IUserTeamRepository UserRoleRepository { get; }
        IOfficeLeaveRepository OfficeLeaveRepository { get; }
        IIntegrationRepository IntegrationRepository { get; }
        ILeaveRepository LeaveRepository { get; }
        IOfficeRepository OfficeRepository { get; }
        IRequestRepository RequestRepository { get; }
        ISettingsRepository SettingsRepository { get; }
        ITeamRepository ITeamRepository { get; }
        ITrackRecordRepository TrackRecordRepository { get; }
        IWorkScheduleRepository WorkScheduleRepository { get; }

        void SaveChanges();
        void Rollback();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync();
    }
}