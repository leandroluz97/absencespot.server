namespace Absencespot.Dtos
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? FiscalNumber { get; set; }
        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentNullException(nameof(Name));
            }
        }
    }
}