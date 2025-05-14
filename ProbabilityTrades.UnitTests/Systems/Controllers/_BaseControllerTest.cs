namespace ProbabilityTrades.UnitTests.Systems.Controllers;

public class BaseControllerTest<T>
{
    public Mock<IConfiguration> MockConfiguration { get; set; } = new Mock<IConfiguration>();
    public Mock<ILogger<T>> MockLogger { get; set; } = new Mock<ILogger<T>>();

    public BaseControllerTest()
    {
 
    }

    public static string GetRandomString() => "abcdefghijklmnopqrstuvwxyz";
}