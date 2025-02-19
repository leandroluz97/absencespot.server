﻿using Absencespot.Infrastructure.Abstractions.Repositories;

namespace Absencespot.Infrastructure.Abstractions
{
    public interface IUnitOfWork
    {
        ICompanyRepository CompanyRepository { get; }
        IAvailableLeaveRepository AvailableLeaveRepository { get; }
        IUserTeamRepository UserRoleRepository { get; }
        IOfficeLeaveRepository OfficeLeaveRepository { get; }
        IIntegrationRepository IntegrationRepository { get; }
        ILeaveRepository LeaveRepository { get; }
        IOfficeRepository OfficeRepository { get; }
        IRequestRepository RequestRepository { get; }
        ISettingsRepository SettingsRepository { get; }
        ITeamRepository TeamRepository { get; }
        ITrackRecordRepository TrackRecordRepository { get; }
        IWorkScheduleRepository WorkScheduleRepository { get; }
        ISubscriptionRepository SubscriptionRepository { get; }

        void SaveChanges();
        void Rollback();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync();
    }
}