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

        public async Task DeleteAsync(Guid companyId, Guid teamId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (teamId == default)
            {
                throw new ArgumentNullException(nameof(teamId));
            }

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var teamDomain = await _unitOfWork.TeamRepository.FindByGlobalIdAsync(teamId);
            if (teamDomain == null)
            {
                throw new NotFoundException(nameof(teamDomain));
            }

            if (teamDomain.CompanyId != companyDomain.Id)
            {
                throw new UnauthorizedAccessException();
            }

            foreach (var teamUser in teamDomain.Users)
            {
                teamDomain.Users.Remove(teamUser);
            }

            _unitOfWork.TeamRepository.Remove(teamDomain);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<Pagination<Team>> GetAllAsync(Guid companyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var queryable = _unitOfWork.TeamRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(l => l.Company.GlobalId == companyId);

            var totalTeams = queryable.Count();
            queryable = queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            queryable = _unitOfWork.TeamRepository.Include(queryable, x => x.Users);
            queryable = _unitOfWork.TeamRepository.IncludeThen<Domain.UserTeam, Domain.User>(queryable, x => x.User);
            var teams = await _unitOfWork.TeamRepository.ToListAsync(queryable, cancellationToken);

            _logger.LogInformation($"Get office pageSize: {pageSize}, pageNumber: {pageNumber}");

            return new Pagination<Dtos.Team>()
            {
                TotalRecords = totalTeams,
                TotalPages = (int)Math.Ceiling((decimal)totalTeams / (decimal)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = teams.Select(TeamMapper.ToDto)
            };
        }

        public async Task<Dtos.Team> GetByIdAsync(Guid companyId, Guid teamId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (teamId == default)
            {
                throw new ArgumentNullException(nameof(teamId));
            }

            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }

            var teamDomain = await _unitOfWork.TeamRepository.FindByGlobalIdAsync(teamId);
            if (teamDomain == null)
            {
                throw new NotFoundException(nameof(teamDomain));
            }

            _logger.LogInformation($"Found team Id: {teamId}");

            return TeamMapper.ToDto(teamDomain);
        }

        public async Task<Team> UpdateAsync(Guid companyId, Guid teamId, Team team, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var companyDomain = await _unitOfWork.CompanyRepository.FindByGlobalIdAsync(companyId, cancellationToken: cancellationToken);
            if (companyDomain == null)
            {
                throw new NotFoundException(nameof(companyDomain));
            }
            if (teamId == default)
            {
                throw new ArgumentNullException(nameof(teamId));
            }
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            team.EnsureValidation();

            var queryable = _unitOfWork.TeamRepository.AsQueryable(RepositoryOptions.AsNoTracking());
            queryable = queryable.Where(l => l.Company.GlobalId == companyId);
            queryable = _unitOfWork.TeamRepository.Include(queryable, x => x.Users);
            queryable = _unitOfWork.TeamRepository.IncludeThen<Domain.UserTeam, Domain.User>(queryable, x => x.User);
            var teamDomain = await _unitOfWork.TeamRepository.FirstOrDefaultAsync(queryable, cancellationToken);

            if (teamDomain == null)
            {
                throw new ArgumentNullException(nameof(teamDomain));
            }

            foreach (Domain.UserTeam userTeam in teamDomain?.Users)
            {
                if (!team.Users.Any(baseUser => baseUser.Id == userTeam.User?.GlobalId))
                {
                    teamDomain.Users.Remove(userTeam);
                }
            }

            foreach (BaseUser baseUser in team.Users)
            {
                if (!teamDomain.Users.Any(teamUser => teamUser.User.GlobalId == baseUser.Id))
                {
                    var user = await LoadUserByIdAsync(companyId, baseUser.Id);
                    if (user == null)
                    {
                        throw new NotFoundException(nameof(user));
                    }
                    teamDomain.Users ??= new List<Domain.UserTeam>();
                    teamDomain.Users.Add(new Domain.UserTeam
                    {
                        Team = teamDomain,
                        User = user
                    });
                }
            }

            teamDomain.Name = team.Name;
            teamDomain.Name = team.Description;
            teamDomain.IsAutoApproved = team.IsAutoApproved;

            _unitOfWork.TeamRepository.Update(teamDomain);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Updated leave Id: {teamId}");

            return TeamMapper.ToDto(teamDomain);
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
