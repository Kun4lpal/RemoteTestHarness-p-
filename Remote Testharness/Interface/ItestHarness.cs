using CommChannelDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface ItestHarness
    {
        Message getResults(Message m);
        string[] performTests();
    }
}
