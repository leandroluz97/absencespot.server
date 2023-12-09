using Absencespot.Utils;

namespace Absencespot.Business.Abstractions
{
    public interface IRequestService
    {
        Task<Dtos.Request> GetByIdAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default);
        Task<Pagination<Dtos.Request>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<Dtos.Request> CreateAsync(Guid companyId, Dtos.Request request, CancellationToken cancellationToken = default);
        Task<Dtos.Request> UpdateAsync(Guid companyId, Guid requestId, Dtos.Request request, CancellationToken cancellationToken = default);
        Task<Dtos.Request> ApproveAsync(Guid companyId, Guid requestId, Dtos.ApproveRequest request, CancellationToken cancellationToken = default);
        Task<Dtos.Request> RejectAsync(Guid companyId, Guid requestId, Dtos.RejectRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid companyId, Guid requestId, CancellationToken cancellationToken = default);
    }
}