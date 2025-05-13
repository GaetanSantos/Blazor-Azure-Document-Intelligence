namespace BlazorDocumentIntelligence.Models
{
    public class CurrencyAmount
    {
        public string? CurrencySymbol { get; set; }
        public double Amount { get; set; }
        public float? Confidence { get; set; }
    }
}
