using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Catalog;

public class Book
{
    public required Guid Id { get; set; }
    public required Guid? ImageId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required TextEditionFields TextEditionFields { get; set; }
    public required AudiobookFields AudiobookFields { get; set; }
}

public class TextEditionFields
{
    public required bool Exist { get; set; }
    public required int TotalPages { get; set; }
}

public class AudiobookFields
{
    public required bool Exist { get; set; }
    public required int TotalMinutes { get; set; }
}