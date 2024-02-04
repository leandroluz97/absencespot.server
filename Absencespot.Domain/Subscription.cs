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
        public string SubscriptionId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
