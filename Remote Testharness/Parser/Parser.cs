/////////////////////////////////////////////////////////////////////
// xmlparser.cs - Parser module of the test harness                  //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 2  Testharness prototype                   //
// Source:      Jim Fawcett, CSE681- Xmlhelp demo                  //
//              Software Modeling and Analysis, Fall 2016          //
// Author:      Kunal Paliwal, Syracuse University,                //
//              kupaliwa@syr.edu, (315) 876-8002                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This Module defines the parser class of the test harness. 
 * ->This module is responsible for parsing the xml string.
 * ->The name of the author, testdrivers and code to be tested are extracted using parsing.
 * ->XDocument class is used to convtert xml to string format and then fed to the parser.
 * ->This module has two class Test is to get and set values of all the attributes and XMl test is used to parse the xml in string.
 * 
 * Pulic methods:
 * ===================
 * parse(string path)
 * this method is called to parse the xml file in string format. All the attribute values are obtained from the string and can be called 
 * or changed using the get and set methods.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SWTools;

namespace testHarness
{
    public class Test
    {
        //Class that defines a TEST CASE
        public string testName { get; set; }
        public string author { get; set; }
        public DateTime timeStamp { get; set; }
        public String testDriver { get; set; }
        public List<String> testCode { get; set; }
        public void show()
        {
            Console.Write("\n {0,-12} : {1}", "test name", testName);
            Console.Write("\n {0,12} : {1}", "author", author);
            Console.Write("\n {0,12} : {1}", "Time Stamp", timeStamp);
            Console.Write("\n {0,12} : {1}", "Test Driver", testDriver);
            foreach (string library in testCode)
            {
                Console.Write("\n {0,12} : {1}", "library", library);
            }

        }
    }
    public class Xmltest
    {
        private XDocument doc_ { get; set; }
        private List<Test> testList_ { get; set; }
        // constructor
        public Xmltest()
        {
            doc_ = new XDocument();
            testList_ = new List<Test>();
        }
        //Parses the xml string
        public List<Test> parse(String xml)
        {
            try                                          // try catch block will catch any error in parsing amd assign null 
            {                                               // to the test list_
                doc_ = XDocument.Parse(xml);
                if (doc_ != null)
                {
                    string author = doc_.Descendants("author").First().Value;

                    Console.WriteLine("\n Inside Parser");
                    XElement[] xtests = doc_.Descendants("test").ToArray();
                    int numTests = xtests.Count();
                    for (int i = 0; i < numTests; i++)
                    {
                        Test test = new Test();
                        test.testCode = new List<String>();
                        test.author = author;
                        test.timeStamp = DateTime.Now;
                        test.testName = xtests[i].Attribute("name") != null ? xtests[i].Attribute("name").Value : "false";
                        test.testDriver = xtests[i].Element("testDriver").Value; //Element because test driver is the child now
                        IEnumerable<XElement> xtestCode = xtests[i].Elements("library");
                        foreach (var xlibrary in xtestCode)
                        {
                            test.testCode.Add(xlibrary.Value);
                        }
                        testList_.Add(test);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n Exception was caught and testList_ set to NULL: {0}", ex.Message);
                testList_ = null;   //Passes null to the test list (List<Test>) if any error encountered while parsing. 
            }
            return testList_;
        }


        static void main(string[] args)
        {
            Xmltest parser = new Xmltest();
            String xmlPath = @"../../../XMLTestRequest";
            Console.WriteLine("\n\n Accepting XMLs from path : \"{0}\"", xmlPath);
            DirectoryInfo dirInfo = new DirectoryInfo(xmlPath);
            BlockingQueue<string> xmlqueue = new BlockingQueue<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(xmlPath);
            if (Directory.Exists(xmlPath))
            {
                string[] fileEntries = Directory.GetFiles(xmlPath);
                Console.WriteLine("\n Accepting Test Requests from Path: {0}  \n", xmlPath);
                foreach (string filename in fileEntries)
                {
                    Console.WriteLine("{0}", Path.GetFileName(filename));
                    xmlqueue.enQ(File.ReadAllText(filename));
                }
                Console.WriteLine("\n Enqueueing all Test Requests");
                Console.WriteLine("\n Size of Blockingqueue: {0}", xmlqueue.size());
            }
            else
            {
                Console.WriteLine("No XML was found at the path specified: {0} \n", xmlPath);
            }


            int count = xmlqueue.size();
            while (count != 0)
            {
                List<Test> test = parser.parse((String)xmlqueue.deQ());

                foreach (Test t in test)
                    t.show();

                count--;
            }
        }
    }
}
