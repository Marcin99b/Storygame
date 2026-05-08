using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Exceptions;

public class StorygameException : Exception
{
    public virtual int HttpStatusCode { get; } = 500;
}