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
using System.Text.Json;

namespace Absencespot.ApiFunctions.Functions
{
    public class CalendarFunctions : BaseFunction
    {
        private readonly ILogger<CalendarFunctions> _logger;
        private readonly ICalendarService _calendarService;

        public CalendarFunctions(ILogger<CalendarFunctions> logger, ICalendarService calendarService):base(logger)
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

        [Function(nameof(GetCalendarContent))]
        public async Task<HttpResponseData> GetCalendarContent([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/calendars/feed")] HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var calendarId = "31cea3071fcfab61f3349200e6a65109c15f293f6c45745f9589d260ee9ea406@group.calendar.google.com";
            //var content = await _calendarService.GetEventsContent(calendarId, req.FunctionContext.CancellationToken);
            var content = await _calendarService.GetContent(calendarId, req.FunctionContext.CancellationToken);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/calendar;charset=utf-8");
            await response.WriteStringAsync(JsonSerializer.Serialize(content));

            //await response.WriteAsJsonAsync("", _objectSerializer, req.FunctionContext.CancellationToken)
            //              .ConfigureAwait(false);
            return response;
        }
    }
}
