using System;

namespace OpenNetworkStatus.Services.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() {}
        public NotFoundException(string message) : base(message)
        {
        }
    }
}