﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Middlewares.JwtAuthentication
{
    public class JwtAuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly JwtAuthenticationOptions _options;
        public JwtAuthenticationMiddleware(IOptions<JwtAuthenticationOptions> options)
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

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                ValidateIssuer = _options.ValidateIssuer,
                ValidateAudience = _options.ValidateAudience,
                ValidateLifetime = _options.ValidateLifetime,
                ValidateIssuerSigningKey = _options.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey))
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
            if (!req.Headers.TryGetValues("Authorization", out var authorizations))
            {
                throw new UnauthorizedAccessException("Header must have an Authorization value.");
            }

            var authorization = authorizations.FirstOrDefault();
            if (authorization == null)
            {
                throw new UnauthorizedAccessException("Authorization must have a value.");
            }
            string token = authorization.Replace("Bearer ", "");

            return token;
        }
    }
}
