using Storygame.Contracts.WebApi;
using Storygame.Contracts.WebApi.Requests;
using System.Net;

namespace Storygame.Tests.Integration.Users;

[TestFixture]
public class UsersTests : IntegrationTestBase
{
    [Test]
    public async Task GetCSRF_ReturnsToken()
    {
        var response = await Client.HttpClient.GetAsync("/api/users/CSRF");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var token = await response.Content.ReadAsStringAsync();
        token.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task Register_SendsVerificationEmail()
    {
        await Client.Register(new RegisterRequest("Alice", "alice@example.com"));

        var mail = await Client.Mail("alice@example.com");
        mail.ShouldHaveSingleItem();
        mail[0].Message.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task Register_WithDuplicateEmail_Fails()
    {
        await Client.Register(new RegisterRequest("Alice", "alice@example.com"));

        await Should.ThrowAsync<HttpRequestException>(() =>
            Client.Register(new RegisterRequest("Alice2", "alice@example.com")));
    }

    [Test]
    public async Task Verify_WithValidCode_Succeeds()
    {
        await Client.Register(new RegisterRequest("Bob", "bob@example.com"));
        var verificationCode = (await Client.Mail("bob@example.com")).Single(m => m.Subject == "Verification code").Message;

        await Should.NotThrowAsync(() =>
            Client.Verify(new VerifyUserRequest("bob@example.com", verificationCode)));
    }

    [Test]
    public async Task Login_SendsConfirmationEmail()
    {
        await Client.Register(new RegisterRequest("Carol", "carol@example.com"));
        var verificationCode = (await Client.Mail("carol@example.com")).Single(m => m.Subject == "Verification code").Message;
        await Client.Verify(new VerifyUserRequest("carol@example.com", verificationCode));

        await Client.Login(new LoginRequest("carol@example.com"));

        var allMail = await Client.Mail("carol@example.com");
        allMail.Length.ShouldBe(2);
    }

    [Test]
    public async Task ConfirmLogin_AuthenticatesUser()
    {
        await AuthenticateAs(Client, "Dave", "dave@example.com");

        var me = await Client.Me();

        me.Name.ShouldBe("Dave");
    }

    [Test]
    public async Task GetMe_WhenUnauthenticated_Returns401()
    {
        var response = await Client.HttpClient.GetAsync("/api/users/Me");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Logout_DeauthenticatesUser()
    {
        await AuthenticateAs(Client, "Eve", "eve@example.com");

        await Client.Logout();

        var response = await Client.HttpClient.GetAsync("/api/users/Me");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
