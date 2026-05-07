namespace Storygame.Tracking;

public class TrackingStatistic
{
    //todo - configurable timezones per user
    public required Guid Id { get; set; }
    public required Guid TrackingId { get; set; }
    public required TimePeriod TimePeriod { get; set; }
    public required TimeRange TimeRange { get; set; }
}
