using Absencespot.Infrastructure.Abstractions;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Absencespot.SqlServer;
using Absencespot.UnitOfWork.Repositories;
using Absencespot.Domain.Seedwork;

namespace Absencespot.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private ICompanyRepository _companyRepository;
        private IUserTeamRepository _userRoleRepository;
        private IOfficeLeaveRepository _officeLeaveRepository;
        private IIntegrationRepository _integrationRepository;
        private ILeaveRepository _leaveRepository;
        private IOfficeRepository _officeRepository;
        private IRequestRepository _requestRepository;
        private ISettingsRepository _settingsRepository;
        private ITeamRepository _teamRepository;
        private ITrackRecordRepository _trackRecordRepository;
        private IWorkScheduleRepository _workScheduleRepository;
        
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IOfficeLeaveRepository OfficeLeaveRepository
        {
            get { return _officeLeaveRepository = _officeLeaveRepository ?? new OfficeLeaveRepository(_dbContext); }
        }
        public IUserTeamRepository UserRoleRepository
        {
            get { return _userRoleRepository = _userRoleRepository ?? new UserTeamRepository(_dbContext); }
        }
        public ICompanyRepository CompanyRepository
        {
            get { return _companyRepository = _companyRepository ?? new CompanyRepository(_dbContext); }
        }

        public IIntegrationRepository IntegrationRepository
        {
            get { return _integrationRepository = _integrationRepository ?? new IntegrationRepository(_dbContext); }
        }
        public ILeaveRepository LeaveRepository
        {
            get { return _leaveRepository = _leaveRepository ?? new LeaveRepository(_dbContext); }
        }
        public IOfficeRepository OfficeRepository
        {
            get { return _officeRepository = _officeRepository ?? new OfficeRepository(_dbContext); }
        }
        public IRequestRepository RequestRepository
        {
            get { return _requestRepository = _requestRepository ?? new RequestRepository(_dbContext); }
        }
        public ISettingsRepository SettingsRepository
        {
            get { return _settingsRepository = _settingsRepository ?? new SettingsRepository(_dbContext); }
        }
        public ITeamRepository ITeamRepository
        {
            get { return _teamRepository = _teamRepository ?? new TeamRepository(_dbContext); }
        }
        public ITrackRecordRepository TrackRecordRepository
        {
            get { return _trackRecordRepository = _trackRecordRepository ?? new TrackRecordRepository(_dbContext); }
        }
        public IWorkScheduleRepository WorkScheduleRepository
        {
            get { return _workScheduleRepository = _workScheduleRepository ?? new WorkScheduleRepository(_dbContext); }
        }

        public void Rollback()
        {
            _dbContext.Dispose();
        }

        public async Task RollbackAsync()
        {
            await _dbContext.DisposeAsync();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries<Signature>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
        
    
}