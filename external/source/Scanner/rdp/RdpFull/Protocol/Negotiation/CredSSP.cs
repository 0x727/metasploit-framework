using System;
using System.Text;

namespace SharpRDPCheck
{
    internal class CredSSP : ASN1
    {
        private  bool m_bAuthenticated = false;
        private  byte[] m_ChallengeMsg;
        private  NTLM m_NTLMAuthenticate;
        private Network m_Network = null;
        private Options m_Options = null;

        public CredSSP(Network network, Options options)
        {
            m_Network = network;
            m_Options = options;
        }

        public  void Negotiate(byte[] ServerPublicKey)
        {
            try
            {
                base.Init();
                m_bAuthenticated = false;
                SendNegotiate();

                //NTLM.DumpHex(ServerPublicKey, ServerPublicKey.Length, "Server Public Key");

                while (!m_bAuthenticated)
                {
                    ProcesssResponse(Receive(), ServerPublicKey);
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Authentication Error (" + exception.Message + ")", exception.InnerException);
            }
        }

        private  void SendNegotiate()
        {
            SendTSRequest(WriteNegoToken(), null, null);
        }

        private  void SendTSRequest(RdpPacket negoTokens, byte[] auth_info, byte[] pub_key_auth)
        {
            RdpPacket packet = new RdpPacket();
            base.WriteTag(packet, base.SequenceTag(0), "TSRequest");
            base.WriteTag(packet, base.ContextTag(0), "CTX_Version");
            base.WriteInteger(packet, 2);
            base.CloseTag(packet, "CTX_Version");

            if (negoTokens != null)
            {
                base.WriteTag(packet, base.ContextTag(1), "CTX_NegTokens");
                base.WriteTag(packet, base.SequenceTag(0), "NegTokens");
                base.WriteTag(packet, base.SequenceTag(0), "NegTokens2");
                base.WriteTag(packet, base.ContextTag(0), "CTX_OctetString");
                base.WriteTag(packet, base.OctetStringTag(), "OctetString");
                packet.copyToByteArray(negoTokens);
                base.CloseTag(packet, "OctetString");
                base.CloseTag(packet, "CTX_OctetString");
                base.CloseTag(packet, "NegTokens2");
                base.CloseTag(packet, "NegTokens");
                base.CloseTag(packet, "CTX_NegTokens");
            }

            if (auth_info != null)
            {
                base.WriteTag(packet, base.ContextTag(2), "CTX_AuthInfo");
                base.WriteTag(packet, base.OctetStringTag(), "OctetString");
                packet.Write(auth_info, 0, auth_info.Length);
                base.CloseTag(packet, "OctetString");
                base.CloseTag(packet, "CTX_AuthInfo");
            }

            if (pub_key_auth != null)
            {
                base.WriteTag(packet, base.ContextTag(3), "CTX_PubKeyAuth");
                base.WriteTag(packet, base.OctetStringTag(), "OctetString");
                packet.Write(pub_key_auth, 0, pub_key_auth.Length);
                base.CloseTag(packet, "OctetString");
                base.CloseTag(packet, "CTX_PubKeyAuth");
            }

            base.CloseTag(packet, "TSRequest");

            Send(packet);
        }

        private  void ProcesssResponse(RdpPacket packet, byte[] ServerPublicKey)
        {
            base.ReadTag(packet, base.SequenceTag(0), "TSRequest");
            base.ReadTag(packet, base.ContextTag(0), "CTX_Version");

            if (base.ReadInteger(packet) < 2)
            {
                throw new Exception("TSRequest version not supported!");
            }

            base.CloseTag(packet, "CTX_Version");
            byte[] buffer = null;
            int num2 = base.ReadTag(packet, "Tag");

            if (num2 == base.ContextTag(1))
            {
                base.ReadTag(packet, base.SequenceTag(0), "NegTokens");
                base.ReadTag(packet, base.SequenceTag(0), "NegTokens2");
                base.ReadTag(packet, base.ContextTag(0), "CTX_OctetString");
                m_ChallengeMsg = new byte[base.ReadTag(packet, base.OctetStringTag(), "OctetString")];
                packet.Read(m_ChallengeMsg, 0, m_ChallengeMsg.Length);
                base.CloseTag(packet, "OctetString");
                base.CloseTag(packet, "CTX_OctetString");
                base.CloseTag(packet, "NegTokens2");
                base.CloseTag(packet, "NegTokens");
            }
            else if (num2 == base.ContextTag(3))
            {
                buffer = new byte[base.ReadTag(packet, base.OctetStringTag(), "OctetString")];
                packet.Read(buffer, 0, buffer.Length);
                base.CloseTag(packet, "OctetString");
            }

            base.CloseTag(packet, "Tag");
            base.CloseTag(packet, "TSRequest");

            if (buffer != null)
            {
                byte[] buffer2 = m_NTLMAuthenticate.DecryptMessage(buffer);
                buffer2[0] = (byte) (buffer2[0] - 1);
                if (!NTLM.CompareArray(buffer2, ServerPublicKey))
                {
                    throw new Exception("Unable to verify the server's public key!");
                }
                buffer2[0] = (byte) (buffer2[0] + 1);
                SendTSRequest(null, WriteTSCredentials(), null);
                m_bAuthenticated = true;
            }
            else
            {
                ReadNegoToken(m_ChallengeMsg, ServerPublicKey);
            }
        }

        private  void ReadNegoToken(byte[] Challenge, byte[] ServerPublicKey)
        {
            RdpPacket negoTokens = new RdpPacket();
            byte[] buffer = m_NTLMAuthenticate.ProcessChallenge(Challenge);
            negoTokens.Write(buffer, 0, buffer.Length);
            negoTokens.Position = 0L;
            byte[] buffer2 = m_NTLMAuthenticate.EncryptMessage(ServerPublicKey);

            SendTSRequest(negoTokens, null, buffer2);
        }

        private  RdpPacket Receive()
        {
            byte[] buffer = new byte[0x2000];
            int length = m_Network.Receive(buffer);
            NTLM.DumpHex(buffer, length, "Receive");
            RdpPacket packet = new RdpPacket();
            packet.Write(buffer, 0, length);
            packet.Position = 0L;

            return packet;
        }

        private  void Send(RdpPacket packet)
        {
            packet.Position = 0L;
            byte[] buffer = new byte[packet.Length];
            packet.Read(buffer, 0, (int) packet.Length);
            NTLM.DumpHex(buffer, (int) packet.Length, "Send");

            m_Network.Send(buffer);
        }
        
        public  RdpPacket WriteNegoToken()
        {
            m_NTLMAuthenticate = new NTLM(m_Network.OpenSocket, m_Options.Username, m_Options.Password, m_Options.Domain);
            byte[] buffer = m_NTLMAuthenticate.Negotiate();
            RdpPacket packet = new RdpPacket();
            packet.Write(buffer, 0, buffer.Length);
            packet.Position = 0L;

            return packet;
        }

        private  byte[] WriteTSCredentials()
        {
            RdpPacket packet = new RdpPacket();
            base.WriteTag(packet, base.SequenceTag(0), "SEQ_TSCRED");
            base.WriteTag(packet, base.ContextTag(0), "CTX_credType");
            base.WriteInteger(packet, 1);
            base.CloseTag(packet, "CTX_credType");
            base.WriteTag(packet, base.ContextTag(1), "CTX_credentials");
            base.WriteTag(packet, base.OctetStringTag(), "CTX_OctetString");
            base.WriteTag(packet, base.SequenceTag(0), "SEQ_Credentials");
            base.WriteTag(packet, base.ContextTag(0), "CTX_domain");
            base.WriteTag(packet, base.OctetStringTag(), "OctectString");
            byte[] bytes = Encoding.Unicode.GetBytes(m_Options.Domain);
            packet.Write(bytes, 0, bytes.Length);
            base.CloseTag(packet, "OctectString");
            base.CloseTag(packet, "CTX_domain");
            base.WriteTag(packet, base.ContextTag(1), "CTX_user");
            base.WriteTag(packet, base.OctetStringTag(), "OctectString");
            byte[] buffer = Encoding.Unicode.GetBytes(m_Options.Username);
            packet.Write(buffer, 0, buffer.Length);
            base.CloseTag(packet, "OctectString");
            base.CloseTag(packet, "CTX_user");
            base.WriteTag(packet, base.ContextTag(2), "CTX_password");
            base.WriteTag(packet, base.OctetStringTag(), "OctectString");
            byte[] buffer3 = Encoding.Unicode.GetBytes(m_Options.Password);
            packet.Write(buffer3, 0, buffer3.Length);
            base.CloseTag(packet, "OctectString");
            base.CloseTag(packet, "CTX_password");
            base.CloseTag(packet, "SEQ_Credentials");
            base.CloseTag(packet, "CTX_OctetString");
            base.CloseTag(packet, "CTX_credentials");
            base.CloseTag(packet, "SEQ_TSCRED");
            byte[] buffer4 = new byte[packet.Length];
            packet.Position = 0L;
            packet.Read(buffer4, 0, buffer4.Length);

            return m_NTLMAuthenticate.EncryptMessage(buffer4);
        }

    }
}