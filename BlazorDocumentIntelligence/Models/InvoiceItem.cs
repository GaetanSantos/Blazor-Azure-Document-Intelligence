namespace BlazorDocumentIntelligence.Models
{
    public class InvoiceItem
    {
        public string? Description { get; set; }
        public CurrencyAmount? Amount { get; set; }
        public float? Confidence { get; set; }
        public double? Quantity { get; set; }
        public float? QuantityConfidence { get; set; }
    }
}
