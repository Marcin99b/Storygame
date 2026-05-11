using Moq;
using Shouldly;
using Storygame.Users;
using Storygame.Users.Queries;

namespace Storygame.Tests.Unit.Users;

[TestFixture]
public class GetUserByEmailQueryHandlerTests
{
    private Mock<IUsersRepository> _repository = null!;
    private GetUserByEmailQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUsersRepository>();
        _handler = new GetUserByEmailQueryHandler(_repository.Object);
    }

    [Test]
    public async Task Returns_user_matching_given_email()
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Name = "Jan Kowalski", Email = "jan@example.com",
            RegisteredAt = DateTime.UtcNow, VerifiedAt = null
        };
        _repository.Setup(r => r.GetUserByEmail("jan@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.HandleAsync(new GetUserByEmailQuery("jan@example.com"), CancellationToken.None);

        result.User.ShouldBe(user);
    }

    [Test]
    public async Task Passes_email_to_repository_without_modification()
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Name = "Anna", Email = "anna@example.com",
            RegisteredAt = DateTime.UtcNow, VerifiedAt = null
        };
        _repository.Setup(r => r.GetUserByEmail("anna@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await _handler.HandleAsync(new GetUserByEmailQuery("anna@example.com"), CancellationToken.None);

        _repository.Verify(r => r.GetUserByEmail("anna@example.com", It.IsAny<CancellationToken>()), Times.Once);
    }
}
