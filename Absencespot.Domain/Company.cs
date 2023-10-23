using Absencespot.Domain.Seedwork;

namespace Absencespot.Domain
{
    public class Company : Entity
    {
        public string Name { get; set; }
        public string FiscalNumber { get; set; }

        public int SubcriptionId { get; set; }
        public Subscription Subcription { get; set; }
    }
}