using Absencespot.Domain.Seedwork;

namespace Absencespot.Domain
{
    public class Address : Entity
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3   { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }

        public int? OfficeId { get; set; }
        public Office? Office { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }

    }
}
