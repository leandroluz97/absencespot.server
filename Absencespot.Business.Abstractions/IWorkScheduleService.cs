using Absencespot.Utils;

namespace Absencespot.Business.Abstractions
{
    public interface IWorkScheduleService
    {
        Task<Dtos.WorkSchedule> GetByIdAsync(Guid companyId, Guid workScheduleId, CancellationToken cancellationToken = default);
        Task<Pagination<Dtos.WorkSchedule>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<Dtos.WorkSchedule> CreateAsync(Guid companyId, Dtos.WorkSchedule workSchedule, CancellationToken cancellationToken = default);
        Task<Dtos.WorkSchedule> UpdateAsync(Guid companyId, Guid workScheduleId, Dtos.WorkSchedule workSchedule, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid companyId, Guid workScheduleId, CancellationToken cancellationToken = default);
    }
}
