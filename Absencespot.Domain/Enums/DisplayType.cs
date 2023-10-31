using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain.Enums
{
    public class DisplayType : Enumeration
    {
        public static readonly DisplayType OFFICE = new DisplayType(1, "OFFICE");
        public static readonly DisplayType TEAM = new DisplayType(2, "TEAM");
        public DisplayType():base(){}
        public DisplayType(int id, string name) : base(id, name)
        {

        }

    }
}
