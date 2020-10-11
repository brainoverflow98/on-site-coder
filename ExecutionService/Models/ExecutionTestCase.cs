

using Common.DataBase.Entities;
using System.Collections.Generic;

namespace ExecutionService.Models
{
    public class ExecutionTestCase
    {
        public int No { get; set; }
        public string Arguments { get; set; }
        public bool IsHidden { get; set; }
        public string ExpectedOutput { get; set; }
        public string UserOutput { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
