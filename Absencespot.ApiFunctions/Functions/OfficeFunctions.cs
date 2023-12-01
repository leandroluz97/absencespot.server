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
    public class OfficeFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly IOfficeService _officeService;

        public OfficeFunctions(ILogger<OfficeFunctions> logger, IOfficeService leaveService) : base(logger)
        {
            _logger = logger;
            _officeService = leaveService;
        }


        [Function(nameof(CreateOffice))]
        public async Task<HttpResponseData> CreateOffice([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/offices")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var officeBody = JsonSerializer.Deserialize<Dtos.Office>(req.Body, _jsonSerializerOptions);
            var officeResponse = await _officeService.CreateAsync(companyId, officeBody);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(officeResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetOfficeById))]
        public async Task<HttpResponseData> GetOfficeById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/offices/{officeId}")] 
        HttpRequestData req, Guid companyId, Guid officeId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _officeService.GetByIdAsync(companyId, officeId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetAllOffices))]
        public async Task<HttpResponseData> GetAllOffices([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/offices")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _officeService.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(UpdateOffice))]
        public async Task<HttpResponseData> UpdateOffice([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/offices/{officeId}")] 
        HttpRequestData req, Guid companyId, Guid officeId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var officeBody = JsonSerializer.Deserialize<Dtos.Office>(req.Body, _jsonSerializerOptions);
            var companyResponse = await _officeService.UpdateAsync(companyId, officeId, officeBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(companyResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(DeleteOffice))]
        public async Task<HttpResponseData> DeleteOffice([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{companyId}/offices/{officeId}")]
        HttpRequestData req, Guid companyId, Guid officeId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

           await _officeService.DeleteAsync(companyId, officeId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

    }
}
