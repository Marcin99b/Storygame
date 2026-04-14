using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tracking;

public interface ITrackingRepository
{
    void AddTracking(Tracking tracking);
    Task<bool> CheckIfBookIsAlreadyTracked(Guid libraryBookId);
}
