using Moq;
using Shouldly;
using Storygame.Users;
using Storygame.Users.Queries;

namespace Storygame.Tests.Unit.Users;

[TestFixture]
public class GetUserByIdQueryHandlerTests
{
    private Mock<IUsersRepository> _repository = null!;
    private GetUserByIdQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IUsersRepository>();
        _handler = new GetUserByIdQueryHandler(_repository.Object);
    }

    [Test]
    public async Task Returns_user_matching_given_id()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId, Name = "Jan Kowalski", Email = "jan@example.com",
            RegisteredAt = DateTime.UtcNow, VerifiedAt = null
        };
        _repository.Setup(r => r.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.HandleAsync(new GetUserByIdQuery(userId), CancellationToken.None);

        result.User.ShouldBe(user);
    }

    [Test]
    public async Task Passes_user_id_to_repository_without_modification()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId, Name = "Jan", Email = "jan@example.com",
            RegisteredAt = DateTime.UtcNow, VerifiedAt = null
        };
        _repository.Setup(r => r.GetUserById(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        await _handler.HandleAsync(new GetUserByIdQuery(userId), CancellationToken.None);

        _repository.Verify(r => r.GetUserById(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
