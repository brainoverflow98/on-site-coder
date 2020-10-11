using System;
using System.Collections.Generic;

namespace WebApp.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }
        public object Model { get; }
        public ValidationException(IEnumerable<string> errors, object model)
        {
            Errors = errors;
            Model = model;
        }

        public ValidationException(string message, object model)
        {
            Errors = new List<string> { message };
            Model = model;
        }

        public ValidationException(IEnumerable<string> errors)
        {
            Errors = errors;
        }

        public ValidationException(string message)
        {
            Errors = new List<string> { message };
        }
    }
}
