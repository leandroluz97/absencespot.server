using Absencespot.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class TrackRecord
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime BreakStart { get; set; }
        public DateTime BreakEnd { get; set; }
        public LocationType Location { get; set; }
        public string? Note { get; set; }


        public void EnsureValidation()
        {
            if (CheckOut <= CheckIn)
            {
                throw new ArgumentException(nameof(CheckOut));
            }
            if (BreakEnd <= BreakStart)
            {
                throw new ArgumentException(nameof(BreakEnd));
            }
            if (BreakStart <= CheckIn || BreakStart >= CheckOut)
            {
                throw new ArgumentException(nameof(BreakStart));
            }
            if (BreakEnd <= CheckIn || BreakEnd >= CheckOut)
            {
                throw new ArgumentException(nameof(BreakEnd));
            }
        }
    }
}
