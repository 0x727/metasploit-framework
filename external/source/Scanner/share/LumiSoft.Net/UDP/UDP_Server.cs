using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace LumiSoft.Net.UDP
{
	// Token: 0x02000130 RID: 304
	public class UDP_Server : IDisposable
	{
		// Token: 0x06000C0A RID: 3082 RVA: 0x00049E80 File Offset: 0x00048E80
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = false;
				this.Stop();
				this.Error = null;
				this.PacketReceived = null;
			}
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x00049EB8 File Offset: 0x00048EB8
		public void Start()
		{
			bool isRunning = this.m_IsRunning;
			if (!isRunning)
			{
				this.m_IsRunning = true;
				this.m_StartTime = DateTime.Now;
				this.m_pDataReceivers = new List<UDP_DataReceiver>();
				bool flag = this.m_pBindings != null;
				if (flag)
				{
					List<IPEndPoint> list = new List<IPEndPoint>();
					foreach (IPEndPoint ipendPoint in this.m_pBindings)
					{
						bool flag2 = ipendPoint.Address.Equals(IPAddress.Any);
						if (flag2)
						{
							IPEndPoint item = new IPEndPoint(IPAddress.Loopback, ipendPoint.Port);
							bool flag3 = !list.Contains(item);
							if (flag3)
							{
								list.Add(item);
							}
							foreach (IPAddress address in Dns.GetHostAddresses(""))
							{
								IPEndPoint item2 = new IPEndPoint(address, ipendPoint.Port);
								bool flag4 = !list.Contains(item2);
								if (flag4)
								{
									list.Add(item2);
								}
							}
						}
						else
						{
							bool flag5 = !list.Contains(ipendPoint);
							if (flag5)
							{
								list.Add(ipendPoint);
							}
						}
					}
					this.m_pSockets = new List<Socket>();
					foreach (IPEndPoint localEP in list)
					{
						try
						{
							Socket socket = Net_Utils.CreateSocket(localEP, ProtocolType.Udp);
							this.m_pSockets.Add(socket);
							for (int k = 0; k < this.m_ReceiversPerSocket; k++)
							{
								UDP_DataReceiver udp_DataReceiver = new UDP_DataReceiver(socket);
								udp_DataReceiver.PacketReceived += delegate(object s, UDP_e_PacketReceived e)
								{
									try
									{
										this.ProcessUdpPacket(e);
									}
									catch (Exception x2)
									{
										this.OnError(x2);
									}
								};
								udp_DataReceiver.Error += delegate(object s, ExceptionEventArgs e)
								{
									this.OnError(e.Exception);
								};
								this.m_pDataReceivers.Add(udp_DataReceiver);
								udp_DataReceiver.Start();
							}
						}
						catch (Exception x)
						{
							this.OnError(x);
						}
					}
					this.m_pSendSocketsIPv4 = new CircleCollection<Socket>();
					this.m_pSendSocketsIPv6 = new CircleCollection<Socket>();
					foreach (Socket socket2 in this.m_pSockets)
					{
						bool flag6 = ((IPEndPoint)socket2.LocalEndPoint).AddressFamily == AddressFamily.InterNetwork;
						if (flag6)
						{
							bool flag7 = !((IPEndPoint)socket2.LocalEndPoint).Address.Equals(IPAddress.Loopback);
							if (flag7)
							{
								this.m_pSendSocketsIPv4.Add(socket2);
							}
						}
						else
						{
							bool flag8 = ((IPEndPoint)socket2.LocalEndPoint).AddressFamily == AddressFamily.InterNetworkV6;
							if (flag8)
							{
								this.m_pSendSocketsIPv6.Add(socket2);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0004A1BC File Offset: 0x000491BC
		public void Stop()
		{
			bool flag = !this.m_IsRunning;
			if (!flag)
			{
				this.m_IsRunning = false;
				foreach (UDP_DataReceiver udp_DataReceiver in this.m_pDataReceivers)
				{
					udp_DataReceiver.Dispose();
				}
				this.m_pDataReceivers = null;
				foreach (Socket socket in this.m_pSockets)
				{
					socket.Close();
				}
				this.m_pSockets = null;
				this.m_pSendSocketsIPv4 = null;
				this.m_pSendSocketsIPv6 = null;
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0004A294 File Offset: 0x00049294
		public void Restart()
		{
			bool isRunning = this.m_IsRunning;
			if (isRunning)
			{
				this.Stop();
				this.Start();
			}
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0004A2BC File Offset: 0x000492BC
		public void SendPacket(byte[] packet, int offset, int count, IPEndPoint remoteEP)
		{
			IPEndPoint ipendPoint = null;
			this.SendPacket(packet, offset, count, remoteEP, out ipendPoint);
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0004A2DC File Offset: 0x000492DC
		public void SendPacket(byte[] packet, int offset, int count, IPEndPoint remoteEP, out IPEndPoint localEP)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("UdpServer");
			}
			bool flag = !this.m_IsRunning;
			if (flag)
			{
				throw new InvalidOperationException("UDP server is not running.");
			}
			bool flag2 = packet == null;
			if (flag2)
			{
				throw new ArgumentNullException("packet");
			}
			bool flag3 = remoteEP == null;
			if (flag3)
			{
				throw new ArgumentNullException("remoteEP");
			}
			localEP = null;
			this.SendPacket(null, packet, offset, count, remoteEP, out localEP);
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0004A358 File Offset: 0x00049358
		public void SendPacket(IPEndPoint localEP, byte[] packet, int offset, int count, IPEndPoint remoteEP)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("UdpServer");
			}
			bool flag = !this.m_IsRunning;
			if (flag)
			{
				throw new InvalidOperationException("UDP server is not running.");
			}
			bool flag2 = packet == null;
			if (flag2)
			{
				throw new ArgumentNullException("packet");
			}
			bool flag3 = localEP == null;
			if (flag3)
			{
				throw new ArgumentNullException("localEP");
			}
			bool flag4 = remoteEP == null;
			if (flag4)
			{
				throw new ArgumentNullException("remoteEP");
			}
			bool flag5 = localEP.AddressFamily != remoteEP.AddressFamily;
			if (flag5)
			{
				throw new ArgumentException("Argumnet localEP and remoteEP AddressFamily won't match.");
			}
			Socket socket = null;
			bool flag6 = localEP.AddressFamily == AddressFamily.InterNetwork;
			if (flag6)
			{
				foreach (Socket socket2 in this.m_pSendSocketsIPv4.ToArray())
				{
					bool flag7 = localEP.Equals((IPEndPoint)socket2.LocalEndPoint);
					if (flag7)
					{
						socket = socket2;
						break;
					}
				}
			}
			else
			{
				bool flag8 = localEP.AddressFamily == AddressFamily.InterNetworkV6;
				if (!flag8)
				{
					throw new ArgumentException("Argument 'localEP' has unknown AddressFamily.");
				}
				foreach (Socket socket3 in this.m_pSendSocketsIPv6.ToArray())
				{
					bool flag9 = localEP.Equals((IPEndPoint)socket3.LocalEndPoint);
					if (flag9)
					{
						socket = socket3;
						break;
					}
				}
			}
			bool flag10 = socket == null;
			if (flag10)
			{
				throw new ArgumentException("Specified local end point '" + localEP + "' doesn't exist.");
			}
			IPEndPoint ipendPoint = null;
			this.SendPacket(socket, packet, offset, count, remoteEP, out ipendPoint);
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0004A4F4 File Offset: 0x000494F4
		internal void SendPacket(Socket socket, byte[] packet, int offset, int count, IPEndPoint remoteEP, out IPEndPoint localEP)
		{
			bool flag = socket == null;
			if (flag)
			{
				bool flag2 = remoteEP.AddressFamily == AddressFamily.InterNetwork;
				if (flag2)
				{
					bool flag3 = this.m_pSendSocketsIPv4.Count == 0;
					if (flag3)
					{
						throw new ArgumentException("There is no suitable IPv4 local end point in this.Bindings.");
					}
					socket = this.m_pSendSocketsIPv4.Next();
				}
				else
				{
					bool flag4 = remoteEP.AddressFamily == AddressFamily.InterNetworkV6;
					if (!flag4)
					{
						throw new ArgumentException("Invalid remote end point address family.");
					}
					bool flag5 = this.m_pSendSocketsIPv6.Count == 0;
					if (flag5)
					{
						throw new ArgumentException("There is no suitable IPv6 local end point in this.Bindings.");
					}
					socket = this.m_pSendSocketsIPv6.Next();
				}
			}
			socket.SendTo(packet, 0, count, SocketFlags.None, remoteEP);
			localEP = (IPEndPoint)socket.LocalEndPoint;
			this.m_BytesSent += (long)count;
			this.m_PacketsSent += 1L;
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x0004A5D4 File Offset: 0x000495D4
		public IPEndPoint GetLocalEndPoint(IPEndPoint remoteEP)
		{
			bool flag = remoteEP == null;
			if (flag)
			{
				throw new ArgumentNullException("remoteEP");
			}
			bool flag2 = remoteEP.AddressFamily == AddressFamily.InterNetwork;
			IPEndPoint result;
			if (flag2)
			{
				bool flag3 = this.m_pSendSocketsIPv4.Count == 0;
				if (flag3)
				{
					throw new InvalidOperationException("There is no suitable IPv4 local end point in this.Bindings.");
				}
				result = (IPEndPoint)this.m_pSendSocketsIPv4.Next().LocalEndPoint;
			}
			else
			{
				bool flag4 = remoteEP.AddressFamily == AddressFamily.InterNetworkV6;
				if (!flag4)
				{
					throw new ArgumentException("Argument 'remoteEP' has unknown AddressFamily.");
				}
				bool flag5 = this.m_pSendSocketsIPv6.Count == 0;
				if (flag5)
				{
					throw new InvalidOperationException("There is no suitable IPv6 local end point in this.Bindings.");
				}
				result = (IPEndPoint)this.m_pSendSocketsIPv6.Next().LocalEndPoint;
			}
			return result;
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0004A690 File Offset: 0x00049690
		private void ProcessUdpPacket(UDP_e_PacketReceived e)
		{
			bool flag = e == null;
			if (flag)
			{
				throw new ArgumentNullException("e");
			}
			this.OnUdpPacketReceived(e);
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06000C14 RID: 3092 RVA: 0x0004A6BC File Offset: 0x000496BC
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06000C15 RID: 3093 RVA: 0x0004A6D4 File Offset: 0x000496D4
		public bool IsRunning
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				return this.m_IsRunning;
			}
		}

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06000C16 RID: 3094 RVA: 0x0004A704 File Offset: 0x00049704
		// (set) Token: 0x06000C17 RID: 3095 RVA: 0x0004A734 File Offset: 0x00049734
		public int MTU
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				return this.m_MTU;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				bool isRunning = this.m_IsRunning;
				if (isRunning)
				{
					throw new InvalidOperationException("MTU value can be changed only if UDP server is not running.");
				}
				this.m_MTU = value;
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06000C18 RID: 3096 RVA: 0x0004A778 File Offset: 0x00049778
		// (set) Token: 0x06000C19 RID: 3097 RVA: 0x0004A7A8 File Offset: 0x000497A8
		public IPEndPoint[] Bindings
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				return this.m_pBindings;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException();
				}
				bool flag2 = false;
				bool flag3 = this.m_pBindings == null;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = this.m_pBindings.Length != value.Length;
					if (flag4)
					{
						flag2 = true;
					}
					else
					{
						for (int i = 0; i < this.m_pBindings.Length; i++)
						{
							bool flag5 = !this.m_pBindings[i].Equals(value[i]);
							if (flag5)
							{
								flag2 = true;
								break;
							}
						}
					}
				}
				bool flag6 = flag2;
				if (flag6)
				{
					this.m_pBindings = value;
					this.Restart();
				}
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06000C1A RID: 3098 RVA: 0x0004A864 File Offset: 0x00049864
		public DateTime StartTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_StartTime;
			}
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06000C1B RID: 3099 RVA: 0x0004A8AC File Offset: 0x000498AC
		public long BytesReceived
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_BytesReceived;
			}
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06000C1C RID: 3100 RVA: 0x0004A8F4 File Offset: 0x000498F4
		public long PacketsReceived
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_PacketsReceived;
			}
		}

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06000C1D RID: 3101 RVA: 0x0004A93C File Offset: 0x0004993C
		public long BytesSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_BytesSent;
			}
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06000C1E RID: 3102 RVA: 0x0004A984 File Offset: 0x00049984
		public long PacketsSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("UdpServer");
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("UDP server is not running.");
				}
				return this.m_PacketsSent;
			}
		}

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x06000C1F RID: 3103 RVA: 0x0004A9CC File Offset: 0x000499CC
		// (remove) Token: 0x06000C20 RID: 3104 RVA: 0x0004AA04 File Offset: 0x00049A04
		
		public event EventHandler<UDP_e_PacketReceived> PacketReceived = null;

		// Token: 0x06000C21 RID: 3105 RVA: 0x0004AA3C File Offset: 0x00049A3C
		private void OnUdpPacketReceived(UDP_e_PacketReceived e)
		{
			bool flag = this.PacketReceived != null;
			if (flag)
			{
				this.PacketReceived(this, e);
			}
		}

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x06000C22 RID: 3106 RVA: 0x0004AA68 File Offset: 0x00049A68
		// (remove) Token: 0x06000C23 RID: 3107 RVA: 0x0004AAA0 File Offset: 0x00049AA0
		
		public event ErrorEventHandler Error = null;

		// Token: 0x06000C24 RID: 3108 RVA: 0x0004AAD8 File Offset: 0x00049AD8
		private void OnError(Exception x)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, new Error_EventArgs(x, new StackTrace()));
			}
		}

		// Token: 0x040004F5 RID: 1269
		private bool m_IsDisposed = false;

		// Token: 0x040004F6 RID: 1270
		private bool m_IsRunning = false;

		// Token: 0x040004F7 RID: 1271
		private int m_MTU = 1400;

		// Token: 0x040004F8 RID: 1272
		private IPEndPoint[] m_pBindings = null;

		// Token: 0x040004F9 RID: 1273
		private DateTime m_StartTime;

		// Token: 0x040004FA RID: 1274
		private List<Socket> m_pSockets = null;

		// Token: 0x040004FB RID: 1275
		private CircleCollection<Socket> m_pSendSocketsIPv4 = null;

		// Token: 0x040004FC RID: 1276
		private CircleCollection<Socket> m_pSendSocketsIPv6 = null;

		// Token: 0x040004FD RID: 1277
		private int m_ReceiversPerSocket = 10;

		// Token: 0x040004FE RID: 1278
		private List<UDP_DataReceiver> m_pDataReceivers = null;

		// Token: 0x040004FF RID: 1279
		private long m_BytesReceived = 0L;

		// Token: 0x04000500 RID: 1280
		private long m_PacketsReceived = 0L;

		// Token: 0x04000501 RID: 1281
		private long m_BytesSent = 0L;

		// Token: 0x04000502 RID: 1282
		private long m_PacketsSent = 0L;
	}
}
