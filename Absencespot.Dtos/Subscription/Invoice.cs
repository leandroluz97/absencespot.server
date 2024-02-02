using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class Invoice
    {
        public string Status { get; set; } = null!;
        public string Number { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public string TaxRates { get; set; } = null!;
        public DateTime? DueDate { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; } 
        public string PaymentMethodBrand { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal? SubtotalExcludingTax { get; set; }
        public decimal? Tax { get; set; }
        public decimal Total { get; set; }
        public decimal? TotalExcludingTax { get; set; }
        public decimal? TotalTaxAmounts { get; set; }
    }
}
