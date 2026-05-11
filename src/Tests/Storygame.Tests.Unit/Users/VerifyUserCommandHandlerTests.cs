using Moq;
using Shouldly;
using Storygame.Cqrs;
using Storygame.Users;
using Storygame.Users.Commands;
using Storygame.Users.Events;

namespace Storygame.Tests.Unit.Users;

[TestFixture]
public class VerifyUserCommandHandlerTests
{
    private Mock<IUsersRepository> _repository = null!;
    private Mock<IDispatcher> _dispatcher = null!;
    private VerifyUserCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUsersRepository>();
        _dispatcher = new Mock<IDispatcher>();
        _handler = new VerifyUserCommandHandler(_repository.Object, _dispatcher.Object);
    }

    [Test]
    public async Task Throws_when_user_is_already_verified()
    {
        var user = VerifiedUser();
        _repository.Setup(r => r.GetUserByEmail(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Should.ThrowAsync<ArgumentException>(() =>
            _handler.HandleAsync(new VerifyUserCommand(user.Email, "ANYCODE"), CancellationToken.None));
    }

    [Test]
    public async Task Throws_when_verification_code_does_not_match()
    {
        var user = UnverifiedUser();
        _repository.Setup(r => r.GetUserByEmail(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repository.Setup(r => r.GetUserVerificationCode(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserVerificationCode(Guid.NewGuid(), user.Id, "CORRECTCODE"));

        await Should.ThrowAsync<ArgumentException>(() =>
            _handler.HandleAsync(new VerifyUserCommand(user.Email, "WRONGCODE"), CancellationToken.None));
    }

    [Test]
    public async Task Sets_verified_at_timestamp_and_updates_user()
    {
        var user = UnverifiedUser();
        _repository.Setup(r => r.GetUserByEmail(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repository.Setup(r => r.GetUserVerificationCode(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserVerificationCode(Guid.NewGuid(), user.Id, "VALIDCODE"));

        User? updatedUser = null;
        _repository.Setup(r => r.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => updatedUser = u);

        await _handler.HandleAsync(new VerifyUserCommand(user.Email, "VALIDCODE"), CancellationToken.None);

        updatedUser.ShouldNotBeNull();
        updatedUser.VerifiedAt.ShouldNotBeNull();
        updatedUser.VerifiedAt!.Value.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
    }

    [Test]
    public async Task Publishes_event_with_user_id_and_verification_timestamp()
    {
        var user = UnverifiedUser();
        _repository.Setup(r => r.GetUserByEmail(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repository.Setup(r => r.GetUserVerificationCode(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserVerificationCode(Guid.NewGuid(), user.Id, "VALIDCODE"));

        UserVerifiedEvent? publishedEvent = null;
        _dispatcher.Setup(d => d.PublishAsync(It.IsAny<UserVerifiedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<UserVerifiedEvent, CancellationToken>((e, _) => publishedEvent = e);

        await _handler.HandleAsync(new VerifyUserCommand(user.Email, "VALIDCODE"), CancellationToken.None);

        publishedEvent.ShouldNotBeNull();
        publishedEvent.UserId.ShouldBe(user.Id);
        publishedEvent.VerifiedAt.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
    }

    [Test]
    public async Task Does_not_update_user_when_code_is_wrong()
    {
        var user = UnverifiedUser();
        _repository.Setup(r => r.GetUserByEmail(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repository.Setup(r => r.GetUserVerificationCode(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserVerificationCode(Guid.NewGuid(), user.Id, "CORRECTCODE"));

        await Should.ThrowAsync<ArgumentException>(() =>
            _handler.HandleAsync(new VerifyUserCommand(user.Email, "WRONGCODE"), CancellationToken.None));

        _repository.Verify(r => r.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Does_not_publish_event_when_code_is_wrong()
    {
        var user = UnverifiedUser();
        _repository.Setup(r => r.GetUserByEmail(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _repository.Setup(r => r.GetUserVerificationCode(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserVerificationCode(Guid.NewGuid(), user.Id, "CORRECTCODE"));

        await Should.ThrowAsync<ArgumentException>(() =>
            _handler.HandleAsync(new VerifyUserCommand(user.Email, "WRONGCODE"), CancellationToken.None));

        _dispatcher.Verify(
            d => d.PublishAsync(It.IsAny<UserVerifiedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static User UnverifiedUser() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Jan Kowalski",
        Email = "jan@example.com",
        RegisteredAt = DateTime.UtcNow.AddDays(-1),
        VerifiedAt = null
    };

    private static User VerifiedUser() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Jan Kowalski",
        Email = "jan@example.com",
        RegisteredAt = DateTime.UtcNow.AddDays(-1),
        VerifiedAt = new DateTime(2026, 5, 1)
    };
}
