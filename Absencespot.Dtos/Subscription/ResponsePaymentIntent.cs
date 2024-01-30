using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Dtos
{
    public class ResponsePaymentIntent
    {
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public List<string>? PaymentMethod { get; set; }
        public decimal? TotalExcludingTax { get; set; }
        public decimal? Total { get; set; }
        public decimal? TotalTaxAmounts { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal? Tax { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
