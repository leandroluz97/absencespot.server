using Absencespot.Services.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(FunctionContext context, Exception exception)
        {
            var httpRequestData = GetHttpRequestData(context);

            var traceId = context.InvocationId;
            var logger = context.GetLogger<ExceptionHandlerMiddleware>();
            using (logger.BeginScope("Exception"))
            {
                var eventId = new EventId(1001, exception.InnerException?.Message);
                logger.Log(LogLevel.Error, eventId, exception, exception.Message);
            }


            if(exception is ArgumentNullException || exception is ArgumentNullException)
            {
                var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.InternalServerError);
                // context.StatusCode = (int)statusCode;
            }
            if (exception is NotFoundException)
            {

            }
            if (exception is InvalidOperationException)
            {

            }

        }

        public HttpRequestData GetHttpRequestData(FunctionContext functionContext)
        {
            try
            {
                KeyValuePair<Type, object> keyValuePair = functionContext.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
                object functionBindingsFeature = keyValuePair.Value;
                Type type = functionBindingsFeature.GetType();
                var inputData = type.GetProperties().Single(p => p.Name == "InputData").GetValue(functionBindingsFeature) as IReadOnlyDictionary<string, object>;
                return inputData?.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
            }
            catch
            {
                return null;
            }
        }
    }
}
