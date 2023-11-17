using Absencespot.Business.Abstractions;
using Absencespot.Clients.Sendgrid;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions.Clients;
using Absencespot.Services.Exceptions;
using Absencespot.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        public async Task<Dtos.TokenResponse> Login(LoginRequest login)
        {
            _logger.LogInformation($"{nameof(Login)} param value {login}");

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }
            login.EnsureValidation();

            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                throw new NotFoundException($"{nameof(user)} with {login.Email} not found");
            }

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unable to authenticate user {nameof(user.Email)}.");
            }
            
            return CreateJWT(user);
        }

        public async Task<TokenResponse> ExternalLogin(ExternalLoginInfo externalLogin)
        {
            _logger.LogInformation("{ExternalLogin} param value {externalLoginInfo}", nameof(ExternalLogin), externalLogin);

            if (externalLogin == null)
            {
                return null;
            }

            var signinResult = await _signInManager.ExternalLoginSignInAsync(externalLogin.LoginProvider, externalLogin.ProviderKey, false);
            var email = externalLogin.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (signinResult.Succeeded)
            {
                var jwt = CreateJWT(user);
                await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "jwt", jwt.Token);
                return jwt;
            }

            if (!string.IsNullOrEmpty(email))
            {
                if (user == null)
                {
                    user = new Domain.User()
                    {
                        FirstName = externalLogin.Principal.FindFirstValue(ClaimTypes.GivenName),
                        LastName = externalLogin.Principal.FindFirstValue(ClaimTypes.Surname),
                        UserName = externalLogin.Principal.FindFirstValue(ClaimTypes.Email),
                        Email = externalLogin.Principal.FindFirstValue(ClaimTypes.Email),
                        TwoFactorEnabled = false,
                        EmailConfirmed = true
                    };
                    await _userManager.CreateAsync(user);
                }
                await _userManager.AddLoginAsync(user, externalLogin);
                await _signInManager.SignInAsync(user, false);

                var jwt = CreateJWT(user);
                await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "jwt", jwt.Token);
                return jwt;
            }

            _logger.LogInformation("{ExternalLogin} failed", nameof(ExternalLogin));

            return null;
        }


        public async Task LoginWithGoogle()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new ArgumentNullException();
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (!result.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = new Domain.User()
                {
                    Email = email,
                    UserName = email,
                    TwoFactorEnabled = false
                };

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                }
            }
        }

        public Task LoginWithMicrosoft()
        {
            throw new NotImplementedException();
        }

        public Task LoginWithSlack()
        {
            throw new NotImplementedException();
        }


        public async Task RequestResetPassword(string email)
        {
            _logger.LogInformation("{RequestResetPassword} param value {email}", nameof(RequestResetPassword), email);

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var codeString = Conversor.ToBase64(code);

            var uriBuilder = new UriBuilder(_configuration["FrontEnd:ResetPassword"]!);
            uriBuilder.Query = $"activationToken={codeString}&email={user.Email}";
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

            _logger.LogInformation($"{nameof(RequestResetPassword)} {user}");
        }

        public async Task ResetPassword(ResetPassword resetPassword)
        {
            _logger.LogInformation("{ResetPassword} param value {email}", nameof(ResetPassword), resetPassword);

            if (resetPassword == null)
            {
                throw new ArgumentNullException(nameof(resetPassword));
            }
            resetPassword.EnsureValidation();

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
            {
                throw new NotFoundException(nameof(user));
            }

            var activationToken = Conversor.ToString(resetPassword.ActivationToken);

            var result = await _userManager.ResetPasswordAsync(user, activationToken, resetPassword.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException();
            }

            _logger.LogInformation($"{nameof(ResetPassword)} failed {user.Email}");
        }

        private Dtos.TokenResponse CreateJWT(Domain.User user)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Exp_time"]));
            Claim[] claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //Subject (userId)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //JWT unique ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), //Issued at (date and timeof token generation)
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), //Unit name Identifier of the user (Id or email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret_key"]));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenGenerator);

            return new Dtos.TokenResponse() { Token = token };
        }
    }
}
