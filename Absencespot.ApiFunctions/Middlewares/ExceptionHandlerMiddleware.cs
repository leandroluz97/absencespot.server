using Absencespot.Services.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            {//HttpContext must not be null.
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task<HttpResponseData> HandleExceptionAsync(FunctionContext context, Exception exception)
        {
            var httpRequestData = GetHttpRequestData(context);

            var traceId = context.InvocationId;
            var logger = context.GetLogger<ExceptionHandlerMiddleware>();
            using (logger.BeginScope("Exception"))
            {
                var eventId = new EventId(1001, exception.InnerException?.Message);
                logger.Log(LogLevel.Error, eventId, exception, exception.Message);
            }

            //GetInvocationResult()

            var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.InternalServerError);
            await httpResponse.WriteAsJsonAsync(new { FooStatus = "Invocation failed!" });
            return httpResponse;

            if (exception is ArgumentNullException || exception is ArgumentNullException)
            {
                //var httpResponse = httpRequestData.CreateResponse(HttpStatusCode.InternalServerError);
                //await httpResponse.WriteAsJsonAsync(new { FooStatus = "Invocation failed!" }, httpResponse.StatusCode);
                //var invocationResult = GetHttpResponseData(context);
                
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

        public  HttpResponseData GetHttpResponseData(FunctionContext functionContext)
        {
            try
            {
                var request = GetHttpRequestData(functionContext);
                if (request == null) return null;
                var response = HttpResponseData.CreateResponse(request);
                var keyValuePair = functionContext.Features.FirstOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
                if (keyValuePair.Equals(default(KeyValuePair<Type, object>))) return null;
                object functionBindingsFeature = keyValuePair.Value;
                if (functionBindingsFeature == null) return null;
                PropertyInfo pinfo = functionBindingsFeature.GetType().GetProperty("InvocationResult");
                pinfo.SetValue(functionBindingsFeature, response);
                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}
