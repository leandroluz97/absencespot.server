using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Absencespot.Business.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Absencespot.ApiFunctions.Functions
{
    public class AuthenticationFunctions : BaseFunction
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly SignInManager<Domain.User> _signInManager;

        public AuthenticationFunctions(
            ILogger<AuthenticationFunctions> logger,
            IAuthenticationService authenticationService,
            SignInManager<Domain.User> signInManager
            ) : base(logger)
        {
            _authenticationService = authenticationService;
            _signInManager = signInManager;
        }


        [Function(nameof(RegisterAsync))]
        public async Task<HttpResponseData> RegisterAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/register")] HttpRequestData req)
        {
            _logger.LogInformation($"{nameof(RegisterAsync)} HTTP trigger function processed a request.");

            var registerBody = JsonSerializer.Deserialize<Dtos.Register>(req.Body, _jsonSerializerOptions);
            await _authenticationService.Register(registerBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        [Function(nameof(ConfirmEmailAsync))]
        public async Task<HttpResponseData> ConfirmEmailAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/confirm")] HttpRequestData req)
        {
            _logger.LogInformation($"{nameof(ConfirmEmailAsync)} HTTP trigger function processed a request.");

            var confirmEmailBody = JsonSerializer.Deserialize<Dtos.ConfirmEmail>(req.Body, _jsonSerializerOptions);
            await _authenticationService.ConfirmEmail(confirmEmailBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        [Function(nameof(LoginAsync))]
        public async Task<HttpResponseData> LoginAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/login")] HttpRequestData req)
        {
            _logger.LogInformation($"{nameof(LoginAsync)} HTTP trigger function processed a request.");

            var loginBody = JsonSerializer.Deserialize<Dtos.LoginRequest>(req.Body, _jsonSerializerOptions);
            var loginResponse = await _authenticationService.Login(loginBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(loginResponse, _objectSerializer)
                .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(RequestResetPasswordAsync))]
        public async Task<HttpResponseData> RequestResetPasswordAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/password-reset")] HttpRequestData req)
        {
            _logger.LogInformation($"{nameof(RequestResetPasswordAsync)} HTTP trigger function processed a request.");

            var requestResetPasswordBody = JsonSerializer.Deserialize<Dtos.PasswordReset>(req.Body, _jsonSerializerOptions);
            await _authenticationService.RequestResetPassword(requestResetPasswordBody.Email);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [Function(nameof(ResetPasswordAsync))]
        public async Task<HttpResponseData> ResetPasswordAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/reset-password")] HttpRequestData req)
        {
            _logger.LogInformation($"{nameof(ResetPasswordAsync)} HTTP trigger function processed a request.");

            var resetPasswordBody = JsonSerializer.Deserialize<Dtos.ResetPassword>(req.Body, _jsonSerializerOptions);
            await _authenticationService.ResetPassword(resetPasswordBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        [Function(nameof(ExternalLogin))]
        public async Task<ChallengeResult> ExternalLogin(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/external-login")] HttpRequestData req)
        {
            _logger.LogInformation($"{nameof(ExternalLogin)} HTTP trigger function processed a request.");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var provider = query["provider"];
            var returnUrl = query["returnUrl"];
            var redirectUrl = $"{GetFunctionAppBaseUrl(req)}/api/authentication/external-auth-callBack?returnUrl={returnUrl}";

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.AllowRefresh = true;
            var challengeResult = new ChallengeResult(redirectUrl, properties);
            return challengeResult;
        }

        [Function(nameof(ExternalAuthCallBack))]
        public async Task<HttpResponseData> ExternalAuthCallBack(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authentication/external-auth-callBack")] HttpRequestData req)
        {
            _logger.LogInformation($"{nameof(ExternalAuthCallBack)} HTTP trigger function processed a request.");

            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            var result = await _authenticationService.ExternalLogin(info);

            if (result == null)
            {
                throw new InvalidOperationException();
            }

            var redirectUrl = $"http://localhost:3000/?token={result.Token}";

            var response = req.CreateResponse(HttpStatusCode.Redirect);
            response.Headers.Add("Location", redirectUrl);
            return response;
        }

        private static string GetFunctionAppBaseUrl(HttpRequestData req)
        {
            return $"{req.Url.Scheme}://{req.Url.Host}{(req.Url.IsDefaultPort ? string.Empty : $":{req.Url.Port}")}";
        }
    }
}
