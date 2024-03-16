using Absencespot.Business.Abstractions;
using Absencespot.Clients.GoogleCalendar.Options;
using Absencespot.Clients.Sendgrid;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions.Clients;
using Absencespot.Services.Exceptions;
using Absencespot.Utils;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace Absencespot.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Domain.User> _userManager;
        private readonly SignInManager<Domain.User> _signInManager;
        private readonly IEmailClient _sendgridClient;
        private readonly GoogleAuthOptions _googleAuthOptions;
        private readonly MicrosoftAuthOptions _microsoftAuthOptions;

        public AuthenticationService(
            ILogger<AuthenticationService> logger,
            IConfiguration configuration,
            UserManager<Domain.User> userManager,
            SignInManager<Domain.User> signInManager,
            IEmailClient sendgridClient,
            IOptions<GoogleAuthOptions> googleAuthOptions,
            IOptions<MicrosoftAuthOptions> microsoftAuthOptions)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _sendgridClient = sendgridClient;
            _googleAuthOptions = googleAuthOptions.Value;
            _microsoftAuthOptions = microsoftAuthOptions.Value;
        }


        public async Task Register(Dtos.Register register, CancellationToken cancellationToken = default)
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
                UserName = register.Email,
                Position = register.Position,
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false,
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

        public async Task ConfirmEmail(Dtos.ConfirmEmail confirmEmail, CancellationToken cancellationToken = default)
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

            var token = Conversor.ToString(confirmEmail.Token);

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException();
            }

            user.FirstName = confirmEmail.FirstName;
            user.LastName = confirmEmail.LastName;
            user.PhoneNumber = confirmEmail.PhoneNumber;

            await _userManager.AddPasswordAsync(user, confirmEmail.Password);

            _logger.LogInformation($"{nameof(ConfirmEmail)} {user.Email}");
        }

        public async Task<Dtos.TokenResponse> Login(LoginRequest login, CancellationToken cancellationToken = default)
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

            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unable to authenticate user {user.Email}.");
            }

            //await _signInManager.SignInAsync(user, isPersistent: false, authenticationMethod: "Local");

            return CreateJWT(user);
        }

        public async Task<TokenResponse> ExternalLogin(ExternalLoginInfo externalLogin, CancellationToken cancellationToken = default)
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

        public async Task RequestResetPassword(string email, CancellationToken cancellationToken = default)
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

        public async Task ResetPassword(ResetPassword resetPassword, CancellationToken cancellationToken = default)
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

            //long unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var unixTimestamp = EpochTime.GetIntDate(DateTime.Now).ToString(CultureInfo.InvariantCulture);
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationTime"]));

            Claim[] claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.GlobalId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, unixTimestamp, ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.GlobalId.ToString()),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));

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

        public async Task<TokenResponse> GetTokenFromAuthorizationCode(AuthorizationCode authorizationCode, CancellationToken cancellationToken = default)
        {
            if (authorizationCode == null)
            {
                throw new ArgumentNullException("Authorization Code is required", nameof(authorizationCode));
            }
            authorizationCode.EnsureValidation();

            switch (authorizationCode.IdentityProvider)
            {
                case "google":
                    return await GetGoogleTokenFromAuthorizationCode(authorizationCode, cancellationToken);
                case "microsoft":
                    return await GetMicrosoftTokenFromAuthorizationCode(authorizationCode, cancellationToken);
                default:
                    throw new ArgumentException("Invalid Identity Provider");
            }
        }

        private async Task<TokenResponse> GetGoogleTokenFromAuthorizationCode(AuthorizationCode authorizationCode, CancellationToken cancellationToken = default)
        {
            if (authorizationCode == null)
            {
                throw new ArgumentNullException("Authorization Code is required", nameof(authorizationCode));
            }
            authorizationCode.EnsureValidation();

            var googleAuthorization = new AuthorizationCodeTokenRequest()
            {
                Code = authorizationCode.Code,
                ClientId = _googleAuthOptions.ClientId,
                ClientSecret = _googleAuthOptions.ClientSecret,
                RedirectUri = "http://localhost:3000", // Must match the one used during authorization
                GrantType = "authorization_code"
            };

            var httpClient = new HttpClient();
            var tokenUrl = "https://oauth2.googleapis.com/token";

            var tokenResponse = await googleAuthorization.ExecuteAsync(
                httpClient,
                tokenUrl,
                cancellationToken,
                SystemClock.Default);

            return new TokenResponse()
            {
                IdToken = tokenResponse.IdToken,
                Token = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken
            };
        }

        private async Task<TokenResponse> GetMicrosoftTokenFromAuthorizationCode(AuthorizationCode authorizationCode, CancellationToken cancellationToken = default)
        {
            if (authorizationCode == null)
            {
                throw new ArgumentNullException("Authorization Code is required", nameof(authorizationCode));
            }
            authorizationCode.EnsureValidation();

            var app = ConfidentialClientApplicationBuilder
                .Create(_microsoftAuthOptions.ClientId)
                .WithClientSecret(_microsoftAuthOptions.ClientSecret)
                .WithRedirectUri(_microsoftAuthOptions.RedirectUri)
                .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
                .Build();

            string[] scopes = _microsoftAuthOptions.Scopes.Split(" ");

            var result = await app.AcquireTokenByAuthorizationCode(scopes, authorizationCode.Code)
                .WithPkceCodeVerifier(authorizationCode.CodeVerifier)
                .ExecuteAsync(cancellationToken);

            return new TokenResponse()
            {
                IdToken = result.IdToken,
                Token = result.AccessToken,
                RefreshToken = result.TokenType,
            };
        }
    }
}
