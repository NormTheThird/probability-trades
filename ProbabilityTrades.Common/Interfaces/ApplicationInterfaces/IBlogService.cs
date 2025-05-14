namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IBlogService
{
    Task<List<BlogModel>> GetBlogsAsync();
    Task<List<PostedBlogBaseModel>> GetPostedBlogsAsync();
    Task<PostedBlogModel> GetPostedBlogAsync(Guid blogId);
    Task<Guid> CreateBlogAsync(Guid userId, string name);
    Task UpdateBlogAsync(BlogModel blogModel);
    Task DeleteBlogAsync(Guid blogId);
}