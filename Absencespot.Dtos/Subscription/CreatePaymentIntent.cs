using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class CreatePaymentIntent
    {
        public string CustomerId { get; set; }
        public string PriceId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public void EnsureValidation()
        {

        }
    }
}
