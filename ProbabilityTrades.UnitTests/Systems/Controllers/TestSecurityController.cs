using ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

namespace ProbabilityTrades.UnitTests.Systems.Controllers;

[TestClass]
public class TestSecurityController : BaseControllerTest<SecurityController>
{

    public Mock<ISecurityService> MockSecurityService { get; set; } = new Mock<ISecurityService>();
    public Mock<IMailService> MockMailService { get; set; } = new Mock<IMailService>();
    public Mock<IUserPasswordResetService> MockUserPasswordResetService { get; set; } = new Mock<IUserPasswordResetService>();

    public TestSecurityController() : base()
    {

    }

    [TestMethod]
    public async Task Should_AuthenticatesUser()
    {
        // Arrange
        var randomString = GetRandomString();
        var authenticateRequest = new AuthenticateRequest { Username = "UserName", Password = "Password" };
        var userAuthenticationModel = UserFixture.GetAuthenticatedUser();

        MockSecurityService.Setup(_ => _.AuthenticateUserAsync(authenticateRequest.Username, authenticateRequest.Password)).ReturnsAsync(userAuthenticationModel);
        MockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "Jwt:Key")]).Returns("BcHNP4dpxtUYw9Sucx03YJWSfFuIpBILtktXkW4Z");

        var sut = new SecurityController(MockConfiguration.Object, MockLogger.Object, MockMailService.Object, MockSecurityService.Object, MockUserPasswordResetService.Object);

        // Act
        var result = (OkObjectResult)await sut.Authenticate(authenticateRequest);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().NotBeNull();
        var authenticateResponse = result.Value as AuthenticateResponse ?? new();
        authenticateResponse.AccessToken.Should().NotBeNullOrEmpty();
        authenticateResponse.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task Should_Not_AuthenticatesUser()
    {
        // Arrange
        var randomString = GetRandomString();
        var authenticateRequest = new AuthenticateRequest { Username = "UserName", Password = "Password" };

        MockSecurityService.Setup(_ => _.AuthenticateUserAsync(authenticateRequest.Username, authenticateRequest.Password)).ReturnsAsync((UserAuthenticationModel?)null);

        var sut = new SecurityController(MockConfiguration.Object, MockLogger.Object, MockMailService.Object, MockSecurityService.Object, MockUserPasswordResetService.Object);

        // Act
        var result = (UnauthorizedObjectResult)await sut.Authenticate(authenticateRequest);

        // Assert
        result.StatusCode.Should().Be(401);
        result.Value.Should().NotBeNull();
        var authenticateResponse = result.Value as AuthenticateResponse ?? new();
        authenticateResponse.AccessToken.Should().BeNullOrEmpty();
        authenticateResponse.RefreshToken.Should().BeNullOrEmpty();
    }

    [TestMethod]
    public async Task Should_RefreshAuthentication()
    {
        // Arrange
        var randomString = GetRandomString();
        var authenticateRequest = new AuthenticateRequest { Username = "UserName", Password = "Password" };
        var refreshAuthenticationRequest = new RefreshAuthenticationRequest { RefreshToken = randomString };
        var userAuthenticationModel = UserFixture.GetAuthenticatedUser();

        MockSecurityService.Setup(_ => _.AuthenticateUserAsync(authenticateRequest.Username, authenticateRequest.Password)).ReturnsAsync((UserAuthenticationModel?)null);
        MockSecurityService.Setup(_ => _.RefreshUserAuthenticationAsync(It.IsAny<string>())).ReturnsAsync(userAuthenticationModel);
        MockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "Jwt:Key")]).Returns("BcHNP4dpxtUYw9Sucx03YJWSfFuIpBILtktXkW4Z");

        var sut = new SecurityController(MockConfiguration.Object, MockLogger.Object, MockMailService.Object, MockSecurityService.Object, MockUserPasswordResetService.Object);

        // Act
        var result = (OkObjectResult)await sut.RefreshAuthentication(refreshAuthenticationRequest);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().NotBeNull();
        var authenticateResponse = result.Value as AuthenticateResponse ?? new();
        authenticateResponse.UserId.Should().Be(userAuthenticationModel.Id);
        authenticateResponse.AccessToken.Should().NotBeNullOrEmpty();
        authenticateResponse.RefreshToken.Should().NotBeNullOrEmpty();
    }
}