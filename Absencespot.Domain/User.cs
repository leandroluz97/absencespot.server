using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class User : IdentityUser<int>
    {
        public Guid GlobalId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? BirthDay { get; set; }
        public ICollection<TrackRecord> TrackRecords { get; set; }
        public ICollection<UserTeam>? Teams { get; set; }
        public ICollection<Request> Requests { get; set; }
        //public ICollection<Request> OnBehalfOs { get; set; }
        public ICollection<Request>? OnBehalfOfs { get; set; }
        public ICollection<Request>? Approved { get; set; }
        public ICollection<AvailableLeave>? AvailableLeaves { get; set; }

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        public int? OfficeId { get; set; }
        public Office? Office { get; set; }
        public int? WorkScheduleId { get; set; }
        public WorkSchedule? WorkSchedule { get; set; }
    }
}
