/////////////////////////////////////////////////////////////////////
// ITest.cs interface - ITest from whom test driver class derives  //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 2  Testharness prototype                   //
// Source:      Jim Fawcett, CSE681- ITest interface               //
//              Software Modeling and Analysis, Fall 2016          //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This Module defines the ITest of the test harness. Test driver derives from this interface.
 * 
 * Pulic methods:
 * ===================
 * bool test()
 * This method will be called by the test driver while testing the code to be tested.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testHarness
{
    public interface ITest
    {
        bool test();
    }
}
