using Absencespot.Domain.Enums;
using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Settings : Entity
    {
        public DisplayType DisplayMode { get; set; }
        public bool PrivacyMode { get; set; }
        public bool OnBehalfOf { get; set; }
        public bool IsLoginFromEmailLinkRequired { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string TimeZone { get; set; }
        public string FirstDayOfTheWeek { get; set; }
        public string DisplayBirthday { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
