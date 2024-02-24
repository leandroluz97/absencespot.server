using Absencespot.Clients.GoogleCalendar.Options;
using Absencespot.Infrastructure.Abstractions.Clients.Calendar;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;

namespace Absencespot.Clients.GoogleCalendar
{
    public class CalendarClient : ICalendarClient
    {
        private readonly GoogleAuthOptions _options;
        private CalendarService _services;

        public CalendarClient(IOptions<GoogleAuthOptions> options)
        {
            _options = options.Value;
            _services = Initialize();
        }

        private CalendarService Initialize()
        {
            var credential = GoogleCredential.FromFile(_options.KeyFilePath)
                .CreateScoped(CalendarService.Scope.Calendar);

            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _options.ApplicationName,
            });
        }

        public async Task<Calendar> CreateAsync(Calendar options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException("Calendar options is required.", nameof(options));
            }

            var insertCalendarRequest = _services.Calendars.Insert(options);
            var calendarResult = await insertCalendarRequest.ExecuteAsync(cancellationToken);

            return calendarResult;
        }

        public async Task<Event> CreateEventAsync(string calendarId, Event options, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(calendarId))
            {
                throw new ArgumentNullException("Calendar Id is required.", nameof(options));
            }

            var insertEventRequest = _services.Events.Insert(options, calendarId);
            var eventResult = await insertEventRequest.ExecuteAsync(cancellationToken);

            return eventResult;
        }

        public async Task DeleteAsync(string calendarId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(calendarId))
            {
                throw new ArgumentNullException("Calendar Id is required.", nameof(calendarId));
            }

            var calendarRequest = _services.CalendarList.Delete(calendarId);
            await calendarRequest.ExecuteAsync(cancellationToken);
        }

        public async Task DeleteEventAsync(string calendarId, string eventId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(calendarId))
            {
                throw new ArgumentNullException("Calendar Id is required.", nameof(calendarId));
            }
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ArgumentNullException("Event Id is required.", nameof(eventId));
            }

            var insertEventRequest = _services.Events.Delete(calendarId, eventId);
            await insertEventRequest.ExecuteAsync(cancellationToken);
        }

        public async Task<CalendarListEntry> GetById(string calendarId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(calendarId))
            {
                throw new ArgumentNullException("Calendar Id is required.", nameof(calendarId));
            }

            var getCalendarRequest = _services.CalendarList.Get(calendarId);
            var calendarResult = await getCalendarRequest.ExecuteAsync(cancellationToken);

            return calendarResult;
        }

        public async Task<CalendarListEntry> GetByOwnerId(string calendarId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(calendarId))
            {
                throw new ArgumentNullException("Calendar Id is required.", nameof(calendarId));
            }



            var getCalendarRequest = _services.CalendarList.Get(calendarId);
            var calendarResult = await getCalendarRequest.ExecuteAsync(cancellationToken);

            return calendarResult;
        }

        public async Task<CalendarList> GetListAsync(CancellationToken cancellationToken = default)
        {


            var getCalendarRequest = _services.CalendarList.List();
            var calendarResult = await getCalendarRequest.ExecuteAsync(cancellationToken);

            return calendarResult;
        }

        public async Task<CalendarListEntry> UpdateAsync(string calendarId, CalendarListEntry options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException("Calendar options is required.", nameof(options));
            }



            var insertCalendarRequest = _services.CalendarList.Update(options, calendarId);
            var calendarResult = await insertCalendarRequest.ExecuteAsync(cancellationToken);

            return calendarResult;
            throw new NotImplementedException();
        }

        public async Task<Event> UpdateEventAsync(string calendarId, string eventId, Event options, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(calendarId))
            {
                throw new ArgumentNullException("Calendar Id is required.", nameof(calendarId));
            }
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ArgumentNullException("Event Id is required.", nameof(eventId));
            }



            var eventRequest = _services.Events.Update(options, calendarId, eventId);
            var eventResult = await eventRequest.ExecuteAsync(cancellationToken);

            return eventResult;
        }
    }
}
