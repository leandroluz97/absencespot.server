using Absencespot.Business.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions
{
    public class CompanyFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly ICompanyService _companyService;
        

        public CompanyFunctions(ILogger<CompanyFunctions> logger, ICompanyService companyService) : base(logger)
        {
            _logger = logger;
            _companyService = companyService;
        }

        [Function(nameof(CreateAsync))]
        public async Task<HttpResponseData> CreateAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = new
            {
                Name = "Leandro",
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer )
                          .ConfigureAwait(false);             
            return  response;
        }

        [Function(nameof(GetByIdAsync))]
        public async Task<HttpResponseData> GetByIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _companyService.GetByIdAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }
    }
}
