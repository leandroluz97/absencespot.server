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
    public class TrackRecordFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly ITrackRecordService _trackRecordService;

        public TrackRecordFunctions(ILogger<RequestFunction> logger, ITrackRecordService trackRecordService) : base(logger)
        {
            _trackRecordService = trackRecordService;
        }

        [Function(nameof(CreateTrackRecord))]
        public async Task<HttpResponseData> CreateTrackRecord([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/track-records")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var trackRecordBody = JsonSerializer.Deserialize<Dtos.TrackRecord>(req.Body, _jsonSerializerOptions);
            var trackRecordResponse = await _trackRecordService.CreateAsync(companyId, trackRecordBody);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(trackRecordResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetRequestById))]
        public async Task<HttpResponseData> GetRequestById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/track-records/{trackRecordId}")]
        HttpRequestData req, Guid companyId, Guid trackRecordId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _trackRecordService.GetByIdAsync(companyId, trackRecordId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetAllRequests))]
        public async Task<HttpResponseData> GetAllRequests([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/track-records")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _trackRecordService.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(UpdateRequest))]
        public async Task<HttpResponseData> UpdateRequest([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/track-records/{trackRecordId}")]
        HttpRequestData req, Guid companyId, Guid trackRecordId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var trackRecordBody = JsonSerializer.Deserialize<Dtos.TrackRecord>(req.Body, _jsonSerializerOptions);
            var trackRecordResponse = await _trackRecordService.UpdateAsync(companyId, trackRecordId, trackRecordBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(trackRecordResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetRequestMetrics))]
        public async Task<HttpResponseData> GetRequestMetrics([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/track-records/{trackRecordId}/metrics")]
        HttpRequestData req, Guid companyId, Guid trackRecordId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var trackRecordBody = JsonSerializer.Deserialize<Dtos.ApproveRequest>(req.Body, _jsonSerializerOptions);
            var trackRecordResponse = await _trackRecordService.GetMetricsByIdAsync(companyId, trackRecordId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(trackRecordResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(DeleteRequest))]
        public async Task<HttpResponseData> DeleteRequest([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{companyId}/track-records/{trackRecordId}")]
        HttpRequestData req, Guid companyId, Guid trackRecordId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _trackRecordService.DeleteAsync(companyId, trackRecordId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
