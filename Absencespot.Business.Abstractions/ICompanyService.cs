namespace Absencespot.Business.Abstractions
{
    public interface ICompanyService
    {
        Task<Dtos.Company> CreateAsync(Dtos.Company company, CancellationToken cancellationToken = default);
        Task<Dtos.Company> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<Dtos.Company> UpdateAsync(Guid companyId, Dtos.Company company, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid companyId, CancellationToken cancellationToken = default);
    }
}
