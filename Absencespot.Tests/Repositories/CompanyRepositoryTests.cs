using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Absencespot.Tests.Repositories
{
    public class CompanyRepositoryTests : BaseTest
    {
        private IUnitOfWork _unitOfWork;
        public CompanyRepositoryTests()
        {
            _unitOfWork = this.ServiceProvider.GetRequiredService<IUnitOfWork>();
        }

        [Test]
        public async Task CreateCompany_Successfull()
        {
            var company = new Company()
            {
                Name = "Company X",
                FiscalNumber = "286828666"
            };

            var result = _unitOfWork.CompanyRepository.Add(company);
            await _unitOfWork.SaveChangesAsync();

            Assert.IsNotNull(result);
        }
    }
}
