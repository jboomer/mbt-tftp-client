/// <summary>
/// The current states of the TFTP client implementation. 
/// </summary>
public FSM_Modes tftpClientMode;


/// <summary>
/// The 9 states of the TFTP client implementation. They define the state space of the finite state machine mode of the TFTP client.
/// </summary>
public enum FSM_Modes
{
	INIT = 0,
	EXIT = 1,
	ERROR = 2,
	RRQ_SENT = 3,
	DATA_RECEIVED = 4,
	ACK_SENT = 5,
	WRQ_SENT = 6,
	ACK_RECEIVED = 7,
	DATA_SENT = 8,
}

/// <summary>
/// Three different modes in transfering files. This type is one of the parameters of Get, sendReadRequest, Put, sendWriteRequest.
/// </summary>
public enum Modes
{
	NetAscii = 0,
	Octet = 1,
	Mail = 2,
}

/// <summary>
/// The operation codes used for assemblying packets.
/// </summary>
public enum Opcodes
{
	Read = 0,
	Write = 1,
	Data = 2,
	Ack = 3,
	Error = 4,
}





/// <summary>
/// Initialze the TFTP client. Set the state to be INIT
/// </summary>
public void initialize();

/// <summary>
/// Gets the specified remote file. This is a coarse wrapper of the whole downloading track. INIT --> ... -->  EXIT.
/// </summary>
/// <param name="remoteFile">The remote file, e.g. @"rm-1.txt".</param>
/// <param name="localFile">The path to the local file, e.g. @"D:\clientDir\loc-1.txt".</param>
public void Get(string remoteFile, string localFile);

/// <summary>
/// Gets the specified remote file. This is a fine grained wrapper of the downloading track. RRQ_SENT --> ... --> EXIT.
/// </summary>
/// <param name="remoteFile">The remote file.</param>
/// <param name="localFile">The local file.</param>
/// <param name="tftpMode">The TFTP mode: NetAscii, Octet, Mail..</param>
public void Get(string remoteFile, string localFile, Modes tftpMode);

/// <summary>
/// Send Read Request. It creates the TFTP read request packet, send the packet. INIT --> RRQ_SENT.
/// </summary>
/// <param name="remoteFile">The remote file.</param>
/// <param name="localFile">The local file.</param>
/// <param name="tftpMode">The TFTP mode, NetAscii, Octet, Mail.</param>
public void sendReadRequest(string remoteFile, string localFile, Modes tftpMode);

/// <summary>
/// Receive Data Block from the TFTP server. RRQ_SENT or ACK_SENT --> DATA_RECEIVED.
/// </summary>
/// <param name="rcvBuffer">the receiving buffer for return.</param>
/// <returns>
/// A int variable that tell the length of the receiving buffer. 
/// </returns>
public int receiveDataBlock(out byte[] rcvBuffer);

/// <summary>
/// Send ACK packet to the TFTP server, after successfully receive the data block from the TFTP server. DATA_RECEIVED --> ACK_SENT.
/// </summary>
public void sendACK();

/// <summary>
/// check whether it is time to successfully exit. If yes, current state --> EXIT; otherwise, keep the current state.
/// </summary>
/// <param name="len">The length of receving buffer.</param>
/// <returns>
/// A bool variable indicating whether it is time to successfully exit. When len < 516, it returns TRUE for successfully exit. When len >= 516, it returns FALSE for staying in the loop and continuing the file transfer. 
/// </returns>
public bool canGetExit(int len);

/// <summary>
/// Upload the specified remote file. This is a coarse wrapper of the whole uploading track. INIT --> ... --> EXIT.
/// </summary>
/// <param name="remoteFile">The remote file, e.g. @"rm-1.txt".</param>
/// <param name="localFile">The path to the local file, e.g. @"D:\clientDir\loc-1.txt".</param>
public void Put(string remoteFile, string localFile);

/// <summary>
/// Uploading the specified local file to be the remote file in the server side. This is a detail wrapper of the whole uploading track. WRQ_SENT --> EXIT.
/// </summary>
/// <param name="remoteFile">The name of the remote file, e.g. @"rm-1.txt".</param>
/// <param name="localFile">The path to the local file, e.g. @"D:\clientDir\loc-1.txt".</param>
/// <param name="tftpMode">The TFTP mode: NetAscii, Octet, Mail.</param>
public void Put(string remoteFile, string localFile, Modes tftpMode);

/// <summary>
/// Send Write Request. Create the WRQ packet, send it to the server. INIT --> WRQ_SENT.
/// </summary>
/// <param name="remoteFile">The remote file.</param>
/// <param name="localFile">The local file.</param>
/// <param name="tftpMode">The TFTP mode.</param>
/// <param name="sndBuffer">The returned sending buffer.</param>
public void sendWriteRequest(string remoteFile, string localFile, Modes tftpMode, out byte[] sndBuffer);

/// <summary>
/// Send Data Block. ACK_RECEIVED --> DATA_SENT.
/// </summary>
/// <param name="fileStream">The file stream for writting the downloading file.</param>
/// <param name="sndBuffer">The returned sending buffer.</param>
public void sendDataBlock(BinaryReader fileStream, out byte[] sndBuffer);

/// <summary>
/// Receive ACK from the server. WRQ_SENT or DATA_SENT --> ACK_RECEIVED
/// </summary>
/// <param name="rcvBuffer">The receiving buffer.</param>
/// <returns>
/// A int variable that tell the length of the receiving buffer. 
/// </returns>
public int receiveACK(out byte[] rcvBuffer);

/// <summary>
/// check whether it is time to successfully exit. If yes, current state --> EXIT; otherwise, keep the current state unchanged. 
/// </summary>
/// <param name="len">The length of the sending buffer.</param>
/// <returns>
/// A bool variable indicating whether it is time to successfully exit. When len < 516, it returns TRUE for successfully exit. When len >= 516, it returns FALSE for staying in the loop and continuing the file transfer. 
/// </returns>
public bool canPutExit(int len);
