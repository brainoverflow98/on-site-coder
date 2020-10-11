using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExecutionService.Exceptions
{
    public class ExecutionServiceException : Exception
    {
        public ExecutionServiceException(string message) : base(message) { }

        public ExecutionServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
