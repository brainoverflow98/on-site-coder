using System;

namespace WebApp.Exceptions
{
    public class UnauthenticatedException : Exception
    {
        public UnauthenticatedException(string message)
            : base(message)
        {
        }
    }
}
