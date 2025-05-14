namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IBlogImageService
{
    Task<NewBlogImageModel> CreateBlogImageAsync(Guid blogId, string imageName, MemoryStream imageStream, bool isMainImage);
    Task DeleteBlogImageAsync(Guid blogImageId);
}