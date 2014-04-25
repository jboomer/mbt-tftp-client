using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TFTPClientNameSpace;

namespace TFTPClientTest.Adapter
{
    public class TFTPClientAdapter
    {
        #region Declarations
        static string serverIP="127.0.0.1";
        static int port = 69;
        static string remotePutPath = @"testput.txt";
        static string remoteGetPath = @"testget.txt";
        static string serverPath = @"C:\TFTP\";
        static string localPutPath = @"C:\Users\Jasper\Projects\TFTPClientTest\TestFiles\testput.txt";
        static string localGetPath = @"C:\Users\Jasper\Projects\TFTPClientTest\TestFiles\testget.txt";
        static TFTPClient clnt;
        static BinaryReader localFile;
        static byte[] rcvBuffer;
        static byte[] sndBuffer;
        #endregion

        #region Intitialize
        public static void Initialize()
        {
            clnt = new TFTPClient(serverIP, port);
            rcvBuffer = new byte[516];
            sndBuffer = new byte[516];
            localFile = new BinaryReader(File.Open(localPutPath, FileMode.Open));
        }
        #endregion

        #region Put
        public static void PutCoarseWrapper()
        {
            clnt.Put(remotePutPath, localPutPath);
        }

        public static void PutFineWrapper(int SelMode)
        {
            clnt.Put(remotePutPath, localPutPath, (TFTPClient.Modes)SelMode);
        }

        public static void StartWrite(int SelMode)
        {
            clnt.sendWriteRequest(remotePutPath, localPutPath,(TFTPClient.Modes)SelMode,out sndBuffer);
        }

        public static void ReceiveAck()
        {
            clnt.receiveACK(out rcvBuffer);
        }

        public static void SendDataBlock()
        {
            clnt.sendDataBlock(localFile, out sndBuffer);
        }

        public static void PutExit()
        {
            clnt.canPutExit(sndBuffer.Length);
        }
        #endregion

        #region Get
        public static void GetCoarseWrapper()
        {
            clnt.Get(remoteGetPath, localGetPath);
        }

        public static void GetFineWrapper(int SelMode)
        {
            clnt.Get(remoteGetPath, localGetPath, (TFTPClient.Modes)SelMode);
        }

        public static void StartRead(int SelMode)
        {
            clnt.sendReadRequest(remoteGetPath, localGetPath, (TFTPClient.Modes)SelMode);
        }

        public static void ReceiveDataBlock()
        {
            clnt.receiveDataBlock(out rcvBuffer);
        }

        public static void SendAck()
        {
            clnt.sendACK();
        }

        public static void GetExit()
        {
            clnt.canGetExit(rcvBuffer.Length);
        }
        #endregion
    }
}
