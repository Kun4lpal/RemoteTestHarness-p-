/////////////////////////////////////////////////////////////////////
// codetotest1.cs     - This is the code which has to be tested    //
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
 * int adder(int,int)
 * this method is called to add two int values.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testHarness
{
    public class Codetotest1
    {
        public int adder(int a, int b)                  //performs addition of two integers
        {
            int sum = a + b;
            Console.Write("\n  Sum of two integers= {0}", sum);
            return sum;
        }
        static void Main(string[] args)
        {
            Codetotest1 ctt = new Codetotest1();
            int S = ctt.adder(4, 5);
            Console.Write("\n\n");
        }
    }
}
