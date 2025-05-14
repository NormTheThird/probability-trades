namespace ProbabilityTrades.Common.RequestAndResponses;

public class RegisterRequest : BaseRequest
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterResponse : BaseResponse
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ServerTimeNow { get; private set; } = DateTime.Now;
    public DateTime ServerTimeUtcNow { get; private set; } = DateTime.UtcNow;
}

public class AuthenticateRequest : BaseRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthenticateResponse : BaseResponse
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ServerTimeNow { get; private set; } = DateTime.Now;
    public DateTime ServerTimeUtcNow { get; private set; } = DateTime.UtcNow;
}

public class RefreshAuthenticationRequest : BaseRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshAuthenticationResponse : BaseResponse
{
    public Guid UserId { get; set; } = Guid.Empty;
    public bool IsAdmin { get; set; } = false;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ServerTimeNow { get; private set; } = DateTime.Now;
    public DateTime ServerTimeUtcNow { get; private set; } = DateTime.UtcNow;
}