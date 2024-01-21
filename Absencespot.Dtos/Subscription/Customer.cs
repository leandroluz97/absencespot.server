namespace Absencespot.Dtos
{
    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(nameof(Name));
            }
            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentException(nameof(Email));
            }
        }
    }
}
