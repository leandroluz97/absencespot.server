using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class UpgradeSubscription
    {
        public string SubscriptionId { get; set; }
        public string SubscriptionItemId { get; set; }
        public string PriceId { get; set; }
        public int Quantity { get; set; }

        public void EnsureValidation()
        {
            if (string.IsNullOrWhiteSpace(SubscriptionId))
            {
                throw new ArgumentException(nameof(SubscriptionId));
            }
        }
    }
}
