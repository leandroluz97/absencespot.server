using Absencespot.Domain.Enums;
using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Request : Entity
    {
        public User? OnBehalfOf { get; set; }
        public User Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public StatusType Status { get; set; }
        public string File { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
