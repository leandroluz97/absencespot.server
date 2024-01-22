using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Price
    {
        public string Id { get; set; }
        public decimal UnitAmount { get; set; }
        public string ProductId { get; set; }
        public string Currency { get; set; }
    }
}
