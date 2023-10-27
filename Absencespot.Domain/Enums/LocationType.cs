using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain.Enums
{
    public class LocationType : Enumeration
    {
        public static readonly LocationType ONSITE = new LocationType(1, "ON_SITE");
        public static readonly LocationType REMOTE = new LocationType(2, "REMOTE");
        public LocationType(){}
        public LocationType(int id, string name) : base(id, name)
        {

        }
    }
}
