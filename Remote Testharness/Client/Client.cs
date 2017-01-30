using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWTools;
using Interface;
using System.IO;
using CommChannelDemo;

namespace client
{
    public class Client
    {
        private static BlockingQueue<string> xmlqueue;
        private static Message fm = new Message();
        public Client()
        {
            //constructor  
        }
        // Reading all xmls and storing data into a string and queueing
        public BlockingQueue<string> getXmlsAndEnqueue(string xmlPath)
        {
            xmlqueue = new BlockingQueue<string>();
            if (Directory.Exists(xmlPath))
            {
                string[] fileEntries = Directory.GetFiles(xmlPath);
                Console.WriteLine("\n Accepting Test Requests from Path: {0}  \n", xmlPath);
                foreach (string filename in fileEntries)
                {
                    Console.WriteLine("{0}", Path.GetFileName(filename));
                    xmlqueue.enQ(File.ReadAllText(filename));
                }
                Console.WriteLine("\n Enqueueing..................................");
                Console.WriteLine("\n Size of Blockingqueue: {0}", xmlqueue.size());
            }
            else
            {
                Console.WriteLine("No XML was found at the path specified: {0} \n", xmlPath);
            }
            return xmlqueue;
        }

        public Message makeMessage(string author, string fromEndPoint, string toEndPoint)
        {
            Message msg = new Message();
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            return msg;
        }

        static void Main(string[] args)
        {
            
        }
    }
}
