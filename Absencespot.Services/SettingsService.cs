using Absencespot.Business.Abstractions;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ILogger<SettingsService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Domain.User> _userManager;
        public SettingsService(ILogger<SettingsService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
       
        public async Task<Dtos.Settings> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            Guid userId = new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2");
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var companyDomain = await LoadCompanyByIdAsync(companyId, RepositoryOptions.AsTracking(), cancellationToken);
            var userDomain = await LoadUserByIdAsync(companyId, userId);

            var settingsDomain = await _unitOfWork.SettingsRepository.FindByCompanyIdAsync(companyDomain.Id, cancellationToken: cancellationToken);

            if (userDomain.CompanyId != companyDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            if(settingsDomain == null)
            {
                settingsDomain = await CreateAsync(companyDomain);
            }

            if (settingsDomain.CompanyId != companyDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            _logger.LogInformation($"Found settings company Id: {companyDomain.GlobalId}");
            return SettingsMapper.ToDto(settingsDomain);
        }


        public async Task<Dtos.Settings> UpdateAsync(Guid companyId, Dtos.Settings settings, CancellationToken cancellationToken = default)
        {
            Guid userId = new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2");
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            settings.EnsureValidation();

            var companyDomain = await LoadCompanyByIdAsync(companyId, RepositoryOptions.AsTracking(), cancellationToken);
            var userDomain = await LoadUserByIdAsync(companyId, userId);

            var options = RepositoryOptions.AsTracking();
            var settingsDomain = await _unitOfWork.SettingsRepository.FindByCompanyIdAsync(companyDomain.Id, options, cancellationToken);

            if(settingsDomain == null)
            {
                settingsDomain = await CreateAsync(companyDomain);
            }

            settingsDomain.OnBehalfOf = settings.OnBehalfOf;
            settingsDomain.DisplayBirthday = settings.DisplayBirthday;
            settingsDomain.DisplayMode = settings.DisplayMode;
            settingsDomain.DateFormat = settings.DateFormat;
            settingsDomain.FirstDayOfTheWeek = settings.FirstDayOfTheWeek;
            settingsDomain.IsLoginFromEmailLinkRequired = settings.IsLoginFromEmailLinkRequired;
            settingsDomain.PrivacyMode = settings.PrivacyMode;
            settingsDomain.TimeFormat = settings.TimeFormat;
            settingsDomain.TimeZone = settings.TimeZone;

            settingsDomain = _unitOfWork.SettingsRepository.Update(settingsDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Updated settings Id: {companyId}");
            return SettingsMapper.ToDto(settingsDomain);
        }

        public async Task<Domain.Settings> CreateAsync(Domain.Company company, CancellationToken cancellationToken = default)
        {
            var settings = new Dtos.Settings()
            {
                OnBehalfOf = true,
                DateFormat = "22-90-2024",
                DisplayBirthday = true,
                DisplayMode = Domain.Enums.DisplayType.Office,
                FirstDayOfTheWeek = "Mon",
                IsLoginFromEmailLinkRequired = true,
                PrivacyMode = true,
                TimeFormat = "dd/mm/yyyy",
                TimeZone = "Lisbon"
            };
            var settingsDomain = SettingsMapper.ToDomain(settings);
            settingsDomain.Company = company;

            settingsDomain = _unitOfWork.SettingsRepository.Add(settingsDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return settingsDomain;
        }

        private async Task<Domain.User> LoadUserByIdAsync(Guid companyId, Guid userId)
        {
            var queryable = _userManager.Users.AsQueryable();
            queryable = queryable.Where(u => u.Company.GlobalId == companyId && u.GlobalId == userId);
            var user = queryable.ToList().FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException(nameof(userId));
            }

            return user;
        }

        private async Task<Domain.Company> LoadCompanyByIdAsync(Guid companyId, RepositoryOptions options = null, CancellationToken cancellationToken = default)
        {
            Domain.Company? companyDomain;
            if (options == null)
            {
                companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            }
            else
            {
                companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, options, cancellationToken: cancellationToken);
            }

            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            return companyDomain;
        }


    }
}
