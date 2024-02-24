using Google.Apis.Calendar.v3.Data;

namespace Absencespot.Infrastructure.Abstractions.Clients.Calendar
{
    public interface ICalendarClient
    {
        Task<Google.Apis.Calendar.v3.Data.Calendar> CreateAsync(Google.Apis.Calendar.v3.Data.Calendar options, CancellationToken cancellationToken = default);
        Task<CalendarListEntry> GetById(string calendarId, CancellationToken cancellationToken = default);
        Task<CalendarListEntry> GetByOwnerId(string calendarId, CancellationToken cancellationToken = default);
        Task<CalendarList> GetListAsync(CancellationToken cancellationToken = default);
        Task<CalendarListEntry> UpdateAsync(string calendarId, CalendarListEntry options, CancellationToken cancellationToken = default);
        Task DeleteAsync(string calendarId, CancellationToken cancellationToken = default);
        Task<Event> CreateEventAsync(string calendarId, Event options, CancellationToken cancellationToken = default);
        Task<Event> UpdateEventAsync(string calendarId, string eventId, Event options, CancellationToken cancellationToken = default);
        Task DeleteEventAsync(string calendarId, string eventId, CancellationToken cancellationToken = default);
    }
}
