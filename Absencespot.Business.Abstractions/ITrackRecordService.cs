using Absencespot.Utils;

namespace Absencespot.Business.Abstractions
{
    public interface ITrackRecordService
    {
        Task<Dtos.TrackRecordMetrics> GetMetricsByUserIdAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default);
        Task<Dtos.TrackRecord> GetByIdAsync(Guid companyId, Guid trackRecordId, CancellationToken cancellationToken = default);
        Task<Pagination<Dtos.TrackRecord>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<Dtos.TrackRecord> CreateAsync(Guid companyId, Dtos.TrackRecord trackRecord, CancellationToken cancellationToken = default);
        Task<Dtos.TrackRecord> UpdateAsync(Guid companyId, Guid trackRecordId, Dtos.TrackRecord trackRecord, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid companyId, Guid trackRecordId, CancellationToken cancellationToken = default);
    }
}
