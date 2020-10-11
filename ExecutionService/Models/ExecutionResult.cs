using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExecutionService.Models
{
    public class ExecutionResult
    {
        public int FailedTestCount { get; set; }
        public int SuccessfulTestCount { get; set; }
        public IEnumerable<ExecutionTestResult> TestCases { get; set; }
    }

    public class ExecutionTestResult
    {
        public int No { get; set; }        
        public string UserOutput { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
