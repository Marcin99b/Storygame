using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking;

public interface ITrackingRepository
{
    Task AddTracking(Tracking tracking);
    Task UpdateTracking(Tracking tracking);
    Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId);
    Task<IEnumerable<Tracking>> GetUserTrackings(Guid userId);
}
