namespace Absencespot.Dtos
{
    public class ResponseSubscription
    {
        public string Id { get; set; }
        public string ClientSecret { get; set; }
        public IEnumerable<string> Ids { get; set; }

        
    }
}
