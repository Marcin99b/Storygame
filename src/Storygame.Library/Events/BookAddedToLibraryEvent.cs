using Storygame.Cqrs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library.Events;

public record BookAddedToLibraryEvent(Guid LibraryBookId, Guid UserId, Guid? CatalogBookId, Guid? ImageId, string Title, string Description, int Length, MediaType MediaType, DateTime AddedToLibraryAt) : IEvent;