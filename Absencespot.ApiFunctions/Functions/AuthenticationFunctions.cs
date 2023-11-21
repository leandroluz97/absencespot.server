using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Absencespot.Business.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Absencespot.ApiFunctions.Functions
{
    public class AuthenticationFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationFunctions(
            ILogger<AuthenticationFunctions> logger,
            IAuthenticationService authenticationService) : base(logger)
        {
            _logger = logger;
            _authenticationService = authenticationService;
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
    }
}
