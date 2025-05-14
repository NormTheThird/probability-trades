namespace ProbabilityTrades.API.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BlogsController : BaseController<BlogsController>
{
    private readonly IBlogService _blogService;
    private readonly IBlogImageService _blogImageService;

    public BlogsController(IConfiguration config, ILogger<BlogsController> logger, IBlogService blogService,
                           IBlogImageService blogImageService)
        : base(config, logger)
    {
        _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
        _blogImageService = blogImageService ?? throw new ArgumentNullException(nameof(blogImageService));
    }

    [HttpGet]
    public async Task<IActionResult> GetBlogs()
    {
        try
        {
            var response = new BaseDataResponse();
            var blogs = await _blogService.GetBlogsAsync();



            response.Data = blogs;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [AllowAnonymous]
    [HttpGet("posted")]
    public async Task<IActionResult> GetPostedBlogs()
    {
        try
        {
            var response = new BaseDataResponse();
            var blogs = await _blogService.GetPostedBlogsAsync();

            response.Data = blogs;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [AllowAnonymous]
    [HttpGet("{blogId}")]
    public async Task<IActionResult> GetBlog(Guid blogId)
    {
        try
        {
            var response = new BaseDataResponse();
            var postedBlog = await _blogService.GetPostedBlogAsync(blogId);

            response.Data = postedBlog;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog(NewBlogModel newBlogModel)
    {
        try
        {
            var response = new BaseDataResponse();
            var newBlogId = await _blogService.CreateBlogAsync(GetLoggedInUser().UserId, newBlogModel.Name);

            response.Data = newBlogId;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBlog(BlogModel blogModel)
    {
        try
        {
            var response = new BaseResponse();
            await _blogService.UpdateBlogAsync(blogModel);
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpDelete("{blogId}")]
    public async Task<IActionResult> DeleteBlog(Guid blogId)
    {
        try
        {
            var response = new BaseResponse();
            await _blogService.DeleteBlogAsync(blogId);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }


    #region Blog Images

    [HttpPost("{blogId}/images/{isMainImage}")]
    public async Task<IActionResult> CreateBlogImage(Guid blogId, IFormFile formFile, bool isMainImage)
    {
        try
        {
            var response = new BaseDataResponse();
            using (var fileStream = new MemoryStream())
            {
                await formFile.CopyToAsync(fileStream);
                var newBlogImage = await _blogImageService.CreateBlogImageAsync(blogId, formFile.FileName, fileStream, isMainImage);

                response.Data = newBlogImage;
            }

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpDelete("{blogId}/images/{blogImageId}")]
    public async Task<IActionResult> DeleteBlogImage(Guid blogId, Guid blogImageId)
    {
        try
        {
            var response = new BaseResponse();

            await _blogImageService.DeleteBlogImageAsync(blogImageId);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    #endregion
}