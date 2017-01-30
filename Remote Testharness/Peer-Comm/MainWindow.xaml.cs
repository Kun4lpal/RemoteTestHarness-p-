/////////////////////////////////////////////////////////////////////
// Window1.xaml.cs - WPF User Interface for WCF Communicator       //
// ver 2.2                                                         //
// Kunal Paliwal, CSE681 - Software Modeling & Analysis, Fall 2016 //
/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ====================
 * ver 1.0 : 23 Nob 16
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;
using Interface;
using SWTools;
using CommChannelDemo;
using Communication;
using testHarness;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.ServiceModel;
using Utilities;

namespace WPF_GUI
{
    public partial class Client1 : Window
    {
        public static RoutedCommand ButtonClickCommand = new RoutedCommand();
        Comm<Client1> comm = new Comm<Client1>();

        Message rcvdMsg = new Message();
        int MaxMsgCount = 100;
        private Thread rcvThread = null;

        delegate void NewMessage(Message msg);
        event NewMessage OnNewMessage;
        BlockingQueue<Message> testQueue = new BlockingQueue<Message>();
        BlockingQueue<string> xmlqueue = new BlockingQueue<string>();
        private int BlockSize = 1024;
        private byte[] block;
        private HiResTimer hrt = new HiResTimer();
        private string ToSendPath = "..\\..\\ToSend";
        private string SavePath = "..\\..\\SavedFiles";
        private ICommunicator channel;
        private ICommunicator channel2;
        private Message utlitity = new Message();

        void ThreadProc()
        {
            while (true)
            {
                rcvdMsg = comm.rcvr.GetMessage();
                Console.WriteLine("\n\n");
                rcvdMsg.showMsg();

                this.Dispatcher.BeginInvoke(                   //sends it to the main thread. This is spawning a third
                  System.Windows.Threading.DispatcherPriority.Normal,           //thread so that we return immediately
                  OnNewMessage,
                  rcvdMsg);
            }
        }

        //----< called by UI thread when dispatched from rcvThrd >-------

        void OnNewMessageHandler(Message msg)                        //this is where delegate is pointing to. 
        {

            Console.WriteLine("\n Results should be available in GUI");
            listBox2.Items.Insert(0, msg.body);
            getSavedLogs();
            if (listBox2.Items.Count > MaxMsgCount)
                listBox2.Items.RemoveAt(listBox2.Items.Count - 1);
        }

        //----< subscribe to new message events >------------------------

        public Client1()
        {
            InitializeComponent();
            block = new byte[BlockSize];
            Title = "CLIENT1 VISUAL";
            Console.WriteLine("\n Console of client titled: {0}", Title);

            rcvThread = new Thread(new ThreadStart(this.ThreadProc));
            rcvThread.IsBackground = true;
            rcvThread.Start();

            OnNewMessage += new NewMessage(OnNewMessageHandler);
            ConnectButton.IsEnabled = true;    //set to false- can't connect.
            SendButton.IsEnabled = false;
            //DELETE THIS TEXT TO USE GUI ----------------------------------------------------------------------------
            string remoteAddress = RemoteAddressTextBox.Text;
            string remotePort = RemotePortTextBox.Text;
            string endPoint = Comm<Client1>.makeEndPoint(remoteAddress, remotePort, "IClient1");
            comm.rcvr.CreateRecvChannel(endPoint);
            SendButton.IsEnabled = true;
            channel = comm.sndr.CreateSendChannel("http://localhost:8000/IRepo");
            sendFiles(@"..\..\..\Client\ToSend\");

            string localPort = LocalPortTextBox.Text;
            string endpoint = "http://localhost:" + localPort + "/ITestHarness";  //place where we want to send msg
            try
            {
                BlockingQueue<String> bq = getXmlsAndEnqueue(@"..\..\..\Xmlfilesc1");
                String postString = bq.deQ();
                listBox1.Items.Insert(0, postString);             //CLIENT 1 DEFINING MESSAGE BODY HERE
                Message postMessage = makeMessage("KUNAL PALIWAL", "http://localhost:4000/IClient", endpoint);
                postMessage.body = postString;
                postMessage.from = Comm<Client1>.makeEndPoint(RemoteAddressTextBox.Text, RemotePortTextBox.Text, "IClient1");
                postMessage.time = DateTime.Now;
                postMessage.type = "TEST REQUEST 1";
                comm.sndr.PostMessage(postMessage);
            }
            catch (Exception ex)
            {
                Window temp = new Window();
                temp.Content = ex.Message;
                temp.Height = 100;
                temp.Width = 500;
            }
        }



        public BlockingQueue<string> getXmlsAndEnqueue(string xmlPath)
        {
            xmlqueue = new BlockingQueue<string>();
            if (Directory.Exists(xmlPath))
            {
                string[] fileEntries = Directory.GetFiles(xmlPath);
                Console.WriteLine("\n Accepting Test Requests from Path: {0}  \n", xmlPath);
                foreach (string filename in fileEntries)
                {
                    Console.WriteLine("{0}", System.IO.Path.GetFileName(filename));
                    xmlqueue.enQ(File.ReadAllText(filename));
                }

                Console.WriteLine("\n XML FILE LOADED");
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


        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // REMOVE THESE COMMENTS TO USE BUTTONS

            //string remoteAddress = RemoteAddressTextBox.Text;
            //string remotePort = RemotePortTextBox.Text;
            ////string endpoint = remoteAddress + ":" + remotePort + "/ICommunicator";
            //string endPoint = Comm<Client1>.makeEndPoint(remoteAddress, remotePort);
            //comm.rcvr.CreateRecvChannel(endPoint);
            //SendButton.IsEnabled = true;

            //channel = comm.sndr.CreateSendChannel("http://localhost:8000/IRepo");
            //sendFiles(@"..\..\..\Client\ToSend\");

        }
        //----< send message to connected listener >---------------------

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            // REMOVE THESE COMMENTS TO USE BUTTONS

            //string localPort = LocalPortTextBox.Text;
            //string endpoint = "http://localhost:" + localPort + "/ITestHarness";  //place where we want to send msg
            //try
            //{
            //    BlockingQueue<String> bq = getXmlsAndEnqueue(@"..\..\..\Xmlfilesc1");
            //    String postString = bq.deQ();
            //    listBox1.Items.Insert(0, postString);             //CLIENT 1 DEFINING MESSAGE BODY HERE
            //    Message postMessage = makeMessage("KUNAL PALIWAL", "http://localhost:4000/IClient", endpoint);
            //    postMessage.body = postString;
            //    postMessage.from = Comm<Client1>.makeEndPoint(RemoteAddressTextBox.Text, RemotePortTextBox.Text);
            //    postMessage.time = DateTime.Now;
            //    postMessage.type = "TEST REQUEST 1";
            //    comm.sndr.PostMessage(postMessage);
            //}
            //catch (Exception ex)
            //{
            //    Window temp = new Window();
            //    temp.Content = ex.Message;
            //    temp.Height = 100;
            //    temp.Width = 500;
            //}
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Message postMessage = new Message();
            postMessage.type = "quit";
            comm.sndr.PostMessage(postMessage);
            comm.sndr.Close();
            comm.rcvr.Close();

        }


        void uploadFile(string filename, string ToSendPath, string savePath)

        {
            string fqname = System.IO.Path.Combine(ToSendPath, filename);
            try
            {
                hrt.Start();
                using (var inputStream = new FileStream(System.IO.Path.GetFullPath(fqname), FileMode.Open))
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
                string rfilename = System.IO.Path.Combine(SavePath, filename);
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

        void sendFiles(string path)
        {

            Console.WriteLine("\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            Console.WriteLine("\n CLIENT 1 SIDE:");
            Console.WriteLine("\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            channel = comm.sndr.CreateSendChannel("http://localhost:8000/IRepo");
            string[] allFiles = Directory.GetFiles(System.IO.Path.GetFullPath(path));
            //requiredDLLs = new List<string>();

            Console.WriteLine("\n\n Requirement: DLL files sent by the client to the Repository server" +
                "before sending the Test Request to the Test Harness."
                      + "The Test Request XML body names one or more of these test DLLs to execute");
            foreach (string file in allFiles)
            {
                string name = System.IO.Path.GetFileName(file);
                listBox3.Items.Insert(0, name);
                uploadFile(name, ToSendPath, SavePath);
            }
        }


        public void getSavedLogs()
        {
            string path = @"..\\..\\..\\Repository\\SavedFiles";

            channel2 = comm.sndr.CreateSendChannel("http://localhost:8000/IRepo");
            //string[] allFiles = Directory.GetFiles(System.IO.Path.GetFullPath(path));
            string[] dirs = Directory.GetFiles(path, "*.txt");
            foreach (string name in dirs)
            {
                string filename = System.IO.Path.GetFileName(name);
                download(SavePath, filename);
            }

            Console.WriteLine("\n Requirement: Logs retrieved by client being displayed");

            string path2 = @"..\\..\\SavedFiles";
            string[] dirs2 = Directory.GetFiles(path2,"*.txt");
            foreach (string name in dirs2)
            {
                string filename = System.IO.Path.GetFileName(name);
                listBox4.Items.Insert(0, filename);
            }
           
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            getSavedLogs();

        }
    }

}
