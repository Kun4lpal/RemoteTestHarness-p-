/////////////////////////////////////////////////////////////////////
// TServer.cs - Demonstrate application use of channel              //
// Ver 1.0                                                          //
// Kunal Paliwal, CSE681 - Software Modeling and Analysis,Fall 2016 //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This is the main communication package of the remote test harness. It communicates with the clients and the repository
 * using WCF. Messages are sent and received in data format and filestream.
 * 
 * Required Files:
 * ---------------
 * ICommunicator
 * Utilities- HRTIMER
 * testharness files
 * 
 * Maintenance History:
 * --------------------
 * Ver 1.0 : 23 Nov 2016
 * - first release 
 *  
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Communication;
using CommChannelDemo;
using SWTools;
using testHarness;
using Interface;
using Utilities;
using System.ServiceModel;
using System.IO;

namespace Server
{
    class Tserver
    {
        public Comm<Tserver> comm { get; set; } = new Comm<Tserver>();
        public string endPoint { get; } = "http://localhost:" + "4000" + "/ITestHarness";
        private Thread rcvThread = null;
        delegate void NewMessage(Message msg);
        BlockingQueue<Message> testQueue = new BlockingQueue<Message>();
        private int threadpool = 0;
        private int maxthreads;

        private int BlockSize = 1024;
        private byte[] block;
        private HiResTimer hrt = null;
        private string ToSendPath = "..\\..\\ToSend";
        private string SavePath = "..\\..\\SavedFiles";
        private ICommunicator channel;
        private ServiceHost host;
        private Message utlitity = new Message();
        private List<Test> test = new List<Test>();
        private List<string> requiredDLLs;

        Tserver(int max)
        {
            this.maxthreads = max;
            host = comm.rcvr.CreateRecvChannel(endPoint);                                   // service hosted
            rcvThread = new Thread(new ThreadStart(this.rcvThreadProc));
            rcvThread.IsBackground = true;
            rcvThread.Start();

            block = new byte[BlockSize];
            hrt = new HiResTimer();
        }

        public Message startTesting(BlockingQueue<Message> q)
        {
            Message m = q.deQ();
            getFilesFromRepo(m.body);
            TestHarness t = new TestHarness(m);
            Message msg = t.getResults(m);
            return msg;
        }

        void OnNewMessageHandler(Message msg)                        //this is where delegate is pointing to. 
        {
            testQueue.enQ(msg);                               // with 1000's of messages on the list box.
            Console.WriteLine("\n Message was queued {0}", msg.type);
        }


        public void wait()
        {
            rcvThread.Join();
        }


        public Message makeMessage(string author, string fromEndPoint, string toEndPoint)
        {
            Message msg = new Message();
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            return msg;
        }

        public Message performTask(Message msg)
        {
            Task<Message> t = new Task<Message>(() => startTesting(testQueue));
            Console.WriteLine("\n    Status of child thread:{0}", t.Status);
            t.Start();
            t.Wait();
            Message result = t.Result;
            result.from = endPoint;
            result.author = "Kunal";
            result.type = "RESULTS";
            result.to = msg.from;
            Console.Write("\n  {0} is adding following message to the send-queue:\n", comm.name);
            result.showMsg();
            comm.sndr.PostMessage(result);

            string[] allFiles = Directory.GetFiles(Path.GetFullPath(@"..\\..\\..\\TServer\\ToSend"));
            requiredDLLs = new List<string>();
            Console.WriteLine("\nRequirement:At the end of test execution the Test Harness shall store the test results and logs in the Repository" + 
                "and send test results to the requesting client. It then shall unload the child AppDomain responsible for that testing.");
            foreach (string file in allFiles)
            {
                string name = Path.GetFileName(file);
                uploadFile(name, ToSendPath, SavePath);
            }

                Console.WriteLine("\nCHECKING TO SEE IF THERE ARE ANY REMAINING MESSAGES IN RECEIVE QUEUE");
                if (testQueue.size() == 0)
                {

                    Console.WriteLine("\nNO MORE REQUESTS AT THIS TIME, WAITING FOR MORE..................");
                }

                return msg;
            }

            void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                Console.WriteLine("\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                Console.Write("\n  {0} received the following message from {1}:\n", comm.name, msg.from);
                Console.WriteLine("\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                msg.time = DateTime.Now;
                Console.WriteLine("\n\n Requirement:  Shall implement a Test Harness Server that accepts one or more Test Requests,"
                          + "each in the form of a message" + ",  with XML body that specifies the test developer's identity "
                          + "and the names of a set of one or more test libraries to be tested. Each test driver and the code it will be" +
                          " testing is implemented as a dynamic link library (DLL)");
                
                    msg.showMsg();   

                if (threadpool < maxthreads - 1)
                    {
                        testQueue.enQ(msg);    
                        Console.WriteLine("\nxxxxxxxxxxxxxxxxxxxx Size of common blocking queue: {0}xxxxxxxxxxxxxxxxxxxxx", testQueue.size());
                        threadpool++;
                        utlitity = performTask(msg);
                        threadpool--;
                    }
                    else
                    {
                        utlitity = performTask(msg);
                        threadpool--;
                    }
                }
            }


            void uploadFile(string filename, string ToSendPath, string savePath)
    
            {
                string fqname = Path.Combine(ToSendPath, filename); 
                try
                {
                    hrt.Start();
                    using (var inputStream = new FileStream(fqname, FileMode.Open))
                    {
                        FileTransferMessage msg = new FileTransferMessage();
                        msg.filename = filename;
                        msg.transferStream = inputStream;
                        msg.savePath = savePath;
                        channel.upLoadFile(msg);
                    }
                    hrt.Stop();
                    Console.Write("\n  Uploaded file \"{0}\" in {1} microsec.", filename, hrt.ElapsedMicroseconds);
                }
                catch
                {
                    Console.Write("\n  can't find \"{0}\"", fqname);
                }
            }

        void download(string SavePath, string filename)
        {
            int totalBytes = 0;
            try
            {
                hrt.Start();
                Stream strm = channel.downLoadFile(SavePath, filename);
                string rfilename = Path.Combine(SavePath, filename);
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
                using (var outputStream = new FileStream(rfilename, FileMode.Create))
                {
                    while (true)
                    {
                        int bytesRead = strm.Read(block, 0, BlockSize);
                        totalBytes += bytesRead;
                        if (bytesRead > 0)
                            outputStream.Write(block, 0, bytesRead);
                        else
                            break;
                    }
                }
                hrt.Stop();
                ulong time = hrt.ElapsedMicroseconds;
                Console.Write("\n  Received file \"{0}\" of {1} bytes in {2} microsec.", filename, totalBytes, time);
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}", ex.Message);
            }
        }

        void getFilesFromRepo(string xmlstring)
        {
                //string xmlstring = Server.utlitity.body;
                Xmltest xt = new Xmltest();
                test = xt.parse(xmlstring);
                channel = comm.sndr.CreateSendChannel("http://localhost:8000/IRepo");

                foreach (Test t in test)
                {   //Displaying Test Drivers and Test Code for this test request. 
                    download(SavePath, t.testDriver);
                    Console.WriteLine("\t-> " + t.testDriver);

                    foreach (string test in t.testCode)
                    {
                        Console.WriteLine("\t-> " + test);
                        download(SavePath, test);
                    }
                }
            }


            static void Main(string[] args)
        {
                Console.Write("\n  Testing Server");
                Console.Write("\n =====================\n\n");
                Tserver Server = new Tserver(8);
                Server.wait();
                Console.Write("\n\n");
            }
        }
    }
