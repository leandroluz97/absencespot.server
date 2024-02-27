using Azure.Core.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using System;
using Absencespot.Business.Abstractions;

namespace Absencespot.ApiFunctions.Functions
{
    public class CalendarFunctions : BaseFunction
    {
        private readonly ILogger<CalendarFunctions> _logger;
        private readonly ICalendarService _calendarService;

        public CalendarFunctions(ILogger<CalendarFunctions> logger, ICalendarService calendarService)
        {
            _logger = logger;
            _calendarService = calendarService;
        }


        [Function(nameof(GetCalendarUrlsById))]
        public async Task<HttpResponseData> GetCalendarUrlsById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/calendars")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _calendarService.GetUrlsAsync(companyId, Guid.NewGuid(), req.FunctionContext.CancellationToken);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync("", _objectSerializer, req.FunctionContext.CancellationToken)
                          .ConfigureAwait(false);
            return response;
        }
    }
}
