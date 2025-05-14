namespace ProbabilityTrades.Domain.Services.ApiServices;

public class AzureApiService : IAzureApiService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureApiService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
    }

    public async Task<string> UploadBlobFile(string containerName, string fileName, MemoryStream fileStream, bool overwrite)
    {
        fileStream.Position = 0;
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        var blobContentInfo = await blobClient.UploadAsync(fileStream, overwrite);
        if (blobContentInfo is null)
            throw new NullReferenceException("Azure blob client upload returned null.");

        if (!blobContentInfo.GetRawResponse().Status.Equals(201))
            throw new InvalidOperationException($"Azure blob client upload failed with status {blobContentInfo.GetRawResponse().Status}");

        return blobClient.Uri.AbsoluteUri;
    }

    public async Task<bool> DeleteBlobFile(string containerName, string fileName)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return await blobContainerClient.GetBlobClient(fileName).DeleteIfExistsAsync();
    }
}