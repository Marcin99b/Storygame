using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Library;

public class Book
{
    public required Guid Id { get; set; }
    public required Guid? CatalogBookId { get; set; }
    public required Guid? ImageId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required MediaType MediaType { get; set; }
    public required int Length { get; set; }
    public required DateTime AddedToLibraryAt { get; set; }

    public LengthUnit LengthUnit => MediaType switch
    {
        MediaType.Ebook => LengthUnit.Pages,
        MediaType.Paperback => LengthUnit.Pages,
        MediaType.Audiobook => LengthUnit.Minutes,
        _ => throw new NotImplementedException()
    };
}

public enum MediaType
{
    Ebook,
    Paperback,
    Audiobook
}

public enum LengthUnit
{
    Pages,
    Minutes
}