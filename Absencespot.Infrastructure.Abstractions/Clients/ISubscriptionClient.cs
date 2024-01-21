using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Clients
{
    public interface ISubscriptionClient
    {
        public void CreateSubscription(int seat);
    }
}
