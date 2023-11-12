using Azure.Storage.Blobs;

namespace Absencespot.Azure
{
    public class BlobStorageClient
    {
        private readonly BlobServiceClient _blobStorageClient;
        public BlobStorageClient(BlobServiceClient blobServiceClient)
        {
            _blobStorageClient = blobServiceClient;
        }

        public async Task<string> GetByIdAsync(string containerName, string blobId)
        {
            BlobContainerClient containerClient = _blobStorageClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobId);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadAsync(string containerName, string blobId, Stream file)
        {
            BlobContainerClient containerClient = _blobStorageClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(blobId);

            var blobInfo = await blobClient.UploadAsync(file);

            if (blobInfo is null) return false;

            return true;
        }

        public async Task DeleteAsync(string containerName, string blobId)
        {
            BlobContainerClient containerClient = _blobStorageClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(blobId);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}