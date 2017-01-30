/////////////////////////////////////////////////////////////////////
// Loader.cs - Loader class of the test harness                    //
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
 * This Module defines the Loader class of a test harness. 
 * ->this module is the entry point of the test harness test processing. Here loading as well as the test execution takes place.
 * Loader fetches the required DLLs and saves them in a new folder by calling the message class.
 * 
 * Pulic methods:
 * ===================

 * begin loader(string)
 * this method has testrequest in string format as the argument and performs parsing,loading and running of tests. This is 
 * the entry point of the loader.
 */

using System;
using System.Collections.Generic;
using System.IO;
using SWTools;
using Interface;
using System.Collections;
using System.Collections.ObjectModel;

namespace testHarness
{

    //public class StringPair
    //{
    //    public string path;
    //    public string results;
    //}

    [Serializable]
    public class Loader : MarshalByRefObject, ILoader
    {
        private List<Test> test = new List<Test>();
        public const string tcPath = @"..\..\..\TServer\SavedFiles";
        private string path;
        string[] results = new string[2];

        private Dictionary<String, String> log = new Dictionary<string, string>();

        //StringPair results;
        FileManager fm = new FileManager();
        Loadandrun ld = new Loadandrun();

        public Loader()
        {
            FileManager fm = new FileManager();
        }

        public string RetrieveTestDriverAndCodeFromRepository()
        {
            Queue testDriverAndCode = new Queue();

            Console.WriteLine("\n--> Test Drivers and Test Code for this test request: " +
                "-------------------------------------------------------------------------------->REQUIREMENT 2");
            foreach (Test t in test)
            {   //Displaying Test Drivers and Test Code for this test request.              
                testDriverAndCode.Enqueue(t.testDriver);
                Console.WriteLine("\t-> " + t.testDriver);
                foreach (string test in t.testCode)
                {
                    Console.WriteLine("\t-> " + test);
                    testDriverAndCode.Enqueue(test);
                }
            }

            Console.WriteLine("\n-->Calling FileManager to retrieve these Test Drivers and Code from Repository \n" +
                "   Path: " + tcPath);

            //Retrieves temp directory info for the current test drivers and test code
            return path = fm.getRequiredDllFiles(testDriverAndCode, test[0].author);


        }

        public string[] beginLoader(string xmlstring)     // This is the entry point of loader which is called after creating AppDomain
        {
            string newpath = String.Empty;
            string status = String.Empty;
            try
            {
                Console.WriteLine("\n Current domain: {0}" + "", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine("\n Requirement: Executing each test request in a serialized manner, running in isolation");

                Xmltest a = new Xmltest();
                Console.WriteLine("\n Parsing testrequest");
                test = a.parse(xmlstring);    //Call to parser

                newpath = RetrieveTestDriverAndCodeFromRepository();
                //Calling function to parse the XML File
                if (test == null)
                {   //If no test cases then an execption will be thrown. 
                    Console.WriteLine("\n Couldnt parse any test requests, parser failure!");
                }
                //Retrieving Test Drivers and Code from Repository
                if (newpath != null)
                {
                    ld = new Loadandrun();     // here Load and Run operation begins
                    Console.WriteLine("\n\t Test execution begins here");

                    log = ld.loadAndRun(newpath, test);  // load and run methods are called.

                    Console.WriteLine("\n\t Test execution ends here");
                    fm.saveLogsInNewFolder(test[0].author, log);
                }

                Console.WriteLine("\n Exiting Domain: {0}\n", AppDomain.CurrentDomain.FriendlyName);

            }
            catch (Exception ex)
            {
                Console.WriteLine("\n Exception Caught in loader (Child Domain): {0}", ex.Message);
            }

            foreach (string str in log.Values)
            {
                status = status + str + " ";
            }
            
            results[0] = status;
            results[1] = path;
            return results;
        }

        //Test stub for loader class
        static void main(string[] args)
        {
            String xmlPath = @"..\..\..\..\Xmlfiles";
            Console.WriteLine(" Accepting XML Files from path {0}", xmlPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(xmlPath);
            FileInfo[] testRequestXmlFile = directoryInfo.GetFiles("*.xml");
            BlockingQueue<string> bq = new BlockingQueue<string>();

            if (Directory.Exists(xmlPath))
            {
                string[] fileEntries = Directory.GetFiles(xmlPath);
                Console.WriteLine("\n Accepting Test Requests from Path: {0}  \n", xmlPath);
                foreach (string filename in fileEntries)
                {
                    Console.WriteLine("{0}", Path.GetFileName(filename));
                    bq.enQ(File.ReadAllText(filename));
                }
            }
            else
            {
                Console.WriteLine("No XML was found at the path specified: {0} \n", xmlPath);
            }

            int count = bq.size();
            while (count != 0)
            {
                string xmlstring = bq.deQ();
                Loader loader = new Loader();
                loader.beginLoader(xmlstring);
                count--;
            }

        }
    }
}
