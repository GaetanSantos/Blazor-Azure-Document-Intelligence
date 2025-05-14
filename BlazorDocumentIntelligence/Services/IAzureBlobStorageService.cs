namespace BlazorDocumentIntelligence.Services
{
    public interface IAzureBlobStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName);
    }
}
