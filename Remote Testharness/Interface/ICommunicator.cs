/////////////////////////////////////////////////////////////////////
// ICommunicator.cs - Peer-To-Peer Communicator Service Contract   //
// ver 2.0                                                         //
// Jim Fawcett, CSE681 - Software Modeling & Analysis, Summer 2011 //
/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ====================
 * ver 2.0 : 10 Oct 11
 * - removed [OperationContract] from GetMessage() so only local client
 *   can dequeue messages
 * ver 1.0 : 14 Jul 07
 * - first release
 */

using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;
using CommChannelDemo;
using System.IO;

namespace Interface
{
    [ServiceContract]
    public interface ICommunicator
    {
        [OperationContract(IsOneWay = true)]
        void PostMessage(Message msg);

        [OperationContract]
        Message GetMessage();

        [OperationContract(IsOneWay = true)]
        void upLoadFile(FileTransferMessage msg);

        [OperationContract]
        Stream downLoadFile(string savePath, string filename);

    }

    [MessageContract]
    public class FileTransferMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string filename { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string savePath { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream transferStream { get; set; }


    }

}
