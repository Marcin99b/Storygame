using Moq;
using Shouldly;
using Storygame.Cqrs;
using Storygame.Integrations.Email;
using Storygame.Users;
using Storygame.Users.Commands;
using Storygame.Users.Events;

namespace Storygame.Tests.Unit.Users;

[TestFixture]
public class RegisterUserCommandHandlerTests
{
    private Mock<IUsersRepository> _repository = null!;
    private EmailClient _emailClient = null!;
    private Mock<IDispatcher> _dispatcher = null!;
    private RegisterUserCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUsersRepository>();
        _emailClient = new EmailClient();
        _dispatcher = new Mock<IDispatcher>();
        _handler = new RegisterUserCommandHandler(_repository.Object, _emailClient, _dispatcher.Object);
    }

    [Test]
    public async Task Throws_when_email_is_already_registered()
    {
        _repository.Setup(r => r.CheckIfEmailExist("jan@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<ArgumentException>(() =>
            _handler.HandleAsync(new RegisterUserCommand("Jan", "jan@example.com"), CancellationToken.None));
    }

    [Test]
    public async Task Saves_user_with_data_from_command()
    {
        _repository.Setup(r => r.CheckIfEmailExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        User? savedUser = null;
        _repository.Setup(r => r.AddUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => savedUser = u);

        await _handler.HandleAsync(new RegisterUserCommand("Jan Kowalski", "jan@example.com"), CancellationToken.None);

        savedUser.ShouldNotBeNull();
        savedUser.Id.ShouldNotBe(Guid.Empty);
        savedUser.Name.ShouldBe("Jan Kowalski");
        savedUser.Email.ShouldBe("jan@example.com");
        savedUser.VerifiedAt.ShouldBeNull();
        savedUser.RegisteredAt.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
    }

    [Test]
    public async Task Saves_verification_code_linked_to_the_new_user()
    {
        _repository.Setup(r => r.CheckIfEmailExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        User? savedUser = null;
        _repository.Setup(r => r.AddUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => savedUser = u);

        UserVerificationCode? savedCode = null;
        _repository.Setup(r => r.SaveUserVerificationCode(It.IsAny<UserVerificationCode>(), It.IsAny<CancellationToken>()))
            .Callback<UserVerificationCode, CancellationToken>((c, _) => savedCode = c);

        await _handler.HandleAsync(new RegisterUserCommand("Jan", "jan@example.com"), CancellationToken.None);

        savedCode.ShouldNotBeNull();
        savedCode.Id.ShouldNotBe(Guid.Empty);
        savedCode.UserId.ShouldBe(savedUser!.Id);
        savedCode.Code.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task Sends_verification_email_to_the_registered_address()
    {
        _repository.Setup(r => r.CheckIfEmailExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _handler.HandleAsync(new RegisterUserCommand("Jan", "jan@example.com"), CancellationToken.None);

        var sentMessages = await _emailClient.Read("jan@example.com");
        sentMessages.Length.ShouldBe(1);
        sentMessages[0].Receiver.ShouldBe("jan@example.com");
    }

    [Test]
    public async Task Publishes_event_with_user_data_after_registration()
    {
        _repository.Setup(r => r.CheckIfEmailExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        UserRegisteredEvent? publishedEvent = null;
        _dispatcher.Setup(d => d.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()))
            .Callback<UserRegisteredEvent, CancellationToken>((e, _) => publishedEvent = e);

        await _handler.HandleAsync(new RegisterUserCommand("Jan Kowalski", "jan@example.com"), CancellationToken.None);

        publishedEvent.ShouldNotBeNull();
        publishedEvent.UserId.ShouldNotBe(Guid.Empty);
        publishedEvent.Name.ShouldBe("Jan Kowalski");
        publishedEvent.Email.ShouldBe("jan@example.com");
    }

    [Test]
    public async Task Does_not_publish_event_when_email_already_exists()
    {
        _repository.Setup(r => r.CheckIfEmailExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Should.ThrowAsync<ArgumentException>(() =>
            _handler.HandleAsync(new RegisterUserCommand("Jan", "jan@example.com"), CancellationToken.None));

        _dispatcher.Verify(
            d => d.PublishAsync(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
