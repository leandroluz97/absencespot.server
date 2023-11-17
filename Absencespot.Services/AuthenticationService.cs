using Absencespot.Business.Abstractions;
using Absencespot.Clients.Sendgrid;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions.Clients;
using Absencespot.Services.Exceptions;
using Absencespot.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Domain.User> _userManager;
        private readonly SignInManager<Domain.User> _signInManager;
        private readonly IEmailClient _sendgridClient;

        public AuthenticationService(
            ILogger<AuthenticationService> logger, 
            IConfiguration configuration,
            UserManager<Domain.User> userManager,
            SignInManager<Domain.User> signInManager,
            IEmailClient sendgridClient)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _sendgridClient = sendgridClient;
        }

        public async Task Register(Dtos.Register register)
        {
            _logger.LogInformation($"{nameof(Register)} param value {register}");

            if (register == null)
            {
                throw new ArgumentNullException(nameof(register));
            }
            register.EnsureValidation();

            var user = new Domain.User()
            {
                Email = register.Email,
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.ToList().ToString());
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenString = Conversor.ToBase64(token);

            var uriBuilder = new UriBuilder(_configuration["FrontEnd:ConfirmEmail"]!);
            uriBuilder.Query = $"token={tokenString}&email={user.Email}";
            var confirmationLink = uriBuilder.Uri.ToString();

            IEmailOptions emailOptions = new SendgridOptions()
            {
                SenderEmail = "support@absencespot.com",
                ReceiverEmail = user.Email,
                TemplateId = "",
                TemplateData = new Dictionary<string, string>
                {
                    { "link", confirmationLink }
                }
            };
            await _sendgridClient.SendEmailAsync(emailOptions);

            _logger.LogInformation($"{nameof(Register)} send email: {user.Email}");
        }


        public async Task ConfirmEmail(Dtos.ConfirmEmail confirmEmail)
        {
            _logger.LogInformation($"{nameof(ConfirmEmail)} param value {confirmEmail}");

            if (confirmEmail == null)
            {
                throw new ArgumentNullException($"Information to confirm email is required.", nameof(confirmEmail));
            }
            confirmEmail.EnsureValidation();

            var user = await _userManager.FindByEmailAsync(confirmEmail.Email);
            if (user == null)
            {
                throw new NotFoundException($"{nameof(user)} with {confirmEmail.Email} not found");
            }
            user.FirstName = confirmEmail.FirstName;
            user.LastName = confirmEmail.LastName;
            user.PhoneNumber = confirmEmail.PhoneNumber;

            await _userManager.ChangePasswordAsync(user, null, user.Email);
            var token = Conversor.ToString(confirmEmail.Token);

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException();
            }

            _logger.LogInformation($"{nameof(ConfirmEmail)} {user.Email}");
        }

        public Task<TokenResponse> ExternalLogin(ExternalLogin externalLogin)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> Login(LoginRequest login)
        {
            throw new NotImplementedException();
        }

        public Task LoginWithGoogle()
        {
            throw new NotImplementedException();
        }

        public Task LoginWithMicrosoft()
        {
            throw new NotImplementedException();
        }

        public Task LoginWithSlack()
        {
            throw new NotImplementedException();
        }


        public Task RequestResetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task ResetPassword(ResetPassword resetPassword)
        {
            throw new NotImplementedException();
        }
    }
}
