/////////////////////////////////////////////////////////////////////
// Message.cs - Message  class of the test harness                 //
// Application: CSE681-Software Modelling and analysis,            //
//              Project 2  Testharness prototype                   //
// Author:      Kunal Paliwal, Syracuse University,                //
//              kupaliwa@syr.edu, (315) 876-8002                   //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ==================
 * This Module defines the Message class of a test harness. 
 * ->This module is responsible for sending logs to new folder which can later be accessed by the client.
 * ->A new folder with the authors name is created and the required Dll files are stored in it.
 * ->This module is responsible for communication with the logger and repository
 * 
 * Pulic methods:
 * ===================
 * saveLogsInNewFolder(string, Dictionart<string,string>)
 * ->this method is responsible for saving logs to a new folder in text files.
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using SWTools;
using System.Collections;

namespace testHarness
{
    [Serializable]

    public class FileManager
    {
        public const string tempLogs = @"..\..\..\TServer\ToSend\";
        public const string tcPath = @"..\..\..\TServer\SavedFiles";
        private Dictionary<String, String> log;
        private List<string> requiredDLLs;
        private string tempdir = @"..\";

        // Create log text file and send them to new folder

        public string getRequiredDllFiles(Queue testDriverAndCode, String author)
        {

            string[] allFiles = Directory.GetFiles(tcPath);
            requiredDLLs = new List<string>();
            bool val;
            while (testDriverAndCode.Count != 0)
            {
                val = false;
                String currentDLL = (String)testDriverAndCode.Dequeue();
                foreach (string file in allFiles)
                {
                    string name = Path.GetFileName(file);
                    if (name.Equals(currentDLL))
                    {
                        requiredDLLs.Add(file);
                        val = true;
                        break;
                    }
                }
                if (val == false)
                {
                    Console.WriteLine("\n\nIncorrect files entered in the Test Request\n\n");
                    return null;
                }
            }

            return saveToTempFolder(author);

        }

        //------------- Saves current Test Drivers and Code to temp folder ------------
        public string saveToTempFolder(String author)
        {

            String path = tempdir + author + " " +
            DateTime.Now.ToString("MM-dd-yyyy") + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
            DirectoryInfo d = Directory.CreateDirectory(@path);

            foreach (string file in requiredDLLs)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string destFile = System.IO.Path.Combine(path, fileName);
                System.IO.File.Copy(file, destFile, true);
            }
            return path;
        }


        public void saveLogsInNewFolder(String author, Dictionary<String, String> log)
        {
            this.log = log;
            string path = tempLogs; 
            //Directory.CreateDirectory(path);

            Console.WriteLine("\n\n Requirement: Saving logs to the Repository with name of the File as - author name,"
                           + "date and time concatenated for each test");

            foreach (String str in log.Keys)
            {
                using (StreamWriter sw = File.CreateText(path + str + ".txt"))
                {
                    sw.WriteLine(log[str]);
                }
            }
        }

        ////Test stub for messaging class   // 
        static void Main(string[] args)
        {
            String path = @"..\..\..\..\Xmlfiles";
            Console.WriteLine("\n Accepting xmls from path :{0}", path);
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileManager fm = new FileManager();
            BlockingQueue<string> testRequestQueue = new BlockingQueue<string>();
            if (Directory.Exists(path))
            {
                string[] fileEntries = Directory.GetFiles(path);
                Console.WriteLine(" Accepting Test Requests from Path: {0}  \n", path);
                foreach (string filename in fileEntries)
                {
                    Console.WriteLine("{0}", filename);
                    testRequestQueue.enQ(File.ReadAllText(filename));
                }
                Console.WriteLine("\n Enqueueing all Test Requests \n");
                Console.WriteLine("\n Size of Blockingqueue: {0}", testRequestQueue.size());
            }
            else
            {
                Console.WriteLine("No XML was found at the path specified: {0} \n", path);
            }
            while (testRequestQueue.size() != 0)
            {
                Xmltest xt = new Xmltest();
                string xml = testRequestQueue.deQ();
                xt.parse(xml);                                       // Enqueing the queue with the xml string
                string[] files = Directory.GetFiles(path, "*.dll");
                List<string> l = new List<string>();
                Dictionary<String, String> log = new Dictionary<string, string>();
                // couldnt create a teststub for this because of 
                // circular dependency
                fm.saveLogsInNewFolder("Kunal Paliwal", log);
            }
        }
    }
}
