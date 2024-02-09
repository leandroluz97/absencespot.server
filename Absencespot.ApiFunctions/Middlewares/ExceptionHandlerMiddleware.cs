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
                await next(context);
            }
            catch (Exception ex)
            {
                //HttpContext must not be null.
                await HandleExceptionAsync(context, ex);
            }
        }


        private async Task HandleExceptionAsync(FunctionContext context, Exception exception)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();
            var traceId = context.InvocationId;
            var logger = context.GetLogger<ExceptionHandlerMiddleware>();

            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["TraceId"] = traceId,
                ["Exception"] = exception.GetType().Name,   
            }))
            {
                if (exception is NotFoundException)
                {
                    var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.NotFound);
                    var invocationResult = context.GetInvocationResult();
                    invocationResult.Value = httpResponse;
                    return;
                }
                else if (exception is ConflictException)
                {
                    var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.Conflict);
                    var invocationResult = context.GetInvocationResult();
                    invocationResult.Value = httpResponse;
                    return;
                }
                else if (exception is UnauthorizedAccessException)
                {
                    var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.Unauthorized);
                    var invocationResult = context.GetInvocationResult();
                    invocationResult.Value = httpResponse;
                    return;
                }
                else if (exception is ArgumentException || exception is ArgumentNullException)
                {
                    var ex = (ArgumentException)exception;
                    var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.BadRequest);
                    await httpResponse.WriteAsJsonAsync(new
                    {
                        TraceId = traceId,
                        StatusCode = HttpStatusCode.BadRequest,
                        Parameter = ex.ParamName,
                        exception.Message,
                    }, HttpStatusCode.BadRequest);

                    var invocationResult = context.GetInvocationResult();
                    invocationResult.Value = httpResponse;
                    return;
                }
                else
                {
                    var eventId = new EventId(1001, exception.InnerException?.Message);
                    logger.Log(LogLevel.Critical, eventId, exception, exception.Message);

                    var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.InternalServerError);
                    await httpResponse.WriteAsJsonAsync(new
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Message = "Internal Server Error.",
                    }, HttpStatusCode.InternalServerError);

                    var invocationResult = context.GetInvocationResult();
                    invocationResult.Value = httpResponse;
                    return;
                }
            }
        }
    }
}
