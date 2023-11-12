using Absencespot.Business.Abstractions;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Services.Exceptions;
using Absencespot.Services.Mappers;
using Absencespot.Utils;
using Absencespot.Utils.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Absencespot.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Domain.User> _userManager;
        public UserService(ILogger<UserService> logger, UserManager<Domain.User> userManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Pagination<Dtos.User>> GetAllAsync(
            Guid companyId,
            string? groupBy,
            Guid? filterBy,
            string? textSearch,  
            int pageNumber = 1,
            int pageSize = 50,
            string? sortBy = GroupBy.Office,
            bool? sortAsc = false,
            CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (pageNumber < 1)
            {
                throw new ArgumentException(nameof(pageNumber));
            }
            if (pageSize < 1 || pageSize > 200)
            {
                throw new ArgumentException(nameof(pageSize));
            }

            var queryable = _userManager.Users.AsQueryable();
            queryable = queryable.Where(u => u.Company.GlobalId == companyId);

            if (!string.IsNullOrWhiteSpace(groupBy) && groupBy == GroupBy.Office)
            {
                if (filterBy != default)
                {
                    queryable = queryable.Where(u => u.Office.GlobalId == filterBy);
                }
            }
            else if (!string.IsNullOrWhiteSpace(groupBy) && groupBy == GroupBy.Team)
            {
                if (filterBy != default)
                {
                    queryable = queryable.Where(u => u.Teams.Any(x => x.Team.GlobalId == filterBy));
                }
            }

            if (!string.IsNullOrWhiteSpace(textSearch))
            {
                queryable = queryable.Where(u => u.UserName.ToLower() == textSearch.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(sortBy) && sortBy.ToLower() == "name")
            {
                queryable = sortAsc == true
                ? queryable.OrderBy(u => u.UserName)
                : queryable.OrderByDescending(u => u.UserName);
            }

            var totalUser = queryable.Count();
            queryable = queryable.Skip(pageNumber * pageSize).Take(pageSize);
            var users = queryable.ToList();

            return new Pagination<Dtos.User>()
            {
                TotalRecords = totalUser,
                TotalPages = (int)Math.Ceiling((decimal)totalUser / (decimal)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = users.Select(UserMapper.ToDto)
            };


            throw new NotImplementedException();
        }


        public async Task<Dtos.User> GetByIdAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var user = await LoadByIdAsync(companyId, userId);
            var userDto = UserMapper.ToDto(user);

            var profileUrl = await GetProfileImageAsync(userId);
            if (!string.IsNullOrWhiteSpace(profileUrl))
            {
                userDto.ProfileUrl = profileUrl;
            }

            return userDto;
        }

        public async Task<string> GetProfileImageAsync(Guid userId)
        {
            //if (userId == default)
            //{
            //    throw new ArgumentNullException(nameof(userId));
            //}
            //var fileLink = await _blobClient.GetByUserIdAsync(_containerName, userId.ToString());

            //if (string.IsNullOrWhiteSpace(fileLink))
            //{
            //    throw new NotFoundException(nameof(fileLink));
            //}

            return "";
        }

        public async Task<string> InviteAsync(Guid companyId, Dtos.InviteUser user, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }
            user.EnsureValidation();

            var userDomain = await _userManager.FindByEmailAsync(user.Email);
            if(userDomain != null)
            {
                throw new Exception($"User {user.Email} already exists.");
            }
            
            userDomain = new Domain.User()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDay = user?.BirthDay,
            };

            var workschedule = await _unitOfWork.WorkScheduleRepository.FindByGlobalIdAsync(user.WorkScheduleId);
            var office = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(user.OfficeId);
            var company = await _unitOfWork.OfficeRepository.FindByGlobalIdAsync(companyId);

            var userTeams = new List<Domain.UserTeam>();
            foreach (var Id in user.TeamIds)
            {
                var team = await _unitOfWork.TeamRepository.FindByGlobalIdAsync(Id);
                userTeams.Add(new Domain.UserTeam() { User = userDomain, Team = team });
            }
            userDomain.Teams = userTeams;

            var result = await _userManager.CreateAsync(userDomain);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.ToList().ToString());
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userDomain);
            var tokenString = Conversor.ToBase64(token);

            var uriBuilder = new UriBuilder("http://localhost:3000/auth/confirm-email");
            uriBuilder.Query = $"token={tokenString}&email={user.Email}";
            var confirmationLink = uriBuilder.Uri.ToString();

            var subject = "Confirm your email address";
            var htmlContent = $"<p>This is your confirmation link: <a href={confirmationLink} >Click here to confirm</a></p>";

           // await _sendgridClient.SendEmailAsync(user.Email, subject, htmlContent);

            _logger.LogInformation($"{nameof(InviteAsync)} send email: {subject}");

            return htmlContent;
        }

        public async Task<Dtos.User> UpdateByIdAsync(Guid companyId, Guid userId, Dtos.User user, CancellationToken cancellationToken = default)
        {
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.EnsureValidation();

            var userDomain = await LoadByIdAsync(companyId, userId);
            if (userDomain == null)
            {
                throw new NotFoundException(nameof(userDomain));
            }

            userDomain.FirstName = user.FirstName;
            userDomain.LastName = user.LastName;
            userDomain.BirthDay = user.BirthDay;
            userDomain.Position = user.Position;
            userDomain.StartDate = user.StartDate;

            var updatedUserResponse = await _userManager.UpdateAsync(userDomain);
            if (!updatedUserResponse.Succeeded)
            {
                throw new InvalidOperationException($"Could not pdate user {userDomain.Id}");
            }

            var userDto = UserMapper.ToDto(userDomain);

            var profileUrl = await GetProfileImageAsync(userId);
            if (!string.IsNullOrWhiteSpace(profileUrl))
            {
                userDto.ProfileUrl = profileUrl;
            }

            return userDto;
        }

        public async Task UploadProfileImageAsync(Guid companyId, Guid userId, FileStream file, CancellationToken cancellationToken = default)
        {
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            //var isUploaded = await _blobClient.UploadAsync(_containerName, userId.ToString(), file);
            //if (!isUploaded)
            //{
            //    throw new InvalidOperationException("Could not upload file");
            //}

            _logger.LogInformation($"{nameof(UploadProfileImageAsync)} userId: {userId}");
        }

        public async Task ChangePasswordAsync(Guid companyId, Guid userId, Dtos.ChangePasswordRequest password, CancellationToken cancellationToken = default)
        {
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            password.EnsureValidation();

            var user = await LoadByIdAsync(companyId, userId);
            if (user == null)
            {
                throw new NotFoundException(nameof(user));
            }

            var result = await _userManager.ChangePasswordAsync(user, password.CurrentPassword, password.NewPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException();
            }

            _logger.LogInformation($"{nameof(ChangePasswordAsync)} userId: {userId}");
        }

        private async Task<Domain.User> LoadByIdAsync(Guid companyId, Guid userId)
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
