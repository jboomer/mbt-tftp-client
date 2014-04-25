using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Modeling;
using TFTPClientTest.Adapter;

namespace TFTPClientTest.Models
{
    /// <summary>
    /// An example model program.
    /// </summary>
    public static class ClientModel
    {
        #region States
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

        public enum Modes
        {
            NetAscii = 0,
            Octet = 1,
            Mail = 2,
        }

        static FSM_Modes modelState;
        static Modes transferMode;
        static bool isSndBufferFull;
        static bool isRcvBufferFull;
        static bool canExit;

        #endregion

        #region Probes
        [Probe]
        public static string Description()
        {
            return string.Format("{0}{1}", modelState,Bufferinfo());
        }

        static string Bufferinfo()
        {
            string info = "";
            if (isRcvBufferFull)
            {
                info +=":R=full";
            }
            if (isSndBufferFull)
            {
                info += ":,S=full";
            }
            if (canExit)
            {
                info += ":Exit ready";
            }
            return info;
        }

        #endregion

        
        #region Rule Methods
        
        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.GetCoarseWrapper()")]
        static void GetCoarseWrapper()
        {
            IsInState(FSM_Modes.INIT);
            transferMode = Modes.Octet;
            modelState = FSM_Modes.EXIT;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.GetExit()")]
        static void GetExit()
        {
            IsInState(FSM_Modes.DATA_RECEIVED);
            canExit = Choice.Some<bool>();
            if (canExit)
            {
                modelState = FSM_Modes.EXIT;
            }
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.GetFineWrapper(SelMode)")]
        static void GetFineWrapper(int SelMode)
        {
            Condition.Equals(modelState, FSM_Modes.RRQ_SENT);
            transferMode = (Modes)SelMode;
            modelState = FSM_Modes.EXIT;

        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.Initialize()")]
        static void Initialize()
        {
            IsInState(FSM_Modes.EXIT);
            canExit = false;
            isRcvBufferFull = false;
            isSndBufferFull = false;
            modelState = FSM_Modes.INIT;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.PutCoarseWrapper()")]
        static void PutCoarseWrapper()
        {
            IsInState(FSM_Modes.INIT);
            transferMode = Modes.Octet;
            modelState = FSM_Modes.EXIT;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.PutExit()")]
        static void PutExit()
        {
            IsInState(FSM_Modes.ACK_RECEIVED);
            canExit = Choice.Some<bool>();
            if (canExit)
            {
                modelState = FSM_Modes.EXIT;
            }
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.PutFineWrapper(SelMode)")]
        static void PutFineWrapper(int SelMode)
        {
            IsInState(FSM_Modes.RRQ_SENT);
            transferMode = (Modes)SelMode;
            modelState = FSM_Modes.EXIT;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.ReceiveAck()")]
        static void ReceiveAck()
        {
            IsInState(FSM_Modes.WRQ_SENT, FSM_Modes.DATA_SENT);
            isRcvBufferFull = Choice.Some<bool>();
            modelState = FSM_Modes.ACK_RECEIVED;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.ReceiveDataBlock()")]
        static void ReceiveDataBlock()
        {
            IsInState(FSM_Modes.RRQ_SENT, FSM_Modes.ACK_SENT);
            isRcvBufferFull = Choice.Some<bool>();
            modelState = FSM_Modes.DATA_RECEIVED;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.SendAck()")]
        static void SendAck()
        {
            IsInState(FSM_Modes.DATA_RECEIVED);
            modelState = FSM_Modes.ACK_SENT;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.SendDataBlock()")]
        static void SendDataBlock()
        {
            IsInState(FSM_Modes.ACK_RECEIVED);
            modelState = FSM_Modes.DATA_SENT;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.StartRead(SelMode)")]
        static void StartRead(int SelMode)
        {
            IsInState(FSM_Modes.INIT);
            transferMode = (Modes)SelMode;
            modelState = FSM_Modes.RRQ_SENT;
        }

        [Rule(Action = "TFTPClientTest.Adapter.TFTPClientAdapter.StartWrite(SelMode)")]
        static void StartWrite(int SelMode)
        {
            IsInState(FSM_Modes.INIT);
            transferMode = (Modes)SelMode;
            modelState = FSM_Modes.WRQ_SENT;
        } 
        
        #endregion

        #region HelperFunctions

        static void IsInState(FSM_Modes state)
        {
            Condition.Equals(modelState, state);
        }

        static void IsInState(FSM_Modes state1, FSM_Modes state2)
        {
            Condition.IsTrue(modelState == state1 || modelState == state2);
        }

        

        #endregion

    }
}
