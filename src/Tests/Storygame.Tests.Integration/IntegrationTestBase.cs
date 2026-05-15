using Storygame.Client;
using Storygame.Contracts.WebApi;
using Storygame.Contracts.WebApi.Requests;
using System.Net;

namespace Storygame.Tests.Integration;

public abstract class IntegrationTestBase
{
    protected StorygameClient Client { get; private set; } = null!;

    [SetUp]
    public void CreateClient()
    {
        Client = WebAppFactory.CreateClientWithDefaultMocks();
    }

    protected static async Task AuthenticateAs(StorygameClient client, string name, string email)
    {
        await client.Register(new RegisterRequest(name, email));
        var verificationCode = (await client.Mail(email)).Single(m => m.Subject == "Verification code").Message;
        await client.Verify(new VerifyUserRequest(email, verificationCode));
        await client.Login(new LoginRequest(email));
        var confirmationKey = (await client.Mail(email)).Single(m => m.Subject == "Login confirmation").Message;
        await client.ConfirmLogin(new ConfirmLoginRequest(confirmationKey));
    }

    protected static async Task<HttpStatusCode> GetUnauthenticated(string path)
    {
        var client = WebAppFactory.CreateClientWithDefaultMocks();
        var response = await client.HttpClient.GetAsync(path);
        return response.StatusCode;
    }
}

public abstract class AuthenticatedIntegrationTestBase : IntegrationTestBase
{
    protected const string DefaultName = "Test User";
    protected const string DefaultEmail = "test@example.com";

    [SetUp]
    public Task Authenticate() => AuthenticateAs(Client, DefaultName, DefaultEmail);
}
