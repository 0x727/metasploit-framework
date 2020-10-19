using System;
using System.Threading;

namespace SharpRDPCheck
{
    class Network
    {        
        private  eConnectionStage m_ConnectionStage = eConnectionStage.None;
        private PacketLogger m_Logger = null;
        private NetworkSocket m_OpenSocket = null;
        delegate void ConnectionStageChangedHandler();
         event ConnectionStageChangedHandler ConnectionStageChanged;

        public Network()
        {
            if (m_Logger == null)
                m_Logger = new PacketLogger();
        }

        public void Dispose()
        {
            try
            {
                if (m_Logger != null)
                {
                    m_Logger.Dispose();
                    m_Logger = null;
                }
            }
            catch { }

            Close();
        }

        public void Close()
        {
            try
            {
                if (m_OpenSocket != null)
                {
                    m_OpenSocket.Close();
                    m_OpenSocket = null;
                }
            }
            catch { }
        }

        public bool Connect(string host, int port)
        {
            bool bReturn = false;
            ConnectionStage = eConnectionStage.Connecting;
            try
            {
                NetworkSocket socket = new NetworkSocket(host.Replace(".", ""), m_Logger);
                bReturn = socket.Connect(host, port);
                if (bReturn)
                    m_OpenSocket = socket;
            }
            catch { }

            return bReturn;
        }

        public void ConnectSSL(string host)
        {
            m_OpenSocket.ConnectSSL(host);
        } 

        public  byte[] GetSSLPublicKey()
        {
            return m_OpenSocket.GetSSLPublicKey();
        }

        public  int Receive(byte[] buffer)
        {
            return Receive(buffer, buffer.Length);
        }

        public  int Receive(byte[] buffer, int size)
        {
            return m_OpenSocket.Receive(buffer, size);
        }

        public  int Send(byte[] buffer)
        {
            return m_OpenSocket.Send(buffer);
        }


        public eConnectionStage ConnectionStage
        {
            get
            {
                return m_ConnectionStage;
            }
            set
            {
                eConnectionStage connectionStage = m_ConnectionStage;
                m_ConnectionStage = value;
                if ((connectionStage != value) && (ConnectionStageChanged != null))
                {
                    ConnectionStageChanged();
                }
            }
        }

        public  PacketLogger Logger
        {
            get
            {
                return m_Logger;
            }
            set
            {
                m_Logger = value;
            }
        }

        public  NetworkSocket OpenSocket
        {
            get
            {
                return m_OpenSocket;
            }
        }


        public enum eConnectionStage
        {
            None,
            Connecting,
            ConnectingToGateway,
            ConnectingToHost,
            Negotiating,
            Securing,
            Authenticating,
            Establishing,
            Login,
            Reconnecting,
            SecureAndLogin
        }

    }
}