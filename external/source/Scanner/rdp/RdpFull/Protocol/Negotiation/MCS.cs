using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpRDPCheck
{
    public class MCS
    {
        public readonly int DPUM = 8;
        public readonly int SDIN = 0x1a;
        public readonly int SDRQ = 0x19;
        public readonly int MCS_USERCHANNEL_BASE = 0x3e9; // 1001
        public readonly int MSC_GLOBAL_CHANNEL = 0x3eb; // 1003
        private List<int> serverSupportedChannels = new List<int>();
        private Network m_Network = null;
        private Options m_Options = null;
        private CredSSP m_CredSSP = null;

        public MCS()
        {
            if (m_Network == null)
            {
                m_Network = new Network();
            }

            if (m_Options == null)
            {
                m_Options = new Options();
            }

            if (m_CredSSP == null)
            {
                m_CredSSP = new CredSSP(m_Network, m_Options);
            }
        }

        public void Dispose()
        {
            try
            {
                if (m_Network != null)
                {
                    m_Network.Dispose();
                    m_Network = null;
                }
            }
            catch { }
        }

        public Options options
        {
            get
            {
                return m_Options;
            }
        }

        public bool Connect(string host, int port)
        {
            return m_Network.Connect(host, port);
        }

        public void sendConnectionRequest(byte[] loadBalanceToken, bool bAutoReconnect)
        {
            int num;
            m_Network.ConnectionStage = Network.eConnectionStage.Negotiating;

            if (m_Options.enableNLA)
            {
                // Client X.224 Connection Request PDU

                sendConnectNegotiation(
                    NegotiationProtocol.PROTOCOL_RDP |
                    NegotiationProtocol.PROTOCOL_SSL |
                    NegotiationProtocol.PROTOCOL_HYBRID,
                    loadBalanceToken);

                // Server X.224 Connection Confirm PDU
                num = receiveConnectNegotiation();


                if (((num & 1) != 0) || ((num & 2) != 0))
                {
                    m_Network.ConnectionStage = Network.eConnectionStage.Securing;
                    m_Network.ConnectSSL(m_Options.Host);
                }

                if ((num & 2) != 0)
                {
                    m_Network.ConnectionStage = Network.eConnectionStage.Authenticating;
                    m_CredSSP.Negotiate(m_Network.GetSSLPublicKey());
                }
                else
                {
                    throw new Exception("Authentication Error: Server doesn't support PROTOCOL_HYBRID");
                }
                
            }
            else
            {
                // Client X.224 Connection Request PDU
                sendConnectNegotiation(NegotiationProtocol.PROTOCOL_RDP, loadBalanceToken);

                // Server X.224 Connection Confirm PDU
                num = receiveConnectNegotiation();

                if (num != 0)
                {
                    throw new RDFatalException("Security negotiation failed!");
                }
            }
            //if(m_Options.hash.Length>0)
            //    Console.WriteLine("[+] Valid:" + m_Options.Username + "  " + m_Options.hash);
            //else
            //    Console.WriteLine("[+] Valid:" + m_Options.Username + "  " + m_Options.Password);

        }

        
        /// <summary>
        /// Client X.224 Connection Request PDU
        /// </summary>
        private  void sendConnectNegotiation(NegotiationProtocol NegotiationFlags, byte[] loadBalanceToken)
        {
            string domainAndUsername = m_Options.DomainAndUsername;

            if (domainAndUsername.Length > 9)
            {
                domainAndUsername = domainAndUsername.Substring(0, 9);
            }

            RdpPacket packet = new RdpPacket();
            packet.WriteByte(3);
            packet.WriteByte(0);
            long position = packet.Position;
            packet.WriteBigEndian16((short)0);
            packet.WriteByte(0);
            packet.WriteByte(0xe0);
            packet.WriteBigEndian16((short)0);
            packet.WriteBigEndian16((short)0);
            packet.WriteByte(0);

            if (loadBalanceToken != null)
            {
                packet.Write(loadBalanceToken, 0, loadBalanceToken.Length);
                packet.WriteString("\r\n", false);
            }
            else
            {
                packet.WriteString("Cookie: mstshash=" + domainAndUsername + "\r\n", true);
            }

            // RDP Negotiation Request
            packet.WriteByte(0x01);
            packet.WriteByte(0);
            packet.WriteLittleEndian16((short)8);
            packet.WriteLittleEndian32((int)NegotiationFlags); // Standard RDP Security, TLS 1.0, CredSSP

            long num2 = packet.Position;
            packet.Position = position;
            packet.WriteBigEndian16((short)num2);
            packet.WriteByte((byte)(num2 - 5L));

            IsoLayerWrite(packet);
        }

        internal void IsoLayerWrite(RdpPacket data)
        {
            data.Position = 0L;
            byte[] buffer = new byte[data.Length];
            data.Read(buffer, 0, (int)data.Length);

            m_Network.Send(buffer);
        }

        internal RdpPacket ISOReceive()
        {
            byte[] buffer = new byte[0x3000];
            int count = m_Network.Receive(buffer);
            RdpPacket packet = new RdpPacket();
            packet.Write(buffer, 0, count);
            packet.Position = 0L;
            int num2 = 0;

            if (packet.ReadByte() == 3)
            {
                packet.ReadByte();
                num2 = packet.ReadBigEndian16();
                long position = packet.Position;

                while (num2 > count)
                {
                    int num4 = m_Network.Receive(buffer);
                    packet.Position = count;
                    packet.Write(buffer, 0, num4);
                    count += num4;
                }

                packet.Position = position;

                return packet;
            }
            num2 = packet.ReadByte();

            if ((num2 & 0x80) != 0)
            {
                num2 &= -129;
                num2 = num2 << (8 + packet.ReadByte());
            }

            return packet;
        }

        /// <summary>
        /// Server X.224 Connection Confirm PDU
        /// </summary>
        private  int receiveConnectNegotiation()
        {
            RdpPacket packet = ISOReceive();
            packet.Position += 7L;

            if (packet.Position >= packet.Length)
            {
                return 0;
            }

            switch (packet.ReadByte())
            {
                // TYPE_RDP_NEG_RSP
                case 0x02:
                    m_Options.serverNegotiateFlags = (NegotiationFlags)packet.ReadByte();
                    packet.ReadLittleEndian16();
                    return packet.ReadLittleEndian32();

                // TYPE_RDP_NEG_FAILURE
                case 0x03:
                    packet.ReadByte();
                    packet.ReadLittleEndian16();

                    switch ((NegotiationFailureCodes)packet.ReadLittleEndian32())
                    {
                        case NegotiationFailureCodes.SSL_REQUIRED_BY_SERVER:
                            throw new RDFatalException("The server requires that the client support Enhanced RDP Security with TLS 1.0");

                        case NegotiationFailureCodes.SSL_NOT_ALLOWED_BY_SERVER:
                            return 0x10000000;

                        case NegotiationFailureCodes.SSL_CERT_NOT_ON_SERVER:
                            throw new RDFatalException("The server does not possess a valid authentication certificate and cannot initialize the External Security Protocol Provider");

                        case NegotiationFailureCodes.INCONSISTENT_FLAGS:
                            throw new RDFatalException("The list of requested security protocols is not consistent with the current security protocol in effect.");

                        case NegotiationFailureCodes.HYBRID_REQUIRED_BY_SERVER:
                            throw new RDFatalException("The server requires that the client support Enhanced RDP Security with CredSSP");

                        case NegotiationFailureCodes.SSL_WITH_USER_AUTH_REQUIRED_BY_SERVER:
                            throw new RDFatalException("The server requires that the client support Enhanced RDP Security and certificate-based client authentication");
                    }

                    throw new RDFatalException("Unknown Negotiation failure!");
            }

            throw new RDFatalException("Negotiation failed, requested security level not supported by server.");
        }
        
        [Flags]
        private enum NegotiationProtocol
        {
            PROTOCOL_RDP = 0x00000000,
            PROTOCOL_SSL = 0x00000001,
            PROTOCOL_HYBRID = 0x00000002
        }

        [Flags]
        public enum NegotiationFlags
        {
            EXTENDED_CLIENT_DATA_SUPPORTED = 0x01,
            DYNVC_GFX_PROTOCOL_SUPPORTED = 0x02,
            NEGRSP_FLAG_RESERVED = 0x04,
            RESTRICTED_ADMIN_MODE_SUPPORTED = 0x08
        }
    
        [Flags]
        private enum NegotiationFailureCodes
        {
            SSL_REQUIRED_BY_SERVER = 0x00000001,
            SSL_NOT_ALLOWED_BY_SERVER = 0x00000002,
            SSL_CERT_NOT_ON_SERVER = 0x00000003,
            INCONSISTENT_FLAGS = 0x00000004,
            HYBRID_REQUIRED_BY_SERVER = 0x00000005,
            SSL_WITH_USER_AUTH_REQUIRED_BY_SERVER = 0x00000006
        }
    }
}