using Absencespot.Utils;

namespace Absencespot.Business.Abstractions
{
    public interface ILeaveService
    {
        Task<Pagination<Dtos.Leave>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<Dtos.Leave> CreateAsync(Guid companyId, Dtos.Leave leave, CancellationToken cancellationToken = default);
        Task<Dtos.Leave> UpdateAsync(Guid companyId,Guid leaveId, Dtos.Leave leave, CancellationToken cancellationToken = default);
        Task<Dtos.Leave> GetByIdAsync(Guid companyId, Guid leaveId, CancellationToken cancellationToken = default);
    }
}
