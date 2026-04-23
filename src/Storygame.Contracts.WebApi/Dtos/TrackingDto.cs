using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Contracts.WebApi.Dtos;

public record TrackingDto(Guid Id, Guid LibraryBookId, Guid UserId, int TotalLength, int CurrentIndex, bool IsStarted, bool IsFinished);
