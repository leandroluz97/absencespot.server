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
    public class TeamService : ITeamService
    {
        private readonly ILogger<TeamService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Domain.User> _userManager;

        public TeamService(ILogger<TeamService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<Team> CreateAsync(Guid companyId, Dtos.Team team, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            team.EnsureValidation();

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, RepositoryOptions.AsTracking());
            if (companyDomain == null)
            {
                throw new NotFoundException($"Not found {companyId}");
            }

            var teamDomain = TeamMapper.ToDomain(team);
            teamDomain.Company = companyDomain;
            List<Domain.UserTeam> usersTeam = new List<Domain.UserTeam>();

            foreach (Dtos.BaseUser user in team.Users)
            {
                var userDomain = await LoadUserByIdAsync(companyId, user.Id);
                if (userDomain == null)
                {
                    throw new NotFoundException(nameof(userDomain));
                }
                usersTeam.Add(new Domain.UserTeam()
                {
                    User = userDomain,
                    Team = teamDomain
                });
            }

            teamDomain.Users = usersTeam;
            teamDomain = _unitOfWork.TeamRepository.Add(teamDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Created team Id {teamDomain.Id}");
            return TeamMapper.ToDto(teamDomain);
        }

        public Task DeleteAsync(Guid companyId, Guid officeId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<Team>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Team> GetByIdAsync(Guid companyId, Guid teamId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Team> UpdateAsync(Guid companyId, Guid officeId, Team office, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
    }
}
