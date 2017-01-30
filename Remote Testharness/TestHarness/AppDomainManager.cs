/////////////////////////////////////////////////////////////////////
// testharness.cs - AppDomainManager class                         //
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
using System.IO;
using Communication;
using Interface;

namespace testHarness
{
    class AppDomainManager
    {
      
        public string[] results;
        string path;


        public string[] createChildAppDomain(string xmlstring)
        {
            Console.WriteLine("\n Requirement:The Test Harness shall enqueue Test Requests from multiple concurrent clients and execute" + 
                "them by creating, for each Test Request, an AppDomain, running on its own thread."+  
                "Once a child AppDomain is constructed, the Test Harness shall start the child processing the dequeued Test Request."+ 
                "The result is that Test Requests can be processed concurrently.\n");
            AppDomain main = AppDomain.CurrentDomain;
            Console.Write("\n  Starting in AppDomain {0}\n", main.FriendlyName);


            //Create application domain setup information for new AppDomain

           AppDomainSetup domaininfo = new AppDomainSetup();
           domaininfo.ApplicationBase
             = "file:///" + Environment.CurrentDirectory;  // defines search path for assemblies
           Evidence adevidence = AppDomain.CurrentDomain.Evidence;

            try
            {
                AppDomain childDomain = AppDomain.CreateDomain("ChildDomain", adevidence ,domaininfo);
                Console.WriteLine("===============================================================================================");
                Console.WriteLine("Creating childappdomain for separate processing: {0}", childDomain);
                Console.WriteLine("===============================================================================================");

                Console.WriteLine("\n Requirement: Creating child app domain and passing it the xml test request in string");
                childDomain.SetData("testRequest", xmlstring);    //Setting data for child app domain.                                                             
                Object ob = childDomain.CreateInstanceAndUnwrap("Loader", "testHarness.Loader");
                Loader ld = (Loader)ob;
                results = ld.beginLoader(xmlstring);  //Calling Loader, this is where Loader operation begins - PARSING/LOAD&RUN

                AppDomain.Unload(childDomain);            //Here we unload the child Appdomain

                Console.WriteLine("\n\t We are back in the Main Appdomain : {0} ", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine("\n TesT request completed\n");

                path = results[1];
                if (path != null)
                {   //If the temporary folder was created to store the current test Drivers and test code.
                    Console.WriteLine("\nDeleting temporary folder that was created.");
                    Directory.Delete(path, true);
                    Console.WriteLine("\nTemporary Folder Deleted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n Exception caught in Test harness (Main Domain):{0}", ex.Message);
            }

            return results;
        }
    }
}
