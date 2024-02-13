using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Middlewares.JwtAuthentication
{
    public class JwtAuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var req = await context.GetHttpRequestDataAsync();

            string token = TryGetTokenFromHeader(req);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException();
            }


            await next(context)
                .ConfigureAwait(false);
            // throw new NotImplementedException();
        }

        
        private string TryGetTokenFromHeader(HttpRequestData req)
        {
            if (!req.Headers.TryGetValues("Authorization", out var authorizations))
            {
                throw new UnauthorizedAccessException();
            }
            var authorization = authorizations.FirstOrDefault();
            if (authorization == null)
            {
                throw new UnauthorizedAccessException();
            }
            string token = authorization.Replace("Bearer ", "");

            return token;
        }
    }
}
