namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class BlogImageService : BaseApplicationService, IBlogImageService
{
    private readonly IAzureApiService _azureApiService;
    private readonly string AzureBlogImageContainer = "blog-images";

    public BlogImageService(IConfiguration configuration, ApplicationDbContext db, IAzureApiService azureApiService) 
        : base(configuration, db)
    {
        _azureApiService = azureApiService ?? throw new ArgumentNullException(nameof(azureApiService));
    }

    public async Task<NewBlogImageModel> CreateBlogImageAsync(Guid blogId, string imageName, MemoryStream imageStream, bool isMainImage)
    {
        var currentMaxOrder = _db.BlogImages.AsNoTracking()
                                            .Where(_ => _.BlogId.Equals(blogId))
                                            .OrderByDescending(_ => _.ImageOrder)
                                            .FirstOrDefault()?.ImageOrder ?? 0;
        var nowInCst = DateTime.Now.InCst();
        var blogImageId = Guid.NewGuid();
        var storedImageName = $"{blogImageId}{GetExtensionFromName(imageName)}";
        var url = await _azureApiService.UploadBlobFile(AzureBlogImageContainer, storedImageName, imageStream, false);
        var blogImage = new BlogImage
        {
            Id = blogImageId,
            BlogId = blogId,
            Name = imageName,
            Url = url,
            IsMainImage = isMainImage,
            ImageOrder = currentMaxOrder + 1,
            DateCreated = nowInCst
        };
        _db.BlogImages.Add(blogImage);

        await _db.SaveChangesAsync();

        return new NewBlogImageModel { BlogImageId = blogImageId, BlogImageUrl = url };
    }

    public async Task DeleteBlogImageAsync(Guid blogImageId)
    {
        var blogImage = await _db.BlogImages.FirstOrDefaultAsync(_ => _.Id.Equals(blogImageId));
        if (blogImage is null)
            throw new KeyNotFoundException($"Unable to get blog image for id {blogImageId}");

        var storedImageName = $"{blogImageId}{GetExtensionFromName(blogImage.Name)}";
        var blobDeleted = await _azureApiService.DeleteBlobFile(AzureBlogImageContainer, storedImageName);

        if (!blobDeleted)
            throw new Exception("Azure was unable to delete the file.");

        _db.BlogImages.Remove(blogImage);

        await _db.SaveChangesAsync();
    }



    private static string GetExtensionFromName(string name) => name[name.IndexOf(".")..];
}