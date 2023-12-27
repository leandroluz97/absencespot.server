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
            _logger = logger;
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

        [Function(nameof(GetTrackRecordMetrics))]
        public async Task<HttpResponseData> GetTrackRecordMetrics([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/track-records/metrics/all")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var trackRecordResponse = await _trackRecordService.GetMetricsByUserIdAsync(companyId, new Guid("B0BB1E63-3688-4CEC-A592-2D9EBE3C88F2"));

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(trackRecordResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetTrackRecordById))]
        public async Task<HttpResponseData> GetTrackRecordById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/track-records/{trackRecordId}")]
        HttpRequestData req, Guid companyId, Guid trackRecordId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _trackRecordService.GetByIdAsync(companyId, trackRecordId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetAllTrackRecords))]
        public async Task<HttpResponseData> GetAllTrackRecords([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/track-records")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _trackRecordService.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(UpdateTrackRecord))]
        public async Task<HttpResponseData> UpdateTrackRecord([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/track-records/{trackRecordId}")]
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


        [Function(nameof(DeleteTrackRecord))]
        public async Task<HttpResponseData> DeleteTrackRecord([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{companyId}/track-records/{trackRecordId}")]
        HttpRequestData req, Guid companyId, Guid trackRecordId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _trackRecordService.DeleteAsync(companyId, trackRecordId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
