﻿namespace Marketplace.Core.Exceptions
{
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException() : base() { }

        public AlreadyExistException(string message) : base(message) { }
    }
}
