using Absencespot.Domain.Enums;
using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class TrackRecord : Entity
    {
        //public User Owner { get; set; }
        //public User OnBehalfOf { get; set; }
        public DateTime Date { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime BreakStart { get; set; }
        public DateTime BreakEnd{ get; set; }
        public LocationType Location { get; set; }
        public string Note{ get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
