using Storygame.Ownership;

namespace Storygame.Tracking;

public class Tracking : IOwnerable
{
    public required Guid Id { get; set; }
    public required Guid LibraryBookId { get; set; }
    public required Guid UserId { get; set; }
    public required int TotalLength { get; set; }
    public required int CurrentIndex { get; set; }
    public bool IsStarted => CurrentIndex > 0;
    public bool IsFinished => CurrentIndex == TotalLength;
}