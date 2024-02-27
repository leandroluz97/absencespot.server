using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Absencespot.Business.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Absencespot.ApiFunctions.Functions
{
    public class IntegrationFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly IIntegrationService _integrationService;

        public IntegrationFunctions(ILogger<IntegrationFunctions> logger, IIntegrationService integrationService) : base(logger)
        {
            _logger = logger;
            _integrationService = integrationService;
        }

        [Function(nameof(UpdateIntegration))]
        public async Task<HttpResponseData> UpdateIntegration([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/integrations")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var integrationBody = JsonSerializer.Deserialize<Dtos.Integration>(req.Body, _jsonSerializerOptions);
            var integrationResponse = await _integrationService.UpdateAsync(companyId, integrationBody, req.FunctionContext.CancellationToken);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(integrationResponse, _objectSerializer, req.FunctionContext.CancellationToken)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetIntegrationsByCompanyId))]
        public async Task<HttpResponseData> GetIntegrationsByCompanyId([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/integrations")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _integrationService.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }
    }
}
