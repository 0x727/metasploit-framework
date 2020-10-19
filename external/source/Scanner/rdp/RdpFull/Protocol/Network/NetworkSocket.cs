using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SharpRDPCheck
{
    internal class NetworkSocket
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private SslStream sslStream;
        private string m_sSocketName;
        private byte[] m_SSLPublicKey;
        private object lockObj = new object();
        private PacketLogger m_Logger = null;

        public void Close()
        {
            if (sslStream != null)
            {
                try
                {
                    sslStream.Flush();
                    sslStream.Close();
                    sslStream.Dispose();
                }
                catch { }
            }

            if (networkStream != null)
            {
                try
                {
                    networkStream.Flush();
                    networkStream.Close();
                    networkStream.Dispose();
                }
                catch { }
            }

            if (tcpClient != null)
            {
                try
                {
                    tcpClient.Close();
                }
                catch { }
            }

            m_sSocketName = "";
            m_SSLPublicKey = null;
        }

        public NetworkSocket(string sSocketName, PacketLogger logger)
        {
            this.m_sSocketName = sSocketName;
            m_Logger = logger;
        }

        public bool TryConnect(string host, int port)
        {
            TcpClient tcp = new TcpClient();
            IAsyncResult async = tcp.BeginConnect(IPAddress.Parse(host), port, null, null);
            async.AsyncWaitHandle.WaitOne(Options.SocketTimeout);
            if (async.IsCompleted)
            {
                //Console.WriteLine($"{host}:{port} is open.");
                tcp.EndConnect(async);
                return true;
            }
            else
            {
                //Console.WriteLine($"{host}:{port} is closed.");
                return false;
            }
        }
    
        public bool Connect(string host, int port)
        {
            if (!TryConnect(host, port)) return false;
            // TcpClient
            tcpClient = new TcpClient();
            tcpClient.SendTimeout = Options.SocketTimeout;
            tcpClient.ReceiveTimeout = Options.SocketTimeout;
            tcpClient.Connect(IPAddress.Parse(host), port);




            // NetworkStream
            networkStream = tcpClient.GetStream();
            networkStream.WriteTimeout = Options.SocketTimeout;
            networkStream.ReadTimeout = Options.SocketTimeout;

            return true;
        }
       
        public void ConnectSSL(string host)
        {
            
            sslStream = new SslStream(networkStream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate));
            sslStream.WriteTimeout = Options.SocketTimeout;
            sslStream.ReadTimeout = Options.SocketTimeout;

            
            sslStream.AuthenticateAsClient(host);
        }

        
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            
            m_SSLPublicKey = certificate.GetPublicKey();

            m_Logger.SSLPublicKey(m_SSLPublicKey, m_sSocketName);

            return true;
        }
        
        
        public void AddBlob(PacketLogger.PacketType type, byte[] data)
        {
            m_Logger.AddBlob(type, data, m_sSocketName);
        }

        public byte[] GetBlob(PacketLogger.PacketType type)
        {
            return m_Logger.GetBlob(type, m_sSocketName);
        }

        public byte[] GetSSLPublicKey()
        {
            return m_SSLPublicKey;
        }

        protected int InternalReceive(byte[] buffer, int offset, int size)
        {
            return networkStream.Read(buffer, offset, size);
        }

        protected int InternalSend(byte[] buffer, int offset, int size)
        {
            networkStream.Write(buffer, offset, size);
            return buffer.Length;
        }

        public int Receive(byte[] buffer, int size)
        {
            return Receive(buffer, 0, size);
        }

        public int Receive(byte[] buffer, int offset, int size)
        {
            int len = -1;
            if (sslStream != null)
            {
                len = sslStream.Read(buffer, offset, size);
            }
            else if (networkStream != null || len == -1)
            {
                len = InternalReceive(buffer, offset, size);
            }
            return len;
        }

        public int Send(byte[] buffer)
        {
            lock (lockObj)
            {
                if (sslStream != null)
                {
                    sslStream.Write(buffer, 0, buffer.Length);
                    sslStream.Flush();
                    return buffer.Length;
                }
            }
            return InternalSend(buffer, 0, buffer.Length);
        }


    }
}