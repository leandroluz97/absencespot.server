using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Absencespot.ApiFunctions
{
    public abstract class BaseFunction
    {
        protected readonly ILogger _logger;
        protected readonly JsonSerializerOptions _jsonserializerOptions;

        public BaseFunction(ILogger logger)
        {
            _logger = logger;
            _jsonserializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        //[Function("BaseFunction")]
        //public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        //{
        //    _logger.LogInformation("C# HTTP trigger function processed a request.");

        //    var response = req.CreateResponse(HttpStatusCode.OK);
        //    response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        //    response.WriteString("Welcome to Azure Functions!");

        //    return response;
        //}
    }
}
