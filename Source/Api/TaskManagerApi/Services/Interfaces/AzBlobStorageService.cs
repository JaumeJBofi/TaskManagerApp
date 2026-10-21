using Azure.Identity;
using Azure.Storage.Blobs;

namespace TaskManagerApi.Services.Interfaces
{
    public class AzBlobStorageService
    {
        private BlobServiceClient _blobServiceClient;
        public AzBlobStorageService(string storageAccountName) 
        {
            _blobServiceClient = new BlobServiceClient(new Uri($"https://<{storageAccountName}.blob.core.windows.net>"), new DefaultAzureCredential())
        }
    }
}
