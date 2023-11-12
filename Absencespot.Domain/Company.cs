using Absencespot.Domain.Enums;
using Absencespot.Domain.Seedwork;

namespace Absencespot.Domain
{
    public class Company : Entity
    {
        public string Name { get; set; }
        public string? FiscalNumber { get; set; }  
        public string? EmailContact { get; set; }
        public string Industry { get; set; }
        public bool IsActive { get; set; }
        public Settings Settings { get; set; }

        public int SubcriptionId { get; set; }
        public Subscription Subcription { get; set; }
        public ICollection<Integration> Integrations { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Leave> Leaves { get; set; }
        public ICollection<Office> Offices { get; set; }
        public ICollection<Team> Teams { get; set; }
        public ICollection<WorkSchedule> WorkSchedules { get; set; }
       // public ICollection<Request> Requests { get; set; }
    }
}