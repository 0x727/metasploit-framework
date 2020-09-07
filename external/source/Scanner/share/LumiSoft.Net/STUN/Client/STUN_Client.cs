using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using LumiSoft.Net.STUN.Message;

namespace LumiSoft.Net.STUN.Client
{
	// Token: 0x02000028 RID: 40
	public class STUN_Client
	{
		// Token: 0x0600013B RID: 315 RVA: 0x00008C88 File Offset: 0x00007C88
		public static STUN_Result Query(string host, int port, IPEndPoint localEP)
		{
			bool flag = host == null;
			if (flag)
			{
				throw new ArgumentNullException("host");
			}
			bool flag2 = localEP == null;
			if (flag2)
			{
				throw new ArgumentNullException("localEP");
			}
			STUN_Result result;
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
			{
				socket.Bind(localEP);
				result = STUN_Client.Query(host, port, socket);
			}
			return result;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00008CF8 File Offset: 0x00007CF8
		public static STUN_Result Query(string host, int port, Socket socket)
		{
			bool flag = host == null;
			if (flag)
			{
				throw new ArgumentNullException("host");
			}
			bool flag2 = socket == null;
			if (flag2)
			{
				throw new ArgumentNullException("socket");
			}
			bool flag3 = port < 1;
			if (flag3)
			{
				throw new ArgumentException("Port value must be >= 1 !");
			}
			bool flag4 = socket.ProtocolType != ProtocolType.Udp;
			if (flag4)
			{
				throw new ArgumentException("Socket must be UDP socket !");
			}
			IPEndPoint remoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(host)[0], port);
			STUN_Result result;
			try
			{
				STUN_Message stun_Message = STUN_Client.DoTransaction(new STUN_Message
				{
					Type = STUN_MessageType.BindingRequest
				}, socket, remoteEndPoint, 1600);
				bool flag5 = stun_Message == null;
				if (flag5)
				{
					result = new STUN_Result(STUN_NetType.UdpBlocked, null);
				}
				else
				{
					STUN_Message stun_Message2 = new STUN_Message();
					stun_Message2.Type = STUN_MessageType.BindingRequest;
					stun_Message2.ChangeRequest = new STUN_t_ChangeRequest(true, true);
					bool flag6 = socket.LocalEndPoint.Equals(stun_Message.MappedAddress);
					if (flag6)
					{
						STUN_Message stun_Message3 = STUN_Client.DoTransaction(stun_Message2, socket, remoteEndPoint, 1600);
						bool flag7 = stun_Message3 != null;
						if (flag7)
						{
							result = new STUN_Result(STUN_NetType.OpenInternet, stun_Message.MappedAddress);
						}
						else
						{
							result = new STUN_Result(STUN_NetType.SymmetricUdpFirewall, stun_Message.MappedAddress);
						}
					}
					else
					{
						STUN_Message stun_Message4 = STUN_Client.DoTransaction(stun_Message2, socket, remoteEndPoint, 1600);
						bool flag8 = stun_Message4 != null;
						if (flag8)
						{
							result = new STUN_Result(STUN_NetType.FullCone, stun_Message.MappedAddress);
						}
						else
						{
							STUN_Message stun_Message5 = STUN_Client.DoTransaction(new STUN_Message
							{
								Type = STUN_MessageType.BindingRequest
							}, socket, stun_Message.ChangedAddress, 1600);
							bool flag9 = stun_Message5 == null;
							if (flag9)
							{
								throw new Exception("STUN Test I(II) dind't get resonse !");
							}
							bool flag10 = !stun_Message5.MappedAddress.Equals(stun_Message.MappedAddress);
							if (flag10)
							{
								result = new STUN_Result(STUN_NetType.Symmetric, stun_Message.MappedAddress);
							}
							else
							{
								STUN_Message stun_Message6 = STUN_Client.DoTransaction(new STUN_Message
								{
									Type = STUN_MessageType.BindingRequest,
									ChangeRequest = new STUN_t_ChangeRequest(false, true)
								}, socket, stun_Message.ChangedAddress, 1600);
								bool flag11 = stun_Message6 != null;
								if (flag11)
								{
									result = new STUN_Result(STUN_NetType.RestrictedCone, stun_Message.MappedAddress);
								}
								else
								{
									result = new STUN_Result(STUN_NetType.PortRestrictedCone, stun_Message.MappedAddress);
								}
							}
						}
					}
				}
			}
			finally
			{
				while (DateTime.Now.AddMilliseconds(200.0) > DateTime.Now)
				{
					bool flag12 = socket.Poll(1, SelectMode.SelectRead);
					if (flag12)
					{
						byte[] buffer = new byte[512];
						socket.Receive(buffer);
					}
				}
			}
			return result;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00008FA0 File Offset: 0x00007FA0
		public static IPAddress GetPublicIP(string stunServer, int port, IPAddress localIP)
		{
			bool flag = stunServer == null;
			if (flag)
			{
				throw new ArgumentNullException("stunServer");
			}
			bool flag2 = stunServer == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'stunServer' value must be specified.");
			}
			bool flag3 = port < 1;
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'port' value.");
			}
			bool flag4 = localIP == null;
			if (flag4)
			{
				throw new ArgumentNullException("localIP");
			}
			bool flag5 = !Net_Utils.IsPrivateIP(localIP);
			IPAddress result;
			if (flag5)
			{
				result = localIP;
			}
			else
			{
				STUN_Result stun_Result = STUN_Client.Query(stunServer, port, Net_Utils.CreateSocket(new IPEndPoint(localIP, 0), ProtocolType.Udp));
				bool flag6 = stun_Result.PublicEndPoint != null;
				if (!flag6)
				{
					throw new IOException("Failed to STUN public IP address. STUN server name is invalid or firewall blocks STUN.");
				}
				result = stun_Result.PublicEndPoint.Address;
			}
			return result;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00009060 File Offset: 0x00008060
		public static IPEndPoint GetPublicEP(string stunServer, int port, Socket socket)
		{
			bool flag = stunServer == null;
			if (flag)
			{
				throw new ArgumentNullException("stunServer");
			}
			bool flag2 = stunServer == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'stunServer' value must be specified.");
			}
			bool flag3 = port < 1;
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'port' value.");
			}
			bool flag4 = socket == null;
			if (flag4)
			{
				throw new ArgumentNullException("socket");
			}
			bool flag5 = socket.ProtocolType != ProtocolType.Udp;
			if (flag5)
			{
				throw new ArgumentException("Socket must be UDP socket !");
			}
			IPEndPoint remoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(stunServer)[0], port);
			IPEndPoint sourceAddress;
			try
			{
				STUN_Message stun_Message = STUN_Client.DoTransaction(new STUN_Message
				{
					Type = STUN_MessageType.BindingRequest
				}, socket, remoteEndPoint, 1000);
				bool flag6 = stun_Message == null;
				if (flag6)
				{
					throw new IOException("Failed to STUN public IP address. STUN server name is invalid or firewall blocks STUN.");
				}
				sourceAddress = stun_Message.SourceAddress;
			}
			catch
			{
				throw new IOException("Failed to STUN public IP address. STUN server name is invalid or firewall blocks STUN.");
			}
			finally
			{
				while (DateTime.Now.AddMilliseconds(200.0) > DateTime.Now)
				{
					bool flag7 = socket.Poll(1, SelectMode.SelectRead);
					if (flag7)
					{
						byte[] buffer = new byte[512];
						socket.Receive(buffer);
					}
				}
			}
			return sourceAddress;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000091B8 File Offset: 0x000081B8
		private void GetSharedSecret()
		{
		}

		// Token: 0x06000140 RID: 320 RVA: 0x000091BC File Offset: 0x000081BC
		private static STUN_Message DoTransaction(STUN_Message request, Socket socket, IPEndPoint remoteEndPoint, int timeout)
		{
			byte[] buffer = request.ToByteData();
			while (DateTime.Now.AddMilliseconds((double)timeout) > DateTime.Now)
			{
				try
				{
					socket.SendTo(buffer, remoteEndPoint);
					bool flag = socket.Poll(500000, SelectMode.SelectRead);
					if (flag)
					{
						byte[] array = new byte[512];
						socket.Receive(array);
						STUN_Message stun_Message = new STUN_Message();
						stun_Message.Parse(array);
						bool flag2 = Net_Utils.CompareArray(request.TransactionID, stun_Message.TransactionID);
						if (flag2)
						{
							return stun_Message;
						}
					}
				}
				catch
				{
				}
			}
			return null;
		}
	}
}
