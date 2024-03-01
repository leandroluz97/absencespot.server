using Absencespot.Business.Abstractions;
using Absencespot.Dtos;
using Absencespot.Infrastructure.Abstractions.Clients.Calendar;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;

namespace Absencespot.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly ILogger<CompanyService> _logger;
        private readonly ICalendarClient _calendarClient;

        public CalendarService(ILogger<CompanyService> logger, ICalendarClient calendarClient)
        {
            _logger = logger;
            _calendarClient = calendarClient;
        }

        public async Task<CalendarListEntry> GetContent(string id, CancellationToken cancellationToken = default)
        {
            var calendar = await _calendarClient.GetById(id, cancellationToken);

            var eventCalendar = new Event()
            {
                
                Summary = "Last day fev",
                Location = "800 Howard St., San Francisco, CA 94103",
                Description = "A chance to hear more about Google's developer products.",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2024-02-29T00:00:00-00:00"),
                    TimeZone = "America/Los_Angeles",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2024-02-29T00:05:05-05:00"),
                    TimeZone = "America/Los_Angeles",
                },

            };
            eventCalendar = await _calendarClient.CreateEventAsync(id, eventCalendar);

            return calendar;
        }

        public async Task<Events> GetEventsContent(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var eventsVC = await _calendarClient.GetByOwnerId(id, cancellationToken);

            var events = await _calendarClient.GetEventListAsync(id, cancellationToken);
            return events;
        }

        public async Task<CalendarFeed> GetUrlsAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default)
        {
            if (companyId == default)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            //_calendarClient.GetById();
            var calendars = await _calendarClient.GetListAsync();

            throw new NotImplementedException();
        }
    }
}
