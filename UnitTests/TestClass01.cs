using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using TFTPClientNameSpace;        

namespace TestSuite01
{
    /// <summary>
    /// Source file for generating Nunit tests
    /// Testing 0-switch coverage of the TFTP client as well as BVA on the
    /// buffersize for exit
    /// </summary>
    

    [TestFixture]
    class TestInitExit
    {
        /// <summary>
        /// Test if client is initialized and can exit at correct size of the buffer(<516)
        /// </summary>

        #region declarations
        string tftpIP = "127.0.0.1";
        TFTPClient client;
        #endregion

        /// <summary>
        /// Initialize a tftp client with server on localhost
        /// </summary>
        [SetUp]
        public void Init()
        {
            client = new TFTPClient(tftpIP, 69); //Initialize
        }

        /// <summary>
        /// Assert the state is INIT
        /// </summary>
        [Test]
        public void TestInitialized()
        {
            Assert.AreEqual(TFTPClient.FSM_Modes.INIT, client.tftpClientMode);
        }

        /// <summary>
        /// Assert "Get" cannot exit with a rcvBuffer of length>516
        /// </summary>
        [Test]
        public void TestGetExitFalse()
        {
            client.canGetExit(520);
            Assert.AreNotEqual(TFTPClient.FSM_Modes.EXIT, client.tftpClientMode);
        }
        
        /// <summary>
        /// Assert "Get" can exit with a rcvBuffer of length<516
        /// </summary>
        [Test]
        public void TestGetExitTrue()
        {
            client.canGetExit(3);
            Assert.AreEqual(TFTPClient.FSM_Modes.EXIT, client.tftpClientMode);
        }

        /// <summary>
        /// Assert "Get" cannot exit with a rcvBuffer of length=516
        /// </summary>
        [Test]
        public void TestGetExitBoundaryHigh()
        {
            client.canGetExit(516);
            Assert.AreNotEqual(TFTPClient.FSM_Modes.EXIT, client.tftpClientMode);
        }

        /// <summary>
        /// Assert "Get" can exit with a rcvBuffer of length=515
        /// </summary>
        [Test]
        public void TestGetExitBoundaryLow()
        {
            client.canGetExit(515);
            Assert.AreEqual(TFTPClient.FSM_Modes.EXIT, client.tftpClientMode);
        }

        /// <summary>
        /// Assert "Put" cannot exit with a sndBuffer of length>516
        /// </summary>
        [Test]
        public void TestPutExitFalse()
        {
            client.canPutExit(520);
            Assert.AreNotEqual(TFTPClient.FSM_Modes.EXIT, client.tftpClientMode);
        }

        /// <summary>
        /// Assert "Put" can exit with a sndBuffer of length<516
        /// </summary>
        [Test]
        public void TestPutExitTrue()
        {
            client.canPutExit(3);
            Assert.AreEqual(TFTPClient.FSM_Modes.EXIT, client.tftpClientMode);
        }

        /// <summary>
        /// Assert "Put" cannot exit with a sndBuffer of length=516
        /// </summary>
        [Test]
        public void TestPutExitBoundaryHigh()
        {
            client.canPutExit(516);
            Assert.AreNotEqual(TFTPClient.FSM_Modes.EXIT, client.tftpClientMode);
        }

        /// <summary>
        /// Assert "Put" can exit with a sndBuffer of length=515
        /// </summary>
        [Test]
        public void TestPutExitBoundaryLow()
        {
            client.canPutExit(515);
            Assert.AreEqual(TFTPClient.FSM_Modes.EXIT, client.tftpClientMode);
        }
    }

    [TestFixture]
    class TestGet
    {
        /// <summary>
        /// Test "Get" method
        /// </summary>
        
        #region declarations
        string remotePath = @"testget.txt";
        string serverPath = @"C:\TFTP\";
        string destPath = @"C:\Users\Jasper\Projects\TFTPClientTest\TestFiles\testget.txt";
        string tftpIP = "127.0.0.1";
        TFTPClient client;
        byte[] rcvBuffer;
        #endregion

        /// <summary>
        /// Create tftp client and empty receive buffer
        /// </summary>
        [SetUp]
        public void Init()
        {
            client = new TFTPClient(tftpIP, 69); //Initialize
            rcvBuffer = new byte[516];
        }

        /// <summary>
        /// Delete destination testfile
        /// </summary>
        [TearDown]
        public void Destr()
        {
            File.Delete(destPath);
        }

        /// <summary>
        /// Send read request and get remotefile(RRQ_Sent->EXIT)
        /// Assert files are equal
        /// </summary>
        [Test]
        public void TestGetFineWrapper()
        {
            client.sendReadRequest(remotePath, destPath, TFTPClient.Modes.Octet);
            client.Get(remotePath, destPath, TFTPClient.Modes.Octet);
            FileAssert.AreEqual(serverPath + remotePath, destPath);
        }

        /// <summary>
        /// "Get" method (INIT->EXIT)
        /// Assert files are equal
        /// </summary>
        [Test]
        public void TestGetCoarseWrapper()
        {
            client.Get(remotePath, destPath);
            FileAssert.AreEqual(serverPath + remotePath, destPath);
        }

        /// <summary>
        /// Send read request
        /// Assert client state = RRQ_SENT
        /// </summary>
        [Test]
        public void TestReadRequest()
        {
            client.sendReadRequest(remotePath, destPath, TFTPClient.Modes.Octet);
            Assert.AreEqual(TFTPClient.FSM_Modes.RRQ_SENT, client.tftpClientMode);
        }

        /// <summary>
        /// Send read request and receive data block
        /// Assert client state = DATA_RECEIVED
        /// </summary>
        [Test]
        public void TestReceiveDataBlock()
        {
            client.sendReadRequest(remotePath, destPath, TFTPClient.Modes.Octet);
            client.receiveDataBlock(out rcvBuffer);
            Assert.AreEqual(TFTPClient.FSM_Modes.DATA_RECEIVED, client.tftpClientMode); //received a block? Nog testen lengte receiveBuffer
        }

        /// <summary>
        /// Send read request and receive data block
        /// Assert that receive buffer is not empty
        /// </summary>
        [Test]
        public void TestReceiveContent()
        {
            client.sendReadRequest(remotePath, destPath, TFTPClient.Modes.Octet);
            client.receiveDataBlock(out rcvBuffer);
            Assert.IsFalse(rcvBuffer.Length == 0); //Non-empty receive buffer
        }

        /// <summary>
        /// Send read request, receive data block and send send acknowledgement
        /// Assert client state is ACK_SENT
        /// </summary>
        [Test]
        public void TestSendACK()
        {
            client.sendReadRequest(remotePath, destPath, TFTPClient.Modes.Octet);
            client.receiveDataBlock(out rcvBuffer);
            client.sendACK();
            Assert.AreEqual(TFTPClient.FSM_Modes.ACK_SENT, client.tftpClientMode);
        }

        /// <summary>
        /// Receive another data block after sending first acknowledgement
        /// Assert client state is DATA_RECEIVED
        /// </summary>
        [Test]
        public void TestReceiveUponAck()
        {
            client.sendReadRequest(remotePath, destPath, TFTPClient.Modes.Octet);
            client.receiveDataBlock(out rcvBuffer);
            client.sendACK();
            client.receiveDataBlock(out rcvBuffer);
            Assert.AreEqual(TFTPClient.FSM_Modes.DATA_RECEIVED, client.tftpClientMode);
        }

        /// <summary>
        /// Send whole file block by block
        /// Assert files are equal
        /// </summary>
        [Test]
        public void TestReceiveMultipleBlocks()
        {
            client.sendReadRequest(remotePath, destPath, TFTPClient.Modes.Octet);
            while (client.receiveDataBlock(out rcvBuffer) >= 516)
            {
                client.sendACK();
            }
            FileAssert.AreEqual(serverPath + remotePath, destPath);
        }

    }

    [TestFixture]
    class TestPut
    {
        /// <summary>
        /// Test "Put" method
        /// </summary>

        #region declarations
        string remotePath = @"testput.txt";
        string serverPath = @"C:\TFTP\";
        string localPath = @"C:\Users\Jasper\Projects\TFTPClientTest\TestFiles\testput.txt";
        string tftpIP = "127.0.0.1";
        TFTPClient client;
        byte[] sndBuffer;
        byte[] rcvBuffer;
        BinaryReader localFile;
        #endregion

        /// <summary>
        /// Create tftp client, empty send and receive buffers and filestream for the local file
        /// </summary>
        [SetUp]
        public void Init()
        {
            client = new TFTPClient(tftpIP, 69); //Initialize
            sndBuffer = new byte[516];
            rcvBuffer = new byte[516];
            localFile = new BinaryReader(File.Open(localPath, FileMode.Open));
        }
        
        /// <summary>
        /// Delete the transferred file and close the local file
        /// </summary>
        [TearDown]
        public void Destr()
        {
            File.Delete(serverPath + remotePath); //Delete the created file(if any)
            localFile.Close(); //Close BinaryStream
        }
        
        /// <summary>
        /// Test "Put" method (INIT->EXIT)
        /// Assert files are equal
        /// </summary>
        [Test]
        public void TestPutCoarseWrapper()
        {
            client.Put(remotePath, localPath);
            FileAssert.AreEqual(localPath,serverPath + remotePath);
        }

        /// <summary>
        /// Test "Put" overloaded method (WRQ_SENT->EXIT)
        /// Assert files are equal
        /// </summary>
        [Test]
        public void TestPutFineWrapper()
        {
            client.sendWriteRequest(remotePath, localPath, TFTPClient.Modes.Octet, out sndBuffer);
            client.Put(remotePath, localPath, TFTPClient.Modes.Octet);
            FileAssert.AreEqual(localPath, serverPath + remotePath);
        }

        /// <summary>
        /// Send write request
        /// Assert client state = WRQ_SENT
        /// </summary>
        [Test]
        public void TestWriteRequest()
        {
            client.sendWriteRequest(remotePath, localPath, TFTPClient.Modes.Octet, out sndBuffer);
            Assert.AreEqual(TFTPClient.FSM_Modes.WRQ_SENT, client.tftpClientMode);
        }

        /// <summary>
        /// Send write request and receive acknowledgement
        /// Assert client state = ACK_RECEIVED
        /// </summary>
        [Test]
        public void TestWriteAcknowledge()
        {
            client.sendWriteRequest(remotePath, localPath, TFTPClient.Modes.Octet, out sndBuffer);
            client.receiveACK(out rcvBuffer);
            Assert.AreEqual(TFTPClient.FSM_Modes.ACK_RECEIVED, client.tftpClientMode);
        }

        /// <summary>
        /// Send write request and receive acknowledgement
        /// Assert receive buffer is not empty
        /// </summary>
        [Test]
        public void TestReturnContent()
        {
            client.sendWriteRequest(remotePath, localPath, TFTPClient.Modes.Octet, out sndBuffer);
            Assert.IsFalse(client.receiveACK(out rcvBuffer) == 0);
        }

        /// <summary>
        /// Send write request, receive acknowledgement and send datablock
        /// Assert client state = DATA_SENT
        /// </summary>
        [Test]
        public void TestSendDataBlock()
        {
            client.sendWriteRequest(remotePath, localPath, TFTPClient.Modes.Octet, out sndBuffer);
            client.receiveACK(out rcvBuffer);
            client.sendDataBlock(localFile, out sndBuffer);
            Assert.AreEqual(TFTPClient.FSM_Modes.DATA_SENT, client.tftpClientMode);
        }

        /// <summary>
        /// Send write request, receive acknowledgement, send data block and
        /// receive acknowledgement
        /// Assert client state = ACK_RECEIVED
        /// </summary>
        [Test]
        public void TestAcknowledgeSend()
        {
            client.sendWriteRequest(remotePath, localPath, TFTPClient.Modes.Octet, out sndBuffer);
            client.receiveACK(out rcvBuffer);
            client.sendDataBlock(localFile, out sndBuffer);
            client.receiveACK(out rcvBuffer);
            Assert.AreEqual(TFTPClient.FSM_Modes.ACK_RECEIVED, client.tftpClientMode);
        }

        /// <summary>
        /// Send whole file block by block
        /// Assert files are equal
        /// </summary>
        [Test]
        public void WriteMultipleBlocks()
        {
            client.sendWriteRequest(remotePath, localPath, TFTPClient.Modes.Octet, out sndBuffer);
            client.receiveACK(out rcvBuffer);
            while (sndBuffer.Length >= 516)
            {
                client.sendDataBlock(localFile, out sndBuffer);
                client.receiveACK(out rcvBuffer);
            }
            FileAssert.AreEqual(localPath, serverPath + remotePath);
        }
    }
}
