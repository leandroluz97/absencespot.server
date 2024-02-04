namespace Absencespot.Dtos
{
    public class ResponsePaymentMethod
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public bool IsExpired { get; set; }
        public string Number { get; set; }
    }
}
