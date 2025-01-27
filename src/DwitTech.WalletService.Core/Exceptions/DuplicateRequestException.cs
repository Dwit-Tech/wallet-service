﻿namespace DwitTech.WalletService.Core.Exceptions
{
    public class DuplicateRequestException : Exception
    {
        public DuplicateRequestException() { }

        public DuplicateRequestException(string message) : base(message) { }

        public DuplicateRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
