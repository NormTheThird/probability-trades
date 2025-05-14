namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class BlogService : BaseApplicationService, IBlogService
{
    public BlogService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<List<BlogModel>> GetBlogsAsync()
    {
        var blogs = await _db.Blogs.AsNoTracking()
                                   .Include(_ => _.BlogImages)
                                   .OrderByDescending(_ => _.DateCreated)
                                   .Select(_ => new BlogModel
                                   {
                                       Id = _.Id,
                                       CreatedByUserId = _.CreatedByUserId,
                                       MainBlogImageId = _.BlogImages.FirstOrDefault(_ => _.IsMainImage).Id,
                                       ShortDescription = _.ShortDescription,
                                       Name = _.Name,
                                       MainImageUrl = _.BlogImages.FirstOrDefault(_ => _.IsMainImage).Url ?? string.Empty,
                                       Title = _.Title,
                                       Body = _.Body,
                                       IsPosted = _.IsPosted,
                                       PostedDate = _.PostedDate,
                                       DateCreated = _.DateCreated,
                                   })
                                   .ToListAsync();
        return blogs;
    }

    public async Task<List<PostedBlogBaseModel>> GetPostedBlogsAsync()
    {
        var postedBlogs = await _db.Blogs.AsNoTracking()
                                         .Include(_ => _.CreatedByUser)
                                         .Where(_ => _.IsPosted)
                                         .OrderByDescending(_ => _.DateCreated)
                                         .Select(_ => new PostedBlogBaseModel
                                         {
                                             Id = _.Id,
                                             MainImageUrl = _.BlogImages.FirstOrDefault(_ => _.IsMainImage).Url ?? string.Empty,
                                             Title = _.Title,
                                             CreatedBy = _.CreatedByUser.FirstName + " " + _.CreatedByUser.LastName,
                                             ShortDescription = _.ShortDescription
                                         })
                                         .ToListAsync();
        return postedBlogs;
    }

    public async Task<PostedBlogModel> GetPostedBlogAsync(Guid blogId)
    {
        var postedBlog = await _db.Blogs.AsNoTracking()
                                        .Include(_ => _.CreatedByUser)
                                        .Include(_ => _.BlogImages)
                                        .FirstOrDefaultAsync(_ => _.Id.Equals(blogId));
        if (postedBlog is null)
            throw new KeyNotFoundException($"Unable to get blog for id {blogId}");

        return new PostedBlogModel
        {
            Id = postedBlog.Id,
            MainImageUrl = postedBlog.BlogImages.FirstOrDefault(_ => _.IsMainImage)?.Url ?? string.Empty,
            Title = postedBlog.Title,
            CreatedBy = $"{postedBlog.CreatedByUser.FirstName} {postedBlog.CreatedByUser.LastName}",
            ShortDescription = postedBlog.ShortDescription,
            Body = postedBlog.Body,
        };
    }

    public async Task<Guid> CreateBlogAsync(Guid userId, string name)
    {
        var blog = await _db.Blogs.FirstOrDefaultAsync(_ => _.Name.Equals(name));
        if (blog is not null)
            throw new DuplicateNameException($"{name} already exists in blogs");

        blog = new Blog
        {
            Id = Guid.NewGuid(),
            CreatedByUserId = userId,
            Name = name,
            Title = "",
            ShortDescription = "",
            Body = "",
            IsPosted = false,
            PostedDate = null,
            DateCreated = DateTime.Now.InCst()
        };
        _db.Blogs.Add(blog);
        await _db.SaveChangesAsync();

        return blog.Id;
    }

    public async Task UpdateBlogAsync(BlogModel blogModel)
    {
        var blog = await _db.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(blogModel.Id));
        if (blog is null)
            throw new KeyNotFoundException($"Unable to get blog for id {blogModel.Id}");

        blog.Title = blogModel.Title;
        blog.ShortDescription = blogModel.ShortDescription;
        blog.Body = blogModel.Body;
        if (blog.IsPosted != blogModel.IsPosted)
        {
            blog.IsPosted = blogModel.IsPosted;
            blog.PostedDate = blog.IsPosted ? DateTime.Now.InCst() : null;
        }

        await _db.SaveChangesAsync();
    }

    public async Task DeleteBlogAsync(Guid blogId)
    {
        var blog = await _db.Blogs.FirstOrDefaultAsync(_ => _.Id.Equals(blogId));
        if (blog is not null)
            _db.Blogs.Remove(blog);
        await _db.SaveChangesAsync();
    }
}