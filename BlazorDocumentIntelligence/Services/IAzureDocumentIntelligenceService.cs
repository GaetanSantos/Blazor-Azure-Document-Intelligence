using BlazorDocumentIntelligence.Models;

namespace BlazorDocumentIntelligence.Services
{
    public interface IAzureDocumentIntelligenceService
    {
        Task<InvoiceAnalysis> AnalyzeAsync(string fileUrl);
    }
}
