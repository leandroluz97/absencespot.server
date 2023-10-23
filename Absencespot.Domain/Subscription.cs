using Absencespot.Domain.Enums;
using Absencespot.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Domain
{
    public class Subscription : Entity
    {
        public SubscriptionType Type { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
