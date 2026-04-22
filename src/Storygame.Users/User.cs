namespace Storygame.Users;

public class User
{
    /// <summary>
    /// User object cannot be older than User class definition
    /// </summary>
    private static readonly DateTime MIN_VERIFICATION_DATE_TIME = new DateTime(2026, 4, 13);

    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required DateTime RegisteredAt { get; set; }
    public required DateTime? VerifiedAt { get; set; }
    public bool IsVerified => VerifiedAt.GetValueOrDefault() > MIN_VERIFICATION_DATE_TIME;
}
