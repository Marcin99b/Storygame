using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Storygame.Client;
using Storygame.Contracts.WebApi;
using Storygame.Contracts.WebApi.Requests;
using Storygame.Users;

namespace Storygame.Tests.Integration.Client;

[TestFixture]
public class StorygameClientTests
{
    private static StorygameClient CreateClient()
        => WebAppFactory.CreateStorygameClient(services =>
            services.AddSingleton<IUsersRepository>(new InMemoryUsersRepository()));

    [Test]
    public async Task Register_SetsXCsrfTokenInHeaders()
    {
        var client = CreateClient();
        var (name, email) = NewCredentials();

        await client.Register(new RegisterRequest(name, email));

        Assert.That(client.HttpClient.DefaultRequestHeaders.Contains("X-CSRF-TOKEN"), Is.True);
    }

    [Test]
    public async Task ConfirmLogin_SetsAuthCookieInHeaders()
    {
        var client = CreateClient();
        var (name, email) = NewCredentials();

        await RegisterAndVerify(client, name, email);
        await PerformLogin(client, email);

        Assert.That(client.HttpClient.DefaultRequestHeaders.TryGetValues(HeaderNames.Cookie, out var cookies), Is.True);
        Assert.That(cookies!.Single(), Does.Contain("__Host-Auth"));
    }

    [Test]
    public async Task AfterLogin_MeReturnsRegisteredUser()
    {
        var client = CreateClient();
        var (name, email) = NewCredentials();

        await RegisterAndVerify(client, name, email);
        await PerformLogin(client, email);

        var me = await client.Me();

        Assert.That(me.Name, Is.EqualTo(name));
    }

    private static async Task RegisterAndVerify(StorygameClient client, string name, string email)
    {
        await client.Register(new RegisterRequest(name, email));
        var mails = await client.Mail(email);
        var verificationCode = mails.Single(m => m.Subject == "Verification code").Message;
        await client.Verify(new VerifyUserRequest(email, verificationCode));
    }

    private static async Task PerformLogin(StorygameClient client, string email)
    {
        await client.Login(new LoginRequest(email));
        var mails = await client.Mail(email);
        var confirmationKey = mails.Single(m => m.Subject == "Login confirmation").Message;
        await client.ConfirmLogin(new ConfirmLoginRequest(confirmationKey));
    }

    private static (string name, string email) NewCredentials()
    {
        var id = Guid.NewGuid().ToString("N");
        return (id[..10], $"{id[..12]}@test.com");
    }
}
