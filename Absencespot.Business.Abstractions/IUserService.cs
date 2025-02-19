﻿using Absencespot;
using Absencespot.Utils;

namespace Absencespot.Business.Abstractions
{
    public interface IUserService
    {
        Task<Pagination<Dtos.User>> GetAllAsync(Guid companyId, string? groupBy, Guid? filterBy, string? textSearch, int pageNumber, int pageSize, string? sortBy, bool? sortAsc, CancellationToken cancellationToken = default);
        Task<Dtos.User> GetByIdAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default);
        Task<Dtos.User> UpdateByIdAsync(Guid companyId, Guid userId, Dtos.User user, CancellationToken cancellationToken = default);
        Task<string> InviteAsync(Guid companyId, Dtos.InviteUser user, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(Guid companyId, Guid userId, Dtos.ChangePasswordRequest password, CancellationToken cancellationToken = default);
        Task UploadProfileImageAsync(Guid companyId, Guid userId, FileStream file, CancellationToken cancellationToken = default);
        Task<string> GetProfileImageAsync(Guid userId);
    }
}
