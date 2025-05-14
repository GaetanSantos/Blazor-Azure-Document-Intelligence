using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace BlazorDocumentIntelligence.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"]
                ?? throw new InvalidOperationException("Blob Storage connection string not configured");
            var containerName = configuration["AzureBlobStorage:Container"]
                ?? throw new InvalidOperationException("Blob Storage container not configured");

            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
            return blobClient.Uri.ToString();
        }
    }
}
