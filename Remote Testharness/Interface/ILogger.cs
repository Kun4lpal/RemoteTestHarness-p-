using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testHarness;

namespace Interface
{
    public interface ILogger
    {
        Dictionary<string, string> DisplayLog(Dictionary<string, string> log, Test t, string result);
    }
}
