using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D4 RID: 212
	internal class NameServiceClient : IRunnable
	{
		// Token: 0x060006F2 RID: 1778 RVA: 0x0002558F File Offset: 0x0002378F
		public NameServiceClient() : this(NameServiceClient.Lport, NameServiceClient.Laddr)
		{
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x000255A4 File Offset: 0x000237A4
		internal NameServiceClient(int lport, IPAddress laddr)
		{
			this._lport = lport;
			this.laddr = laddr;
			try
			{
				this.Baddr = Config.GetInetAddress("jcifs.netbios.baddr", Extensions.GetAddressByName("255.255.255.255"));
			}
			catch (Exception ex)
			{
			}
			this._sndBuf = new byte[NameServiceClient.SndBufSize];
			this._rcvBuf = new byte[NameServiceClient.RcvBufSize];
			bool flag = string.IsNullOrEmpty(NameServiceClient.Ro);
			if (flag)
			{
				bool flag2 = NbtAddress.GetWinsAddress() == null;
				if (flag2)
				{
					this._resolveOrder = new int[2];
					this._resolveOrder[0] = 1;
					this._resolveOrder[1] = 2;
				}
				else
				{
					this._resolveOrder = new int[3];
					this._resolveOrder[0] = 1;
					this._resolveOrder[1] = 3;
					this._resolveOrder[2] = 2;
				}
			}
			else
			{
				int[] array = new int[3];
				StringTokenizer stringTokenizer = new StringTokenizer(NameServiceClient.Ro, ",");
				int num = 0;
				while (stringTokenizer.HasMoreTokens())
				{
					string text = stringTokenizer.NextToken().Trim();
					bool flag3 = Runtime.EqualsIgnoreCase(text, "LMHOSTS");
					if (flag3)
					{
						array[num++] = 1;
					}
					else
					{
						bool flag4 = Runtime.EqualsIgnoreCase(text, "WINS");
						if (flag4)
						{
							bool flag5 = NbtAddress.GetWinsAddress() == null;
							if (flag5)
							{
								bool flag6 = NameServiceClient._log.Level > 1;
								if (flag6)
								{
									NameServiceClient._log.WriteLine("NetBIOS resolveOrder specifies WINS however the jcifs.netbios.wins property has not been set");
								}
							}
							else
							{
								array[num++] = 3;
							}
						}
						else
						{
							bool flag7 = Runtime.EqualsIgnoreCase(text, "BCAST");
							if (flag7)
							{
								array[num++] = 2;
							}
							else
							{
								bool flag8 = Runtime.EqualsIgnoreCase(text, "DNS");
								if (!flag8)
								{
									bool flag9 = NameServiceClient._log.Level > 1;
									if (flag9)
									{
										NameServiceClient._log.WriteLine("unknown resolver method: " + text);
									}
								}
							}
						}
					}
				}
				this._resolveOrder = new int[num];
				Array.Copy(array, 0, this._resolveOrder, 0, num);
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x000257E8 File Offset: 0x000239E8
		internal virtual int GetNextNameTrnId()
		{
			int num = this._nextNameTrnId + 1;
			this._nextNameTrnId = num;
			bool flag = (num & 65535) == 0;
			if (flag)
			{
				this._nextNameTrnId = 1;
			}
			return this._nextNameTrnId;
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x00025828 File Offset: 0x00023A28
		internal virtual void EnsureOpen(int timeout)
		{
			this._closeTimeout = 0;
			bool flag = NameServiceClient.SoTimeout != 0;
			if (flag)
			{
				this._closeTimeout = Math.Max(NameServiceClient.SoTimeout, timeout);
			}
			bool flag2 = this._socket == null;
			if (flag2)
			{
				this._socket = new SocketEx(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				this._socket.Bind(new IPEndPoint(this.laddr.Address, this._lport));
				bool waitResponse = this._waitResponse;
				if (waitResponse)
				{
					this._thread = new SharpCifs.Util.Sharpen.Thread(this);
					this._thread.SetDaemon(true);
					this._thread.Start();
				}
			}
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x000258D0 File Offset: 0x00023AD0
		internal virtual void TryClose()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				bool flag2 = this._socket != null;
				if (flag2)
				{
					this._socket.Close();
					this._socket = null;
				}
				this._thread = null;
				bool waitResponse = this._waitResponse;
				if (waitResponse)
				{
					this._responseTable.Clear();
				}
				else
				{
					this._autoResetWaitReceive.Set();
				}
			}
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x00025960 File Offset: 0x00023B60
		public virtual void Run()
		{
			try
			{
				while (this._thread == SharpCifs.Util.Sharpen.Thread.CurrentThread())
				{
					this._socket.SoTimeOut = this._closeTimeout;
					int length = this._socket.Receive(this._rcvBuf, 0, NameServiceClient.RcvBufSize);
					bool flag = NameServiceClient._log.Level > 3;
					if (flag)
					{
						NameServiceClient._log.WriteLine("NetBIOS: new data read from socket");
					}
					int num = NameServicePacket.ReadNameTrnId(this._rcvBuf, 0);
					NameServicePacket nameServicePacket = (NameServicePacket)this._responseTable.Get(num);
					bool flag2 = nameServicePacket == null || nameServicePacket.Received;
					if (!flag2)
					{
						NameServicePacket obj = nameServicePacket;
						lock (obj)
						{
							nameServicePacket.ReadWireFormat(this._rcvBuf, 0);
							bool flag4 = NameServiceClient._log.Level > 3;
							if (flag4)
							{
								NameServiceClient._log.WriteLine(nameServicePacket);
								Hexdump.ToHexdump(NameServiceClient._log, this._rcvBuf, 0, length);
							}
							bool isResponse = nameServicePacket.IsResponse;
							if (isResponse)
							{
								nameServicePacket.Received = true;
								Runtime.Notify(nameServicePacket);
							}
						}
					}
				}
			}
			catch (TimeoutException)
			{
			}
			catch (Exception ex)
			{
				bool flag5 = NameServiceClient._log.Level > 2;
				if (flag5)
				{
					Runtime.PrintStackTrace(ex, NameServiceClient._log);
				}
			}
			finally
			{
				this.TryClose();
			}
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00025B34 File Offset: 0x00023D34
		internal virtual void Send(NameServicePacket request, NameServicePacket response, int timeout)
		{
			int num = 0;
			int num2 = NbtAddress.Nbns.Length;
			bool flag = num2 == 0;
			if (flag)
			{
				num2 = 1;
			}
			lock (response)
			{
				while (num2-- > 0)
				{
					try
					{
						object @lock = this._lock;
						lock (@lock)
						{
							request.NameTrnId = this.GetNextNameTrnId();
							num = request.NameTrnId;
							response.Received = false;
							this._responseTable.Put(num, response);
							this.EnsureOpen(timeout + 1000);
							int length = request.WriteWireFormat(this._sndBuf, 0);
							this._socket.Send(this._sndBuf, 0, length, new IPEndPoint(request.Addr, this._lport));
							bool flag4 = NameServiceClient._log.Level > 3;
							if (flag4)
							{
								NameServiceClient._log.WriteLine(request);
								Hexdump.ToHexdump(NameServiceClient._log, this._sndBuf, 0, length);
							}
						}
						bool waitResponse = this._waitResponse;
						if (waitResponse)
						{
							long num3 = Runtime.CurrentTimeMillis();
							while (timeout > 0)
							{
								Runtime.Wait(response, (long)timeout);
								bool flag5 = response.Received && request.QuestionType == response.RecordType;
								if (flag5)
								{
									return;
								}
								response.Received = false;
								timeout -= (int)(Runtime.CurrentTimeMillis() - num3);
							}
						}
					}
					catch (Exception ex)
					{
						throw new IOException(ex.Message);
					}
					finally
					{
						bool waitResponse2 = this._waitResponse;
						if (waitResponse2)
						{
							this._responseTable.Remove(num);
						}
					}
					bool waitResponse3 = this._waitResponse;
					if (waitResponse3)
					{
						object lock2 = this._lock;
						lock (lock2)
						{
							bool flag7 = !NbtAddress.IsWins(request.Addr);
							if (flag7)
							{
								break;
							}
							bool flag8 = request.Addr == NbtAddress.GetWinsAddress();
							if (flag8)
							{
								NbtAddress.SwitchWins();
							}
							request.Addr = NbtAddress.GetWinsAddress();
						}
					}
				}
			}
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00025DE4 File Offset: 0x00023FE4
		internal virtual NbtAddress[] GetAllByName(Name name, IPAddress addr)
		{
			NameQueryRequest nameQueryRequest = new NameQueryRequest(name);
			NameQueryResponse nameQueryResponse = new NameQueryResponse();
			nameQueryRequest.Addr = (addr ?? NbtAddress.GetWinsAddress());
			nameQueryRequest.IsBroadcast = (nameQueryRequest.Addr == null);
			bool isBroadcast = nameQueryRequest.IsBroadcast;
			int num;
			if (isBroadcast)
			{
				nameQueryRequest.Addr = this.Baddr;
				num = NameServiceClient.RetryCount;
			}
			else
			{
				nameQueryRequest.IsBroadcast = false;
				num = 1;
			}
			for (;;)
			{
				try
				{
					this.Send(nameQueryRequest, nameQueryResponse, NameServiceClient.RetryTimeout);
				}
				catch (IOException ex)
				{
					bool flag = NameServiceClient._log.Level > 1;
					if (flag)
					{
						Runtime.PrintStackTrace(ex, NameServiceClient._log);
					}
					throw new UnknownHostException(ex);
				}
				bool flag2 = nameQueryResponse.Received && nameQueryResponse.ResultCode == 0;
				if (flag2)
				{
					break;
				}
				if (--num <= 0 || !nameQueryRequest.IsBroadcast)
				{
					goto Block_7;
				}
			}
			return nameQueryResponse.AddrEntry;
			Block_7:
			throw new UnknownHostException();
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00025EE0 File Offset: 0x000240E0
		internal virtual NbtAddress GetByName(Name name, IPAddress addr)
		{
			NameQueryRequest nameQueryRequest = new NameQueryRequest(name);
			NameQueryResponse nameQueryResponse = new NameQueryResponse();
			bool flag = addr != null;
			if (flag)
			{
				nameQueryRequest.Addr = addr;
				nameQueryRequest.IsBroadcast = (addr.GetAddressBytes()[3] == byte.MaxValue);
				int num = NameServiceClient.RetryCount;
				for (;;)
				{
					try
					{
						this.Send(nameQueryRequest, nameQueryResponse, NameServiceClient.RetryTimeout);
					}
					catch (IOException ex)
					{
						bool flag2 = NameServiceClient._log.Level > 1;
						if (flag2)
						{
							Runtime.PrintStackTrace(ex, NameServiceClient._log);
						}
						throw new UnknownHostException(ex);
					}
					bool flag3 = nameQueryResponse.Received && nameQueryResponse.ResultCode == 0 && nameQueryResponse.IsResponse;
					if (flag3)
					{
						break;
					}
					if (--num <= 0 || !nameQueryRequest.IsBroadcast)
					{
						goto Block_7;
					}
				}
				int num2 = nameQueryResponse.AddrEntry.Length - 1;
				nameQueryResponse.AddrEntry[num2].HostName.SrcHashCode = addr.GetHashCode();
				return nameQueryResponse.AddrEntry[num2];
				Block_7:
				throw new UnknownHostException();
			}
			for (int i = 0; i < this._resolveOrder.Length; i++)
			{
				try
				{
					switch (this._resolveOrder[i])
					{
					case 1:
					{
						// modify 0901
						//NbtAddress byName = Lmhosts.GetByName(name);
						//bool flag4 = byName != null;
						//if (flag4)
						//{
						//	byName.HostName.SrcHashCode = 0;
						//	return byName;
						//}
						// end modify 0901
						break;
					}
					case 2:
					case 3:
					{
						bool flag5 = this._resolveOrder[i] == 3 && name.name != NbtAddress.MasterBrowserName && name.HexCode != 29;
						if (flag5)
						{
							nameQueryRequest.Addr = NbtAddress.GetWinsAddress();
							nameQueryRequest.IsBroadcast = false;
						}
						else
						{
							nameQueryRequest.Addr = this.Baddr;
							nameQueryRequest.IsBroadcast = true;
						}
						int num = NameServiceClient.RetryCount;
						while (num-- > 0)
						{
							try
							{
								this.Send(nameQueryRequest, nameQueryResponse, NameServiceClient.RetryTimeout);
							}
							catch (IOException ex2)
							{
								bool flag6 = NameServiceClient._log.Level > 1;
								if (flag6)
								{
									Runtime.PrintStackTrace(ex2, NameServiceClient._log);
								}
								throw new UnknownHostException(ex2);
							}
							bool flag7 = nameQueryResponse.Received && nameQueryResponse.ResultCode == 0 && nameQueryResponse.IsResponse;
							if (flag7)
							{
								nameQueryResponse.AddrEntry[0].HostName.SrcHashCode = nameQueryRequest.Addr.GetHashCode();
								return nameQueryResponse.AddrEntry[0];
							}
							bool flag8 = this._resolveOrder[i] == 3;
							if (flag8)
							{
								break;
							}
						}
						break;
					}
					}
				}
				catch (IOException)
				{
				}
			}
			throw new UnknownHostException();
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x000261D4 File Offset: 0x000243D4
		internal virtual NbtAddress[] GetNodeStatus(NbtAddress addr)
		{
			NodeStatusResponse nodeStatusResponse = new NodeStatusResponse(addr);
			NodeStatusRequest nodeStatusRequest = new NodeStatusRequest(new Name(NbtAddress.AnyHostsName, 0, null));
			nodeStatusRequest.Addr = addr.GetInetAddress();
			int retryCount = NameServiceClient.RetryCount;
			while (retryCount-- > 0)
			{
				try
				{
					this.Send(nodeStatusRequest, nodeStatusResponse, NameServiceClient.RetryTimeout);
				}
				catch (IOException ex)
				{
					bool flag = NameServiceClient._log.Level > 1;
					if (flag)
					{
						Runtime.PrintStackTrace(ex, NameServiceClient._log);
					}
					throw new UnknownHostException(ex);
				}
				bool flag2 = nodeStatusResponse.Received && nodeStatusResponse.ResultCode == 0;
				if (flag2)
				{
					int hashCode = nodeStatusRequest.Addr.GetHashCode();
					for (int i = 0; i < nodeStatusResponse.AddressArray.Length; i++)
					{
						nodeStatusResponse.AddressArray[i].HostName.SrcHashCode = hashCode;
					}
					return nodeStatusResponse.AddressArray;
				}
			}
			throw new UnknownHostException();
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x000262E0 File Offset: 0x000244E0
		internal virtual NbtAddress[] GetHosts()
		{
			try
			{
				this._waitResponse = false;
				byte[] addressBytes = this.laddr.GetAddressBytes();
				for (int i = 1; i <= 254; i++)
				{
					byte[] address = new byte[]
					{
						addressBytes[0],
						addressBytes[1],
						addressBytes[2],
						(byte)i
					};
					IPAddress ipaddress = new IPAddress(address);
					NodeStatusResponse response = new NodeStatusResponse(new NbtAddress(NbtAddress.UnknownName, (int)ipaddress.Address, false, 32));
					this.Send(new NodeStatusRequest(new Name(NbtAddress.AnyHostsName, 32, null))
					{
						Addr = ipaddress
					}, response, 0);
				}
			}
			catch (IOException ex)
			{
				bool flag = NameServiceClient._log.Level > 1;
				if (flag)
				{
					Runtime.PrintStackTrace(ex, NameServiceClient._log);
				}
				throw new UnknownHostException(ex);
			}
			this._autoResetWaitReceive = new AutoResetEvent(false);
			this._thread = new SharpCifs.Util.Sharpen.Thread(this);
			this._thread.SetDaemon(true);
			this._thread.Start();
			this._autoResetWaitReceive.WaitOne();
			List<NbtAddress> list = new List<NbtAddress>();
			foreach (object key in this._responseTable.Keys)
			{
				NodeStatusResponse nodeStatusResponse = (NodeStatusResponse)this._responseTable[key];
				bool flag2 = nodeStatusResponse.Received && nodeStatusResponse.ResultCode == 0;
				if (flag2)
				{
					foreach (NbtAddress nbtAddress in nodeStatusResponse.AddressArray)
					{
						bool flag3 = nbtAddress.HostName.HexCode == 32;
						if (flag3)
						{
							list.Add(nbtAddress);
						}
					}
				}
			}
			this._responseTable.Clear();
			this._waitResponse = true;
			return (list.Count > 0) ? list.ToArray() : null;
		}

		// Token: 0x0400042A RID: 1066
		internal const int DefaultSoTimeout = 5000;

		// Token: 0x0400042B RID: 1067
		internal const int DefaultRcvBufSize = 576;

		// Token: 0x0400042C RID: 1068
		internal const int DefaultSndBufSize = 576;

		// Token: 0x0400042D RID: 1069
		internal const int NameServiceUdpPort = 137;

		// Token: 0x0400042E RID: 1070
		internal const int DefaultRetryCount = 2;

		// Token: 0x0400042F RID: 1071
		internal const int DefaultRetryTimeout = 3000;

		// Token: 0x04000430 RID: 1072
		internal const int ResolverLmhosts = 1;

		// Token: 0x04000431 RID: 1073
		internal const int ResolverBcast = 2;

		// Token: 0x04000432 RID: 1074
		internal const int ResolverWins = 3;

		// Token: 0x04000433 RID: 1075
		private static readonly int SndBufSize = Config.GetInt("jcifs.netbios.snd_buf_size", 576);

		// Token: 0x04000434 RID: 1076
		private static readonly int RcvBufSize = Config.GetInt("jcifs.netbios.rcv_buf_size", 576);

		// Token: 0x04000435 RID: 1077
		private static readonly int SoTimeout = Config.GetInt("jcifs.netbios.soTimeout", 5000);

		// Token: 0x04000436 RID: 1078
		private static readonly int RetryCount = Config.GetInt("jcifs.netbios.retryCount", 2);

		// Token: 0x04000437 RID: 1079
		private static readonly int RetryTimeout = Config.GetInt("jcifs.netbios.retryTimeout", 3000);

		// Token: 0x04000438 RID: 1080
		private static readonly int Lport = Config.GetInt("jcifs.netbios.lport", 137);

		// Token: 0x04000439 RID: 1081
		private static readonly IPAddress Laddr = Config.GetInetAddress("jcifs.netbios.laddr", null);

		// Token: 0x0400043A RID: 1082
		private static readonly string Ro = Config.GetProperty("jcifs.resolveOrder");

		// Token: 0x0400043B RID: 1083
		private static LogStream _log = LogStream.GetInstance();

		// Token: 0x0400043C RID: 1084
		private readonly object _lock = new object();

		// Token: 0x0400043D RID: 1085
		private int _lport;

		// Token: 0x0400043E RID: 1086
		private int _closeTimeout;

		// Token: 0x0400043F RID: 1087
		private byte[] _sndBuf;

		// Token: 0x04000440 RID: 1088
		private byte[] _rcvBuf;

		// Token: 0x04000441 RID: 1089
		private SocketEx _socket;

		// Token: 0x04000442 RID: 1090
		private Hashtable _responseTable = new Hashtable();

		// Token: 0x04000443 RID: 1091
		private SharpCifs.Util.Sharpen.Thread _thread;

		// Token: 0x04000444 RID: 1092
		private int _nextNameTrnId;

		// Token: 0x04000445 RID: 1093
		private int[] _resolveOrder;

		// Token: 0x04000446 RID: 1094
		private bool _waitResponse = true;

		// Token: 0x04000447 RID: 1095
		private AutoResetEvent _autoResetWaitReceive;

		// Token: 0x04000448 RID: 1096
		internal IPAddress laddr;

		// Token: 0x04000449 RID: 1097
		internal IPAddress Baddr;
	}
}
