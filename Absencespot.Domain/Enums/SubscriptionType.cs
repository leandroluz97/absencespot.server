using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain.Enums
{
    public class SubscriptionType : Enumeration
    {
        public static readonly SubscriptionType FREE = new SubscriptionType(1, "FREE");
        public static readonly SubscriptionType BUSINESS = new SubscriptionType(2, "BUSINESS");
        public static readonly SubscriptionType ENTERPRISE = new SubscriptionType(3, "ENTERPRISE");

        public SubscriptionType(int id, string name) : base(id, name)
        {

        }
    }
}
