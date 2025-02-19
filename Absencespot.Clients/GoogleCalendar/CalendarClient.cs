﻿using Absencespot.Clients.GoogleCalendar.Model;
using Absencespot.Clients.GoogleCalendar.Options;
using Absencespot.Infrastructure.Abstractions.Clients.Calendar;
using Absencespot.Utils.Constants;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static Google.Apis.Calendar.v3.Data.AclRule;

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
            string[] scopes = new string[]
            {
                CalendarService.Scope.Calendar,
                CalendarService.Scope.CalendarEvents,
                "https://www.google.com/calendar/feeds"
            };
            var credential = GoogleCredential.FromFile(_options.KeyFilePath)
                .CreateScoped(scopes);

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

            var acl = new AclRule()
            {
                Role = "reader",
                Scope = new ScopeData()
                {
                    Type = "default"
                }
            };
            var aclResult = _services.Acl.Insert(acl, calendarResult.Id);
            await aclResult.ExecuteAsync(cancellationToken);

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

        public async Task<Calendar> GetByOwnerId(string calendarId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(calendarId))
            {
                throw new ArgumentNullException("Calendar Id is required.", nameof(calendarId));
            }

            var getCalendarRequest = _services.Calendars.Get(calendarId);
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

        public async Task<Events?> GetHolidays(string country, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                throw new ArgumentNullException($"Country is required.", nameof(country));
            }
            if (!Country.holidayEntries.Any(c => c == country))
            {
                throw new ArgumentException($"Country '{country}' is not valid.", nameof(country));
            }

            var baseURL = $"https://www.googleapis.com/calendar/v3/calendars/{country}%23holiday%40group.v.calendar.google.com/events";
            var result = await _services.HttpClient.GetAsync(baseURL);

            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();

            var eventResult = JsonSerializer.Deserialize<Events>(content);

            return eventResult;
        }

        public async Task<Events> GetEventListAsync(string calendarId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(calendarId))
            {
                throw new ArgumentNullException("Calendar Id is required.", nameof(calendarId));
            }

            var eventRequest = _services.Events.List(calendarId);
            var eventResult = await eventRequest.ExecuteAsync(cancellationToken);

            return eventResult;
        }
    }
}
