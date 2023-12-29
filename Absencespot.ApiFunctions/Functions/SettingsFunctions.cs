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
    public class SettingsFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly ISettingsService _settingsService;

        public SettingsFunctions(ILogger<SettingsFunctions> logger, ISettingsService settingsService) : base(logger)
        {
            _logger = logger;
            _settingsService = settingsService;
        }


        [Function(nameof(UpdateSettings))]
        public async Task<HttpResponseData> UpdateSettings([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/settings")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var settingsBody = JsonSerializer.Deserialize<Dtos.Settings>(req.Body, _jsonSerializerOptions);
            var trackRecordResponse = await _settingsService.UpdateAsync(companyId, settingsBody);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(trackRecordResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetSettingsByCompanyId))]
        public async Task<HttpResponseData> GetSettingsByCompanyId([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/settings")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _settingsService.GetByIdAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

    }
}
