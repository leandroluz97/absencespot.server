using Absencespot.Business.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Functions
{
    public class LeaveFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly ILeaveService _leaveService;

        public LeaveFunctions(ILogger<LeaveFunctions> logger, ILeaveService leaveService) : base(logger)
        {
            _logger = logger;
            _leaveService = leaveService;
        }


        [Function(nameof(CreateLeave))]
        public async Task<HttpResponseData> CreateLeave([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/leaves")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var leaveBody = JsonSerializer.Deserialize<Dtos.Leave>(req.Body, _jsonSerializerOptions);
            var leaveResponse = await _leaveService.CreateAsync(companyId, leaveBody);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(leaveResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetLeaveById))]
        public async Task<HttpResponseData> GetLeaveById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/leaves/{leaveId}")] 
        HttpRequestData req, Guid companyId, Guid leaveId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _leaveService.GetByIdAsync(companyId, leaveId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetAllLeaves))]
        public async Task<HttpResponseData> GetAllLeaves([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/leaves")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _leaveService.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(UpdateLeave))]
        public async Task<HttpResponseData> UpdateLeave([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/leaves/{leaveId}")] 
        HttpRequestData req, Guid companyId, Guid leaveId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var companyBody = JsonSerializer.Deserialize<Dtos.Leave>(req.Body, _jsonSerializerOptions);
            var companyResponse = await _leaveService.UpdateAsync(companyId, leaveId, companyBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(companyResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(DeleteLeave))]
        public async Task<HttpResponseData> DeleteLeave([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{companyId}/leaves/{leaveId}")]
        HttpRequestData req, Guid companyId, Guid leaveId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

           await _leaveService.DeleteAsync(companyId, leaveId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

    }
}
