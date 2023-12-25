using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Absencespot.Business.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Absencespot.ApiFunctions
{
    public class RequestFunction : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly IRequestService _requestService;

        public RequestFunction(ILogger<RequestFunction> logger, IRequestService requestService) : base(logger)
        {
            _requestService = requestService;
        }

        [Function(nameof(CreateRequest))]
        public async Task<HttpResponseData> CreateRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/requests")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = JsonSerializer.Deserialize<Dtos.Request>(req.Body, _jsonSerializerOptions);
            var requestResponse = await _requestService.CreateAsync(companyId, requestBody);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(requestResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetRequestById))]
        public async Task<HttpResponseData> GetRequestById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/requests/{requestId}")]
        HttpRequestData req, Guid companyId, Guid requestId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _requestService.GetByIdAsync(companyId, requestId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetAllRequests))]
        public async Task<HttpResponseData> GetAllRequests([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/requests")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _requestService.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(UpdateRequest))]
        public async Task<HttpResponseData> UpdateRequest([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/requests/{requestId}")]
        HttpRequestData req, Guid companyId, Guid requestId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = JsonSerializer.Deserialize<Dtos.Request>(req.Body, _jsonSerializerOptions);
            var requestResponse = await _requestService.UpdateAsync(companyId, requestId, requestBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(requestResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(ApproveRequest))]
        public async Task<HttpResponseData> ApproveRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/requests/{requestId}/approve")]
        HttpRequestData req, Guid companyId, Guid requestId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = JsonSerializer.Deserialize<Dtos.ApproveRequest>(req.Body, _jsonSerializerOptions);
            var requestResponse = await _requestService.ApproveAsync(companyId, requestId, requestBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(requestResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(RejectRequest))]
        public async Task<HttpResponseData> RejectRequest([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/requests/{requestId}/reject")]
        HttpRequestData req, Guid companyId, Guid requestId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = JsonSerializer.Deserialize<Dtos.RejectRequest>(req.Body, _jsonSerializerOptions);
            var requestResponse = await _requestService.RejectAsync(companyId, requestId, requestBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(requestResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(DeleteRequest))]
        public async Task<HttpResponseData> DeleteRequest([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{companyId}/requests/{requestId}")]
        HttpRequestData req, Guid companyId, Guid requestId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _requestService.DeleteAsync(companyId, requestId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

    }
}
