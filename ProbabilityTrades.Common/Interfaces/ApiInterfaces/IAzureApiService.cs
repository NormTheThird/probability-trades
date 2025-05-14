namespace ProbabilityTrades.Common.Interfaces.ApiInterfaces;

public interface IAzureApiService
{
    Task<string> UploadBlobFile(string containerName, string fileName, MemoryStream fileStream, bool overwrite);
    Task<bool> DeleteBlobFile(string containerName, string fileName);
}