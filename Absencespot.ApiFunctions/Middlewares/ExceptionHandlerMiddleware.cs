using Absencespot.Services.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Middlewares
{
    public class ExceptionHandlerMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context)
                      .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is AggregateException)
                {
                    await HandleExceptionAsync(context, ex.InnerException)
                        .ConfigureAwait(false);
                }
                else
                {
                    await HandleExceptionAsync(context, ex)
                        .ConfigureAwait(false);
                }
            }
        }


        private async Task HandleExceptionAsync(FunctionContext context, Exception exception)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();
            var traceId = context.InvocationId;
            var logger = context.GetLogger<ExceptionHandlerMiddleware>();

            if (exception is NotFoundException)
            {
                InvokeResponseResult(context, httpRequestData, HttpStatusCode.NotFound);
            }
            else if (exception is ConflictException)
            {
                InvokeResponseResult(context, httpRequestData, HttpStatusCode.Conflict);
            }
            else if (exception is UnauthorizedAccessException)
            {
                InvokeResponseResult(context, httpRequestData, HttpStatusCode.Unauthorized);
            }
            else if (exception is ArgumentException || exception is ArgumentNullException)
            {
                var ex = exception as ArgumentException;
                var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.BadRequest);
                await httpResponse.WriteAsJsonAsync(new
                {
                    TraceId = traceId,
                    Code = HttpStatusCode.BadRequest.ToString(),
                    Parameter = ex.ParamName,
                    exception.Message,
                }, HttpStatusCode.BadRequest);

                var invocationResult = context.GetInvocationResult();
                invocationResult.Value = httpResponse;
            }
            else
            {
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    ["TraceId"] = traceId,
                    ["Exception"] = exception.GetType().Name,
                }))
                {
                    var eventId = new EventId(1001, exception.InnerException?.Message);
                    logger.Log(LogLevel.Critical, eventId, exception, exception.Message);

                    var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.InternalServerError);
                    await httpResponse.WriteAsJsonAsync(new
                    {
                        Code = HttpStatusCode.InternalServerError.ToString(),
                        Description = "Internal Server Error.",
                    }, HttpStatusCode.InternalServerError);

                    var invocationResult = context.GetInvocationResult();
                    invocationResult.Value = httpResponse;
                }
            }
        }

        private static void InvokeResponseResult(
            FunctionContext context,
            Microsoft.Azure.Functions.Worker.Http.HttpRequestData httpRequestData,
            HttpStatusCode statusCode)
        {
            var httpResponse = httpRequestData.CreateResponse(statusCode);
            var invocationResult = context.GetInvocationResult();
            invocationResult.Value = httpResponse;
        }

    }
}
