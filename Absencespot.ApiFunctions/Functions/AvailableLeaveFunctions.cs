using System;
using System.Net;
using System.Threading.Tasks;
using Absencespot.Business.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Absencespot.ApiFunctions.Functions
{
    public class AvailableLeaveFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly IAvailableLeaveService _availableLeaveService;

        public AvailableLeaveFunctions(ILogger<IntegrationFunctions> logger, IAvailableLeaveService availableLeaveService) : base(logger)
        {
            _logger = logger;
            _availableLeaveService = availableLeaveService;
        }


        [Function(nameof(GetAllAvailableLeaves))]
        public async Task<HttpResponseData> GetAllAvailableLeaves([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/available-leaves")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _availableLeaveService.GetAllAsync(companyId, new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2"));

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }
    }
}
