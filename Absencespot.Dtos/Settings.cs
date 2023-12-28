using Absencespot.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Settings
    {
        public DisplayType DisplayMode { get; set; }
        public bool PrivacyMode { get; set; }
        public bool OnBehalfOf { get; set; }
        public bool IsLoginFromEmailLinkRequired { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string TimeZone { get; set; }
        public string FirstDayOfTheWeek { get; set; }
        public bool DisplayBirthday { get; set; }

        public void EnsureValidation()
        {

        }
    }

}
