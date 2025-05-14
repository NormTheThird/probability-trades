namespace ProbabilityTrades.UnitTests.Fixtures;

public static class UserFixture
{
    public static UserAuthenticationModel GetAuthenticatedUser() 
    {
        return new UserAuthenticationModel
        {
            Id = Guid.NewGuid(),
            Username = "UserName",
            Email = "test@email.com",
            Roles = new List<string> { "User" },
        };
    }
}