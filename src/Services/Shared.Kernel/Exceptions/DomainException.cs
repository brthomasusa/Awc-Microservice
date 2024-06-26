﻿
namespace AWC.Shared.Kernel.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message, Exception ex) : base(message, ex)
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException() : base()
    {
    }
}
