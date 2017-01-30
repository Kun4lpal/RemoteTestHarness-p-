/////////////////////////////////////////////////////////////////////
// codetotest2.cs     - This is the code which has to be tested    //
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
 * This Module defines the code to be tested by the test harness. 
 * 
 * Pulic methods:
 * ===================
 * int Subtract(int,int)
 * this method is called to subract two int values.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testHarness
{
    public class Codetotest2
    {
        public int Subtractor(int a, int b)   //performs subraction of two integers
        {
            int Sub = a - b;
            Console.Write("\n  Difference of two integers= {0}", Sub);
            return Sub;
        }
        static void Main(string[] args)
        {
            Codetotest2 ctt = new Codetotest2();
            int Sub = ctt.Subtractor(10, 5);
            Console.Write("\n\n");
        }
    }

}
