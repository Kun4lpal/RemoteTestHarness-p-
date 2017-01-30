/////////////////////////////////////////////////////////////////////
// testharness.cs - Testharness class                              //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 2  Testharness prototype                   //
// Source:      Jim Fawcett, CSE681- Loading Tests demo            //
//              Software Modeling and Analysis, Fall 2016          //
// Author:      Kunal Paliwal, Syracuse University,                //
//              kupaliwa@syr.edu, (315) 876-8002                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This Module defines the Testharness class of a test harness. 
 * ->This module is responsible for creating a child app domain for separate processing of each testrequest.
 * ->After creating the child app domain, it is injected with the loader which runs all the main processes such as parsing,loading and 
 * running of tests.
 * 
 * Pulic methods:
 * ===================
 * createChildAppDomain()
 * this method as the name suggests is responsible for creating a new child app domain for every test request. 
 * 
 * injectloader()
 * ->this method injects the loader into the app domain and sends processing the entry point of the loader.
 *  
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWTools;
using CommChannelDemo;
using System.Threading;
using System.Security.Policy;
using Interface;
using Communication;

namespace testHarness
{
    public class TestHarness : MarshalByRefObject, ItestHarness
    {
        Message m;
        string xmlstring;
        public string[] results;

        public TestHarness(Message m) 
        {
            this.m = m;
            xmlstring = m.body;
            //DEFINE WHAT TO DO HERE. YOU CAN CALL A MEHTOD THAT RETURNS RESULTS HERE.
        }

      
        public Message getResults(Message m)
        {
            Message msg = new Message();
            results = performTests();

            Console.WriteLine("\nGETTING RESULTS {0}");

            Console.WriteLine("\n\n Requirement: If a Test Request specifies test DLLs" +
               " not available from the Repository, the Test Harness server will send back an error message to the client.");

            if (results[0] == " ")
            {
                msg.body = "Error";
            }
            else
            {
                msg.body = results[0];
            }
            return msg;
            
        }

        public string[] performTests()
        {
            AppDomain main = AppDomain.CurrentDomain;
            Console.Write("\n  Starting in AppDomain {0}\n", main.FriendlyName);
            

            AppDomainManager am = new AppDomainManager();
            results = am.createChildAppDomain(xmlstring);
            return results;
        }
        
        static void Main(string[] args)
        {
        }
    }
}
