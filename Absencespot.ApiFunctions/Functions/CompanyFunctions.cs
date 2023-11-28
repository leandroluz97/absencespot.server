using Absencespot.Business.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
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

        [Function(nameof(CreateCompany))]
        public async Task<HttpResponseData> CreateCompany([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            //Guid subscriptionId;
            //if (!Guid.TryParse(query["subscriptionId"], out subscriptionId))
            //{
            //    throw new ArgumentNullException(nameof(subscriptionId));
            //}

            var companyBody = JsonSerializer.Deserialize<Dtos.Company>(req.Body, _jsonSerializerOptions);
            var companyResponse = await _companyService.CreateAsync(companyBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(companyResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetCompanyById))]
        public async Task<HttpResponseData> GetCompanyById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _companyService.GetByIdAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(UpdateCompany))]
        public async Task<HttpResponseData> UpdateCompany([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var companyBody =  JsonSerializer.Deserialize<Dtos.Company>(req.Body, _jsonSerializerOptions); 
            var companyResponse = await _companyService.UpdateAsync(companyId, companyBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(companyResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }
    }
}
