using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain.Enums
{
    public class ProviderType : Enumeration
    {
        public static readonly ProviderType GOOGLE = new ProviderType(1, "GOOGLE");
        public static readonly ProviderType MICROSOFT = new ProviderType(2, "MICROSOFT");

        public ProviderType(){}
        public ProviderType(int id, string name) : base(id, name)
        {

        }
    }
}
