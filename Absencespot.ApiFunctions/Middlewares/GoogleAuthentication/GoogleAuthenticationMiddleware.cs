using Absencespot.Clients.GoogleCalendar.Options;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Middlewares.GoogleAuthentication
{
    public class GoogleAuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly GoogleAuthOptions _options;

        public GoogleAuthenticationMiddleware(IOptions<GoogleAuthOptions> options)
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

            var googleAsymmetricsKey = await GetAsymmetricKeyAsync();

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                ValidateIssuer = _options.ValidateIssuer,
                ValidateAudience = _options.ValidateAudience,
                ValidateLifetime = _options.ValidateLifetime,
                ValidateIssuerSigningKey = _options.ValidateIssuerSigningKey,
                IssuerSigningKeys = googleAsymmetricsKey.Values
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

        private string TryGetTokenFromHeaders(HttpRequestData req)
        {
            if (!req.Headers.TryGetValues("X-GOOGLE-AUTH", out var authorizations))
            {
                throw new UnauthorizedAccessException("Header must have an 'X-GOOGLE-AUTH' value.");
            }

            var authorization = authorizations.FirstOrDefault();
            if (authorization == null)
            {
                throw new UnauthorizedAccessException("Authorization must have a value.");
            }
            string token = authorization.Replace("Bearer ", "");

            return token;
        }


        private async Task<IDictionary<string, SecurityKey>> GetAsymmetricKeyAsync()
        {
            try
            {
                IDictionary<string, SecurityKey> googleKeys = new Dictionary<string, SecurityKey>();
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/certs");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var keys = JObject.Parse(json);

                    foreach (var key in keys)
                    {
                        var keyParameters = new RSAParameters
                        {
                            Modulus = Base64UrlEncoder.DecodeBytes((string)key.Value["n"]),
                            Exponent = Base64UrlEncoder.DecodeBytes((string)key.Value["e"])
                        };

                        var rsa = RSA.Create();
                        rsa.ImportParameters(keyParameters);

                        googleKeys.Add((string)key.Key, new RsaSecurityKey(rsa));
                    }
                }
                return googleKeys;
            }
            catch (Exception)
            {
                throw new Exception("Error while requesting 'GOOGLE KEYS'.");
            }
        }
    }
}
