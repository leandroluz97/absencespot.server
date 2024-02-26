using Google.Apis.Calendar.v3.Data;

namespace Absencespot.Clients.GoogleCalendar.Model
{
    public class EventResult
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string TimeZone { get; set; }
        public IEnumerable<Event> Items { get; set; }
        
    }
}
