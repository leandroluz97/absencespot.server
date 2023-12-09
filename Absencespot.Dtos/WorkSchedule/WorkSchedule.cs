using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class WorkSchedule
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }

        //default
        public IEnumerable<string>? WorkDays { get; set; }
        public DateTime? StartHour { get; set; }
        public DateTime? EndHour { get; set; }

        //flexible
        public int? TotalWorkDays { get; set; }
        public int? Hours { get; set; }

        virtual public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(nameof(Name));
            }

            if (IsDefault)
            {
                if (WorkDays == null || !WorkDays.Any())
                {
                    throw new ArgumentException(nameof(WorkDays));
                }
                if (!StartHour.HasValue)
                {
                    throw new ArgumentException(nameof(StartHour));
                }
                if (!EndHour.HasValue)
                {
                    throw new ArgumentException(nameof(EndHour));
                }
            }
            else
            {
                if (!TotalWorkDays.HasValue)
                {
                    throw new ArgumentException(nameof(TotalWorkDays));
                }
                if (!Hours.HasValue)
                {
                    throw new ArgumentException(nameof(TotalWorkDays));
                }
            }
        }
    }
}
