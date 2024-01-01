using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Services.Mappers
{
    public class SettingsMapper
    {
        public static Domain.Settings ToDomain(Dtos.Settings dto)
        {
            return new Domain.Settings() 
            { 
                DateFormat = dto.DateFormat,
                DisplayBirthday = dto.DisplayBirthday,
                DisplayMode = dto.DisplayMode,
                IsLoginFromEmailLinkRequired = dto.IsLoginFromEmailLinkRequired,
                OnBehalfOf = dto.OnBehalfOf,
                PrivacyMode = dto.PrivacyMode,
                TimeFormat = dto.TimeFormat,
                TimeZone = dto.TimeZone,
                FirstDayOfTheWeek = dto.FirstDayOfTheWeek,
            };
        }
        public static Dtos.Settings ToDto(Domain.Settings domain)
        {
            return new Dtos.Settings() 
            {
                DateFormat = domain.DateFormat,
                DisplayBirthday = domain.DisplayBirthday,
                DisplayMode = domain.DisplayMode,
                IsLoginFromEmailLinkRequired = domain.IsLoginFromEmailLinkRequired,
                OnBehalfOf = domain.OnBehalfOf,
                PrivacyMode = domain.PrivacyMode,
                TimeFormat = domain.TimeFormat,
                TimeZone = domain.TimeZone,
                FirstDayOfTheWeek = domain.FirstDayOfTheWeek,
            };
        }
    }
}
