using Absencespot.Clients.GoogleCalendar.Options;
using Absencespot.Dtos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Middlewares.MicrosoftAuthentication
{
    public class MicrosoftAuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly MicrosoftAuthOptions _options;


        public MicrosoftAuthenticationMiddleware(IOptions<MicrosoftAuthOptions> options)
        {
            _options = options.Value;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var req = await context.GetHttpRequestDataAsync();
            var token = TryGetTokenFromHeaders(req);

            if (string.IsNullOrWhiteSpace(token))
            {
                var httpResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                var invocationResult = context.GetInvocationResult();
                invocationResult.Value = httpResponse;
                return;
            }

            var microsoftAsymmetricsKey = await GetAsymmetricKeyAsync();
            //var microsoftAsymmetricsKey = await IssuerSigningKeysAsync();

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                ValidateIssuer = _options.ValidateIssuer,
                ValidateAudience = _options.ValidateAudience,
                ValidateLifetime = _options.ValidateLifetime,
                ValidateIssuerSigningKey = _options.ValidateIssuerSigningKey,
                IssuerSigningKeys = microsoftAsymmetricsKey,
            };

            TokenValidationResult result = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
            if (!result.IsValid)
            {
                var httpResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                var invocationResult = context.GetInvocationResult();
                invocationResult.Value = httpResponse;
                return;
            }

            await next(context)
                .ConfigureAwait(false);
        }

        private string TryGetTokenFromHeaders(Microsoft.Azure.Functions.Worker.Http.HttpRequestData req)
        {
            if (!req.Headers.TryGetValues("X-MICROSOFT-AUTH", out var authorizations))
            {
                throw new UnauthorizedAccessException("Header must have an 'X-MICROSOFT-AUTH' value.");
            }

            var authorization = authorizations.FirstOrDefault();
            if (authorization == null)
            {
                throw new UnauthorizedAccessException("Authorization must have a value.");
            }
            string token = authorization.Replace("Bearer ", "");

            return token;
        }

        private async Task<IEnumerable<SecurityKey>> GetAsymmetricKeyAsync()
        {
            string microsoftAuthority = "https://login.microsoftonline.com/ce66180d-cb09-4ddd-8b83-77c7ef72a60b/v2.0/.well-known/openid-configuration";

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(microsoftAuthority, new OpenIdConnectConfigurationRetriever());

            var openIdConfig = await configurationManager.GetConfigurationAsync();

            return openIdConfig.SigningKeys;
        }

        private async Task<IEnumerable<SecurityKey>> IssuerSigningKeysAsync()
        {
            //string jwksUri = "https://login.microsoftonline.com/common/discovery/v2.0/keys";
            string jwksUri = "https://login.microsoftonline.com/ce66180d-cb09-4ddd-8b83-77c7ef72a60b/discovery/v2.0/keys";
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(jwksUri);
                response.EnsureSuccessStatusCode();

                var jwksJson = await response.Content.ReadAsStringAsync();
                var jwks = new JsonWebKeySet(jwksJson);

                return jwks.Keys;
            }
        }
    }
}
