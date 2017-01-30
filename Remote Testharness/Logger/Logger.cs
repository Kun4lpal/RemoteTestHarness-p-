/////////////////////////////////////////////////////////////////////
// logger.cs - Logger of the test harness                          //
// Application: CSE681-Software Modelling and analysis,            // 
//              Project 2 Testharness prototype                    //
// Source:      Jim Fawcett, CSE681- Console logger demo           //
//              Software Modeling and Analysis, Fall 2016          //
// Author:      Kunal Paliwal, Syracuse University,                //
//              kupaliwa@syr.edu, (315) 876-8002                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This Module defines the Logger of a test harness. 
 * ->This module is responsible for getting saved logs from the repository when the client makes a request.
 * ->This module is responsible for displaying logs for each test case. The value string of the Dictionary<string,string> log
 * is used for display.
 * 
 * Pulic Methods:
 * ===================
 * getSavedLogs()
 * ->This method is used to get saved logs from the repository.  
 * 
 * Displaylog()
 * -> This method is used to display logs after test execution. The test Status is displayed in the log along with other
 * information.
 * 
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Xml.Linq;
using SWTools;
using Interface;


namespace testHarness
{
    [Serializable]
    public class Logger : MarshalByRefObject, ILogger
    {
        public Dictionary<string,string> DisplayLog(Dictionary<string,string> log, Test t, string result)  //getlog function to display logs during
                                                                                       // load and run operation
        {
            String testCode = string.Empty;
            int count = t.testCode.Count;
            foreach (String library in t.testCode)
            {
                while (count != 1)
                {
                    testCode = testCode + library;
                    break;
                }
                testCode = testCode + library + " ";
            }


            string Logstring = string.Format("\n {0,-12} : {1}", "test name", t.testName) + string.Format("\n {0,-12} : {1}", "author", t.author)
                + string.Format("\n {0,-12} : {1}", "test driver", t.testDriver) + string.Format("\n {0,-12} : {1}", "test code", testCode)
                + string.Format("\n {0,-12} : {1}", "result", result) + string.Format("\n {0,-12} : {1}", "time stamp", t.timeStamp);

            log.Add(t.author + "-" + t.testName + " " +
            DateTime.Now.ToString("MM-dd-yy") + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second, Logstring);

            Console.WriteLine("\t Test Results {0} ", result);
            Console.WriteLine(Logstring);
            return log;

        }



        //Test stub for logger class
        static void Main(String args)
        {
            Logger log = new Logger();
            Dictionary<String, String> example = new Dictionary<String, String>();
            String xmlPath = @"..\..\..\..\Xmlfiles";
            Console.WriteLine("\n Accepting XML Files from path :{0}", xmlPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(xmlPath);
            BlockingQueue<string> testRequestQueue = new BlockingQueue<string>();

            if (Directory.Exists(xmlPath))
            {
                string[] fileEntries = Directory.GetFiles(xmlPath);
                Console.WriteLine("\n Accepting Test Requests from Path: {0}  \n", xmlPath);
                foreach (string filename in fileEntries)
                {
                    Console.WriteLine("{0}", Path.GetFileName(filename));
                    testRequestQueue.enQ(File.ReadAllText(filename));
                }
                Console.WriteLine("\n Enqueueing all Test Requests");
                Console.WriteLine("\n Size of Blockingqueue: {0}", testRequestQueue.size());
            }
            else
            {
                Console.WriteLine("No XML was found at the path specified: {0} \n", xmlPath);
            }

            int count = testRequestQueue.size();

            while (count != 0)
            {
                Xmltest xt = new Xmltest();
                List<Test> test = xt.parse((String)testRequestQueue.deQ());

                foreach (Test t in test)
                    log.DisplayLog(example, t, "PASS");

                foreach (String l in example.Values)
                    Console.WriteLine("\n {0}", l);
                count--;
            }
            //log.getSavedLogs(0);

        }
    }
}
