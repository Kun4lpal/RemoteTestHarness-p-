/////////////////////////////////////////////////////////////////////
// loadandrun.cs - loading/running of test drivers                 //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 2  Testharness prototype                   //
// Source:      Jim Fawcett, CSE681- loading test demo             //
//              Software Modeling and Analysis, Fall 2016          //
// Author:      Kunal Paliwal, Syracuse University,                //
//              kupaliwa@syr.edu, (315) 876-8002                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This Module defines load and run class of the test harness. 
 * ->This module loads and runs tests.
 * ->The dll files are loaded from the path specified by the client. 
 * ->These files are checked if they derive from ITest interface and are added to a list. 
 * ->Next all the testdrivers in the list are executed and result is passed.  
 * ->Logs are also displayed while running of tests.
 * 
 * Pulic Methods:
 * ===================
 * LoadTests(string path)
 * ->assings the xmlPath that is retreived from the command line. 
 * 
 * run()
 * ->Executes and logs the result. 
 * 
 * loadAndRun(string,List<Test>)
 * -> this function returns saved logs . 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using SWTools;

namespace testHarness
{
    [Serializable]
    public class Loadandrun
    {
        private List<Testdriverinfo> testDriverlist = new List<Testdriverinfo>();
        private List<Test> test = new List<Test>();
        private Dictionary<String, String> log = new Dictionary<string, string>();
        private Logger logger = new Logger();

        private struct Testdriverinfo
        {
            public string Name;
            public ITest testDriver;
        }

        public Loadandrun()
        {
            logger = new Logger();

        }

        public bool LoadTests(string path)            //loading all Dll files
        {
            string[] files = Directory.GetFiles(path, "*.dll");
            Console.WriteLine("\n Requirement: Checking to see which files derive from the ITest interface");
            foreach (string file in files)
            {

                try
                {
                    string path1 = Path.GetFullPath(file);
                    Assembly assem = Assembly.LoadFrom(path1);
                    Type[] types = assem.GetExportedTypes();
                    foreach (Type t in types)
                    {
                        if (t.IsClass && typeof(ITest).IsAssignableFrom(t))  // checking to see if it derives from Itest
                        {
                            ITest tdr = (ITest)Activator.CreateInstance(t);    // create an instance of test driver
                            Testdriverinfo td = new Testdriverinfo();          // save type name and reference to created type on managed heap
                            td.Name = t.Name + ".dll";
                            td.testDriver = tdr;
                            Console.Write("loading: {0}", t.Name);
                            testDriverlist.Add(td);
                        }
                    }
                    Console.Write("\n");
                }
                catch (Exception ex)
                {
                    Console.Write("\n{0}\n", ex.Message);
                    return false;
                }
            }
            return testDriverlist.Count > 0;   // if we have items in list then Load succeeded
        }

        public void run()
        {
            foreach (Testdriverinfo td in testDriverlist)  // Test execution for each test driver which were loaded
            {
                try
                {
                    Console.WriteLine("\n\t Testing {0}", td.Name);
                    if (td.testDriver.test() == true)
                    {
                        foreach (Test t in test)
                        {
                            if (t.testDriver == td.Name)
                            {                                     //Checking for the current test driver i.e the one for which the test executed                                
                                log = logger.DisplayLog(log, t, "PASSED");        //Displaying logs by calling getLog()
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (Test t in test)
                        {
                            if (t.testDriver == td.Name)
                            {                                      //Checking for the current test driver i.e the one for which the test executed                                     
                                log = logger.DisplayLog(log, t, "FAILED");
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n Exception thrown while running the Test Driver: {0}", ex.Message);
                    //If an exception is thrown, it means the Test has failed. So, generating logs specfying the exception and Test status
                    //as FAILED.
                    foreach (Test t in test)
                    {
                        if (t.testDriver == td.Name)
                        {   //Current Log                             
                            log = logger.DisplayLog(log, t, ex.Message);         //creating the test log for fail case.
                            break;
                        }
                    }
                }
            }
        }

        public Dictionary<string,string> loadAndRun(string path, List<Test> test)     //this method calls load and run methods above
        {
            this.test = test;    // try to remove this.test if possible
            if (LoadTests(path))
                run();
            else
                Console.Write("\n Could not load tests");
            return log;
        }

        //Test stub for load and run class
        static void Main(String[] args)
        {

            //FileManager fm = new FileManager();
            //Loadandrun ld = new Loadandrun();
            //Xmltest parser = new Xmltest();
            //List<string> l = new List<string>();

            //String xmlPath = @".\.\.\.\Xmlfiles";
            //DirectoryInfo xmldir = new DirectoryInfo(xmlPath);
            //Console.WriteLine("\n Accepting XML Files from path :{0}", xmlPath);

            //BlockingQueue<string> testRequestQueue = new BlockingQueue<string>();

            //if (Directory.Exists(xmlPath))
            //{
            //    string[] fileEntries = Directory.GetFiles(xmlPath);
            //    Console.WriteLine("\n Accepting Test Requests from Path: {0}  \n", xmlPath);
            //    foreach (string filename in fileEntries)
            //    {
            //        Console.WriteLine("{0}", Path.GetFileName(filename));
            //        testRequestQueue.enQ(File.ReadAllText(filename));
            //    }
            //    Console.WriteLine("\n Enqueueing all Test Requests");
            //    Console.WriteLine("\n Size of Blockingqueue: {0}", testRequestQueue.size());
            //}
            //else
            //{
            //    Console.WriteLine("No XML was found at the path specified: {0} \n", xmlPath);
            //}

            //int count = testRequestQueue.size();

            //while (count != 0)
            //{
            //    List<Test> test = parser.parse((String)Queue.deQ());
            //    foreach (Test t in test)
            //    {
            //        Queue.enQ(t.testDriver);
            //        Console.WriteLine("\t-> " + t.testDriver);
            //        foreach (string test1 in t.testCode)
            //        {
            //            Console.WriteLine("\t-> " + test1);
            //            Queue.enQ(test1);
            //        }
            //    }

            //}
        }
    }
}
