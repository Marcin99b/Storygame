using Storygame.Contracts.WebApi.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Contracts.WebApi.Responses;

public record GetLibraryResponse(LibraryBookDto[] Books);
