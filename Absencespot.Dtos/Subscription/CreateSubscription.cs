namespace Absencespot.Dtos
{
    public class CreateSubscription
    {
        public string CustomerId { get; set; }
        public string PriceId { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(CustomerId))
            {
                throw new ArgumentException(nameof(CustomerId));
            }
            if (string.IsNullOrWhiteSpace(PriceId))
            {
                throw new ArgumentException(nameof(PriceId));
            }
        }
    }
}
