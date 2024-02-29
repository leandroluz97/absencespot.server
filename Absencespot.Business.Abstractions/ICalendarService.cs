using Absencespot.Dtos;
using Google.Apis.Calendar.v3.Data;

namespace Absencespot.Business.Abstractions
{
    public interface ICalendarService
    {
        Task<CalendarFeed> GetUrlsAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default);
        Task<CalendarListEntry> GetContent(string id, CancellationToken cancellationToken = default);
        Task<Events> GetEventsContent(string id, CancellationToken cancellationToken = default);
    }
}
