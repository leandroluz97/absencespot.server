using Absencespot.Business.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Absencespot.ApiFunctions.Functions
{
    public class TeamFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly ITeamService _teamService;

        public TeamFunctions(ILogger<TeamFunctions> logger, ITeamService teamService) : base(logger)
        {
            _logger = logger;
            _teamService = teamService;
        }


        [Function(nameof(CreateTeam))]
        public async Task<HttpResponseData> CreateTeam([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/teams")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var teamBody = JsonSerializer.Deserialize<Dtos.Team>(req.Body, _jsonSerializerOptions);
            var TeamResponse = await _teamService.CreateAsync(companyId, teamBody);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(TeamResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetTeamById))]
        public async Task<HttpResponseData> GetTeamById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/teams/{teamId}")]
        HttpRequestData req, Guid companyId, Guid teamId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _teamService.GetByIdAsync(companyId, teamId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetAllTeams))]
        public async Task<HttpResponseData> GetAllTeams([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/teams")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _teamService.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(UpdateTeam))]
        public async Task<HttpResponseData> UpdateTeam([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/teams/{teamId}")]
        HttpRequestData req, Guid companyId, Guid teamId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var TeamBody = JsonSerializer.Deserialize<Dtos.Team>(req.Body, _jsonSerializerOptions);
            var companyResponse = await _teamService.UpdateAsync(companyId, teamId, TeamBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(companyResponse, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(DeleteTeam))]
        public async Task<HttpResponseData> DeleteTeam([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{companyId}/teams/{teamId}")]
        HttpRequestData req, Guid companyId, Guid teamId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _teamService.DeleteAsync(companyId, teamId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

    }
}
