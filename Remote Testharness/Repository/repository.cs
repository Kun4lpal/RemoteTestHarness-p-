/////////////////////////////////////////////////////////////////////
// Repository.cs - Demonstrate application use of channel              //
// Ver 1.0                                                          //
// Kunal Paliwal, CSE681 - Software Modeling and Analysis,Fall 2016 //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This is the main communication package of the repository. It communicates with the clients and the testharness server
 * using WCF. Messages are sent and received in data format and filestream.
 * 
 * Required Files:
 * ---------------
 * ICommunicator
 * Utilities- HRTIMER
 * 
 * Maintenance History:
 * --------------------
 * Ver 1.0 : 23 Nov 2016
 * - first release 
 *  
 */
using System;
using System.IO;
using System.ServiceModel;
using Interface;
using Utilities;
using System.Threading;
using SWTools;
using Communication;
using CommChannelDemo;

namespace Repository
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]

    class repository : MarshalByRefObject, IRepository
    {
        public Comm<repository> comm { get; set; } = new Comm<repository>();

        int BlockSize = 1024;
        byte[] block;
        HiResTimer hrt = null;
        
        ICommunicator channel;
        public string endPoint { get; } = "http://localhost:8000/IRepo";
        ServiceHost host;

        repository()
        {

            block = new byte[BlockSize];
            hrt = new HiResTimer();
            host = comm.rcvr.CreateRecvChannel(endPoint);
        }
        
        public void uploadFile(string filename, string ToSendPath, string SavePath)
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
                    msg.savePath = SavePath;
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

        public void download(string filename, string SavePath)
        {
            int totalBytes = 0;
            try
            {
                hrt.Start();
                Stream strm = comm.rcvr.downLoadFile(filename, SavePath);
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
        
        static void Main()
        {
            Console.Write("\n  REPOSITORY");
            Console.Write("\n ==========================================");
            Console.WriteLine("\nRequirement: Test libraries and Test Requests are sent"+  
                "to the Repository and Test Harness server,"+ 
                "respectively, and results sent back to a requesting client, using an asynchronous"+ 
                " message-passing communication channel." 
                +"The Test Harness receives test libraries from the Repository using the same communication processing."+ 
                "File transfer shall use streams or a chunking file transfer that does not depend on enqueuing messages.\n");
            repository clnt = new repository();
            Console.Write("\n\n  Press key to terminate repository");
            Console.ReadKey();
            Console.Write("\n\n");
        }
    }

}


