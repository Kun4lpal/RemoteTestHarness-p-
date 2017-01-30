/////////////////////////////////////////////////////////////////////
// Communication.svc.cs - Peer-To-Peer WCF Communicator            //
// ver 1                                                         //
// Kunal Paliwal, CSE681 - Software Modeling & Analysis, Summer 2011 //
/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ====================
 * ver 1.0 : 23 Nov 16
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using SWTools;
using Interface;
using CommChannelDemo;
using System.IO;
using Utilities;

namespace Communication
{
    /////////////////////////////////////////////////////////////
    // Receiver hosts Communication service used by other Peers

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Receiver : ICommunicator
    {
        public string name { get; set; }
        static BlockingQueue<Message> rcvBlockingQ = null;      //this queue is shared.
        ServiceHost service = null;

        public string filename { get; private set; }
        public string savePath { get; private set; }

        public ICommunicator channel { get; set; }

        byte[] block;
        int BlockSize = 1024;
        HiResTimer hrt = null;

        public Receiver()
        {
            block = new byte[BlockSize];
            hrt = new HiResTimer();
            if (rcvBlockingQ == null)
                rcvBlockingQ = new BlockingQueue<Message>();
        }

        public void Close()
        {
            service.Close();
        }


        public Stream downLoadFile(string ToSendPath, string filename)
        {
            hrt.Start();
            string sfilename = Path.Combine(ToSendPath, filename);
            FileStream outStream = null;
            if (File.Exists(sfilename))
            {
                outStream = new FileStream(sfilename, FileMode.Open);
            }
            else
                throw new Exception("open failed for \"" + filename + "\"");
            hrt.Stop();
            Console.Write("\n  Sent \"{0}\" in {1} microsec.", filename, hrt.ElapsedMicroseconds);
            return outStream;
        }


        //  Create ServiceHost for Communication service

        public ServiceHost CreateRecvChannel(string address)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(Receiver), baseAddress);
            service.AddServiceEndpoint(typeof(ICommunicator), binding, baseAddress);
            service.Open();
            return service;
        }

        // Implement service method to receive messages from other Peers

        public void PostMessage(Message msg)
        {
            rcvBlockingQ.enQ(msg);
        }

        // Implement service method to extract messages from other Peers.
        // This will often block on empty queue, so user should provide
        // read thread.

        public Message GetMessage()
        {
            return rcvBlockingQ.deQ();
        }

        public void upLoadFile(FileTransferMessage msg)
        {
            int totalBytes = 0;
            hrt.Start();
            filename = msg.filename;
            savePath = msg.savePath;
            string rfilename = Path.Combine(savePath, filename);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            using (var outputStream = new FileStream(rfilename, FileMode.Create))
            {
                while (true)
                {
                    int bytesRead = msg.transferStream.Read(block, 0, BlockSize);
                    totalBytes += bytesRead;
                    if (bytesRead > 0)
                        outputStream.Write(block, 0, bytesRead);
                    else
                        break;
                }
            }
            hrt.Stop();

            
            Console.Write(
              "\n  Received file \"{0}\" of {1} bytes in {2} microsec.",
              filename, totalBytes, hrt.ElapsedMicroseconds
            );
        }

    }
    ///////////////////////////////////////////////////
    // client of another Peer's Communication service

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Sender 
    {
        public string name { get; set; }
        public string filename { get; private set; }
        public string savePath { get; private set; }

        public ICommunicator channel { get; set; }
        string lastError = "";
        BlockingQueue<Message> sndBlockingQ = null;                           //only 1 guy using this queue
        Thread sndThrd = null;
        int tryCount = 0, MaxCount = 10;                  //retries in case he cant connect.
        string currEndpoint = "";

                HiResTimer hrt = null;
        private byte[] block;
        int BlockSize = 1024;

        // Processing for sndThrd to pull msgs out of sndBlockingQ
        // and post them to another Peer's Communication service



        void ThreadProc()
        {
            tryCount = 0;
            while (true)
            {
                Message msg = sndBlockingQ.deQ();
                if (msg.to != currEndpoint)
                {
                    currEndpoint = msg.to;
                    CreateSendChannel(currEndpoint);   // THIS IS WHERE MESSAGE IS SENT AND POSTED 
                }                                                        // TO THE RECEIVE CHANNEL OF SERVER
                while (true)
                {
                    try
                    {
                        channel.PostMessage(msg);
                        Console.Write("\nJUST NOW posted message from {0} to {1}\n",msg.from, msg.to);
                        tryCount = 0;
                        break;
                    }
                    catch
                    {
                        Console.Write("\n  connection failed\n");
                        if (++tryCount < MaxCount)
                            Thread.Sleep(100);
                        else
                        {
                            Console.Write("\n  {0}", "can't connect\n");
                            currEndpoint = "";
                            tryCount = 0;
                            break;
                        }
                    }
                }
                if (msg.body == "quit")
                    break;
            }
        }


        // Create Communication channel proxy, sndBlockingQ, and
        // start sndThrd to send messages that client enqueues

        public Sender() 
        {
            block = new byte[BlockSize];
            hrt = new HiResTimer();
            sndBlockingQ = new BlockingQueue<Message>();
            sndThrd = new Thread(ThreadProc);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }

                             //THIS is THE MAIN PART. FROM HERE REQ IS SENT.
        
        // Create proxy to another Peer's Communicator

        public ICommunicator CreateSendChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<ICommunicator> factory
              = new ChannelFactory<ICommunicator>(binding, address);
            return channel = factory.CreateChannel();
        }

        // Sender posts message to another Peer's queue using
        // Communication service hosted by receipient via sndThrd

        public void PostMessage(Message msg)
        {
            sndBlockingQ.enQ(msg);
        }

        public string GetLastError()
        {
            string temp = lastError;
            lastError = "";
            return temp;
        }

        public void Close()
        {
            ChannelFactory<ICommunicator> temp = (ChannelFactory<ICommunicator>)channel;
            temp.Close();
        }

        
        public Message GetMessage()
        {
            throw new NotImplementedException();
        }

        public Stream downLoadFile(string savePath, string filename)
        {
            throw new NotImplementedException();
        }
    }
    public class Comm <Message>
    {
        public string name { get; set; } = typeof(Message).Name;

        public Receiver rcvr = new Receiver();

        public Sender sndr = new Sender();

        public Comm()
        {
            rcvr.name = name;
            sndr.name = name;
        }
        public static string makeEndPoint(string url, string port, string servername)
        {
            string endPoint = url + ":" + port + "/" + servername;
            return endPoint;
        }

        public static string makeEndPoint(string v1, int v2)
        {
            throw new NotImplementedException();
        }
    }
}
