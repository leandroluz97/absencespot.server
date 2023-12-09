using Absencespot.Business.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Functions
{
    public class WorkScheduleFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly IWorkScheduleService _workSchedule;

        public WorkScheduleFunctions(ILogger<WorkScheduleFunctions> logger, IWorkScheduleService workScheduleService) : base(logger)
        {
            _logger = logger;
            _workSchedule = workScheduleService;
        }


        [Function(nameof(CreateWorkSchedule))]
        public async Task<HttpResponseData> CreateWorkSchedule([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/workSchedules")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var workScheduleBody = JsonSerializer.Deserialize<Dtos.WorkSchedule>(req.Body, _jsonSerializerOptions);

            var WorkScheduleResponse = await _workSchedule.CreateAsync(companyId, workScheduleBody);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(WorkScheduleResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetWorkScheduleById))]
        public async Task<HttpResponseData> GetWorkScheduleById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/workSchedules/{workScheduleId}")]
        HttpRequestData req, Guid companyId, Guid workScheduleId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _workSchedule.GetByIdAsync(companyId, workScheduleId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetAllWorkSchedules))]
        public async Task<HttpResponseData> GetAllWorkSchedules([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/workSchedules")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _workSchedule.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(UpdateWorkSchedule))]
        public async Task<HttpResponseData> UpdateWorkSchedule([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/workSchedules/{workScheduleId}")]
        HttpRequestData req, Guid companyId, Guid workScheduleId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var WorkScheduleBody = JsonSerializer.Deserialize<Dtos.WorkSchedule>(req.Body, _jsonSerializerOptions);
            var companyResponse = await _workSchedule.UpdateAsync(companyId, workScheduleId, WorkScheduleBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(companyResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(DeleteWorkSchedule))]
        public async Task<HttpResponseData> DeleteWorkSchedule([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{companyId}/workSchedules/{workScheduleId}")]
        HttpRequestData req, Guid companyId, Guid workScheduleId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _workSchedule.DeleteAsync(companyId, workScheduleId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

    }
}
