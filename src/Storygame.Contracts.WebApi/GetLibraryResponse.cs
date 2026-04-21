using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Contracts.WebApi;

public record GetLibraryResponse(LibraryBookDto[] Books);
