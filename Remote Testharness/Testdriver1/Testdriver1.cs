/////////////////////////////////////////////////////////////////////
// testdriver1.cs     - This is the testdriver which performs test //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 2  Testharness prototype                   //
// Source:      Jim Fawcett, CSE681- loadingtests demo             //
//              Software Modeling and Analysis, Fall 2016          //
// Author:      Kunal Paliwal, Syracuse University,                //
//              kupaliwa@syr.edu, (315) 876-8002                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * Test driver derives from the ITest interface and performs test on code to be tested. A reference has to be made to 
 * codetotest.
 * 
 * Pulic methods:
 * ===================
 * bool test() function has the testing code which is needed to test the referenced code() code to test;
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testHarness
{
    public class Testdriver1 : MarshalByRefObject, ITest
    {
        AppDomain childAD = null;
        private Codetotest1 code;


        public Testdriver1()
        {
            code = new Codetotest1();
            childAD = AppDomain.CurrentDomain;
        }
        public static ITest create()
        {
            return new Testdriver1();
        }

        public bool test()
        {
            Console.Write("\n  Loading and executing in childAppDomain {0}", childAD.FriendlyName);
            int Sum = code.adder(4, 5);
            if (Sum == 9)
            {
                return true;
            }
            else return false;
        }

    }
}
