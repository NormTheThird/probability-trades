namespace ProbabilityTrades.Common.Models;

public class NewBlogModel
{
    public string Name { get; set; } = string.Empty;
}

public class BlogModel : NewBlogModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid CreatedByUserId { get; set; } = Guid.Empty;
    public Guid? MainBlogImageId { get; set; } = null;
    public string MainImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsPosted { get; set; } = false;
    public DateTimeOffset? PostedDate { get; set; } = new();
    public DateTimeOffset DateCreated { get; set; } = new();
}

public class PostedBlogBaseModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string MainImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
}

public class PostedBlogModel : PostedBlogBaseModel
{
    public string Body { get; set; } = string.Empty;
}

public class NewBlogImageModel
{
    public Guid BlogImageId { get; set; } = Guid.Empty;
    public string BlogImageUrl { get; set; } = string.Empty;
}