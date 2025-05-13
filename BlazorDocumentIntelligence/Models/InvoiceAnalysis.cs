using System.Collections.Generic;

namespace BlazorDocumentIntelligence.Models
{
    public class InvoiceAnalysis
    {
        public string? VendorName { get; set; }
        public string? CustomerName { get; set; }
        public List<InvoiceItem> Items { get; set; } = new();
        public CurrencyAmount? SubTotal { get; set; }
        public CurrencyAmount? TotalTax { get; set; }
        public CurrencyAmount? InvoiceTotal { get; set; }
    }
}
