using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LumiSoft.Net.UDP;

namespace LumiSoft.Net.DNS.Client
{
	// Token: 0x0200025F RID: 607
	public class Dns_Client : IDisposable
	{
		// Token: 0x060015B4 RID: 5556 RVA: 0x000868CC File Offset: 0x000858CC
		static Dns_Client()
		{
			try
			{
				List<IPAddress> list = new List<IPAddress>();
				foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
				{
					bool flag = networkInterface.OperationalStatus == OperationalStatus.Up;
					if (flag)
					{
						foreach (IPAddress ipaddress in networkInterface.GetIPProperties().DnsAddresses)
						{
							bool flag2 = ipaddress.AddressFamily == AddressFamily.InterNetwork;
							if (flag2)
							{
								bool flag3 = !list.Contains(ipaddress);
								if (flag3)
								{
									list.Add(ipaddress);
								}
							}
						}
					}
				}
				Dns_Client.m_DnsServers = list.ToArray();
			}
			catch
			{
			}
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x000869BC File Offset: 0x000859BC
		public Dns_Client()
		{
			this.m_pTransactions = new Dictionary<int, DNS_ClientTransaction>();
			this.m_pIPv4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			this.m_pIPv4Socket.Bind(new IPEndPoint(IPAddress.Any, 0));
			bool ossupportsIPv = Socket.OSSupportsIPv6;
			if (ossupportsIPv)
			{
				this.m_pIPv6Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
				this.m_pIPv6Socket.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
			}
			this.m_pReceivers = new List<UDP_DataReceiver>();
			this.m_pRandom = new Random();
			this.m_pCache = new DNS_ClientCache();
			for (int i = 0; i < 5; i++)
			{
				UDP_DataReceiver udp_DataReceiver = new UDP_DataReceiver(this.m_pIPv4Socket);
				udp_DataReceiver.PacketReceived += delegate(object s1, UDP_e_PacketReceived e1)
				{
					this.ProcessUdpPacket(e1);
				};
				this.m_pReceivers.Add(udp_DataReceiver);
				udp_DataReceiver.Start();
				bool flag = this.m_pIPv6Socket != null;
				if (flag)
				{
					UDP_DataReceiver udp_DataReceiver2 = new UDP_DataReceiver(this.m_pIPv6Socket);
					udp_DataReceiver2.PacketReceived += delegate(object s1, UDP_e_PacketReceived e1)
					{
						this.ProcessUdpPacket(e1);
					};
					this.m_pReceivers.Add(udp_DataReceiver2);
					udp_DataReceiver2.Start();
				}
			}
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x00086B18 File Offset: 0x00085B18
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				bool flag = this.m_pReceivers != null;
				if (flag)
				{
					foreach (UDP_DataReceiver udp_DataReceiver in this.m_pReceivers)
					{
						udp_DataReceiver.Dispose();
					}
					this.m_pReceivers = null;
				}
				this.m_pIPv4Socket.Close();
				this.m_pIPv4Socket = null;
				bool flag2 = this.m_pIPv6Socket != null;
				if (flag2)
				{
					this.m_pIPv6Socket.Close();
					this.m_pIPv6Socket = null;
				}
				this.m_pTransactions = null;
				this.m_pRandom = null;
				this.m_pCache.Dispose();
				this.m_pCache = null;
			}
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x00086BF8 File Offset: 0x00085BF8
		public DNS_ClientTransaction CreateTransaction(DNS_QType queryType, string queryText, int timeout)
		{
			List<IPAddress> list = new List<IPAddress>();
			foreach (string text in Dns_Client.DnsServers)
			{
				bool flag = Net_Utils.IsIPAddress(text);
				if (flag)
				{
					list.Add(IPAddress.Parse(text));
				}
			}
			return this.CreateTransaction(list.ToArray(), queryType, queryText, timeout);
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x00086C58 File Offset: 0x00085C58
		public DNS_ClientTransaction CreateTransaction(IPAddress[] dnsServers, DNS_QType queryType, string queryText, int timeout)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = dnsServers == null;
			if (flag)
			{
				throw new ArgumentNullException("dnsServers");
			}
			bool flag2 = queryText == null;
			if (flag2)
			{
				throw new ArgumentNullException("queryText");
			}
			bool flag3 = queryText == string.Empty;
			if (flag3)
			{
				throw new ArgumentException("Argument 'queryText' value may not be \"\".", "queryText");
			}
			bool flag4 = queryType == DNS_QType.PTR;
			if (flag4)
			{
				IPAddress ipaddress = null;
				bool flag5 = !IPAddress.TryParse(queryText, out ipaddress);
				if (flag5)
				{
					throw new ArgumentException("Argument 'queryText' value must be IP address if queryType == DNS_QType.PTR.", "queryText");
				}
			}
			bool flag6 = queryType == DNS_QType.PTR;
			if (flag6)
			{
				string text = queryText;
				IPAddress ipaddress2 = IPAddress.Parse(text);
				queryText = "";
				bool flag7 = ipaddress2.AddressFamily == AddressFamily.InterNetworkV6;
				if (flag7)
				{
					char[] array = text.Replace(":", "").ToCharArray();
					for (int i = array.Length - 1; i > -1; i--)
					{
						queryText = queryText + array[i].ToString() + ".";
					}
					queryText += "IP6.ARPA";
				}
				else
				{
					string[] array2 = text.Split(new char[]
					{
						'.'
					});
					for (int j = 3; j > -1; j--)
					{
						queryText = queryText + array2[j] + ".";
					}
					queryText += "in-addr.arpa";
				}
			}
			int num = 0;
			Dictionary<int, DNS_ClientTransaction> pTransactions = this.m_pTransactions;
			lock (pTransactions)
			{
				bool flag9;
				do
				{
					num = this.m_pRandom.Next(65535);
					flag9 = !this.m_pTransactions.ContainsKey(num);
				}
				while (!flag9);
			}
			DNS_ClientTransaction retVal = new DNS_ClientTransaction(this, dnsServers, num, queryType, queryText, timeout);
			retVal.StateChanged += delegate(object s1, EventArgs<DNS_ClientTransaction> e1)
			{
				bool flag11 = retVal.State == DNS_ClientTransactionState.Disposed;
				if (flag11)
				{
					Dictionary<int, DNS_ClientTransaction> pTransactions3 = this.m_pTransactions;
					lock (pTransactions3)
					{
						this.m_pTransactions.Remove(e1.Value.ID);
					}
				}
			};
			Dictionary<int, DNS_ClientTransaction> pTransactions2 = this.m_pTransactions;
			lock (pTransactions2)
			{
				this.m_pTransactions.Add(retVal.ID, retVal);
			}
			return retVal;
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x00086EE0 File Offset: 0x00085EE0
		public DnsServerResponse Query(string queryText, DNS_QType queryType)
		{
			return this.Query(queryText, queryType, 2000);
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x00086F00 File Offset: 0x00085F00
		public DnsServerResponse Query(string queryText, DNS_QType queryType, int timeout)
		{
			List<IPAddress> list = new List<IPAddress>();
			foreach (string text in Dns_Client.DnsServers)
			{
				bool flag = Net_Utils.IsIPAddress(text);
				if (flag)
				{
					list.Add(IPAddress.Parse(text));
				}
			}
			return this.Query(list.ToArray(), queryText, queryType, timeout);
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x00086F60 File Offset: 0x00085F60
		public DnsServerResponse Query(IPAddress[] dnsServers, string queryText, DNS_QType queryType, int timeout)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = dnsServers == null;
			if (flag)
			{
				throw new ArgumentNullException("dnsServers");
			}
			bool flag2 = queryText == null;
			if (flag2)
			{
				throw new ArgumentNullException("queryText");
			}
			DnsServerResponse retVal = null;
			ManualResetEvent wait = new ManualResetEvent(false);
			DNS_ClientTransaction transaction = this.CreateTransaction(dnsServers, queryType, queryText, timeout);
			transaction.Timeout += delegate(object s, EventArgs e)
			{
				bool flag3 = wait != null;
				if (flag3)
				{
					wait.Set();
				}
			};
			transaction.StateChanged += delegate(object s1, EventArgs<DNS_ClientTransaction> e1)
			{
				bool flag3 = transaction.State == DNS_ClientTransactionState.Completed || transaction.State == DNS_ClientTransactionState.Disposed;
				if (flag3)
				{
					retVal = transaction.Response;
					bool flag4 = wait != null;
					if (flag4)
					{
						wait.Set();
					}
				}
			};
			transaction.Start();
			wait.WaitOne();
			wait.Close();
			wait = null;
			return retVal;
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x00087044 File Offset: 0x00086044
		public IPAddress[] GetHostAddresses(string hostNameOrIP)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = hostNameOrIP == null;
			if (flag)
			{
				throw new ArgumentNullException("hostNameOrIP");
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			IPAddress[] addresses;
			using (Dns_Client.GetHostAddressesAsyncOP getHostAddressesAsyncOP = new Dns_Client.GetHostAddressesAsyncOP(hostNameOrIP))
			{
				getHostAddressesAsyncOP.CompletedAsync += delegate(object s1, EventArgs<Dns_Client.GetHostAddressesAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.GetHostAddressesAsync(getHostAddressesAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = getHostAddressesAsyncOP.Error != null;
				if (flag3)
				{
					throw getHostAddressesAsyncOP.Error;
				}
				addresses = getHostAddressesAsyncOP.Addresses;
			}
			return addresses;
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x00087124 File Offset: 0x00086124
		public bool GetHostAddressesAsync(Dns_Client.GetHostAddressesAsyncOP op)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.State > AsyncOP_State.WaitingForStart;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x0008718C File Offset: 0x0008618C
		public HostEntry[] GetHostsAddresses(string[] hostNames)
		{
			return this.GetHostsAddresses(hostNames, false);
		}

		// Token: 0x060015BF RID: 5567 RVA: 0x000871A8 File Offset: 0x000861A8
		public HostEntry[] GetHostsAddresses(string[] hostNames, bool resolveAny)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = hostNames == null;
			if (flag)
			{
				throw new ArgumentNullException("hostNames");
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			HostEntry[] hostEntries;
			using (Dns_Client.GetHostsAddressesAsyncOP getHostsAddressesAsyncOP = new Dns_Client.GetHostsAddressesAsyncOP(hostNames, resolveAny))
			{
				getHostsAddressesAsyncOP.CompletedAsync += delegate(object s1, EventArgs<Dns_Client.GetHostsAddressesAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.GetHostsAddressesAsync(getHostsAddressesAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = getHostsAddressesAsyncOP.Error != null;
				if (flag3)
				{
					throw getHostsAddressesAsyncOP.Error;
				}
				hostEntries = getHostsAddressesAsyncOP.HostEntries;
			}
			return hostEntries;
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x0008728C File Offset: 0x0008628C
		public bool GetHostsAddressesAsync(Dns_Client.GetHostsAddressesAsyncOP op)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.State > AsyncOP_State.WaitingForStart;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x000872F4 File Offset: 0x000862F4
		public HostEntry[] GetEmailHosts(string domain)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = domain == null;
			if (flag)
			{
				throw new ArgumentNullException("domain");
			}
			bool flag2 = domain == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'domain' value must be specified.", "domain");
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			HostEntry[] hosts;
			using (Dns_Client.GetEmailHostsAsyncOP getEmailHostsAsyncOP = new Dns_Client.GetEmailHostsAsyncOP(domain))
			{
				getEmailHostsAsyncOP.CompletedAsync += delegate(object s1, EventArgs<Dns_Client.GetEmailHostsAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag3 = !this.GetEmailHostsAsync(getEmailHostsAsyncOP);
				if (flag3)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag4 = getEmailHostsAsyncOP.Error != null;
				if (flag4)
				{
					throw getEmailHostsAsyncOP.Error;
				}
				hosts = getEmailHostsAsyncOP.Hosts;
			}
			return hosts;
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x000873FC File Offset: 0x000863FC
		public bool GetEmailHostsAsync(Dns_Client.GetEmailHostsAsyncOP op)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.State > AsyncOP_State.WaitingForStart;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x00087464 File Offset: 0x00086464
		internal void Send(IPAddress target, byte[] packet, int count)
		{
			bool flag = target == null;
			if (flag)
			{
				throw new ArgumentNullException("target");
			}
			bool flag2 = packet == null;
			if (flag2)
			{
				throw new ArgumentNullException("packet");
			}
			try
			{
				bool flag3 = target.AddressFamily == AddressFamily.InterNetwork;
				if (flag3)
				{
					this.m_pIPv4Socket.SendTo(packet, count, SocketFlags.None, new IPEndPoint(target, 53));
				}
				else
				{
					bool flag4 = target.AddressFamily == AddressFamily.InterNetworkV6;
					if (flag4)
					{
						this.m_pIPv6Socket.SendTo(packet, count, SocketFlags.None, new IPEndPoint(target, 53));
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x00087504 File Offset: 0x00086504
		private void ProcessUdpPacket(UDP_e_PacketReceived e)
		{
			try
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					DnsServerResponse dnsServerResponse = this.ParseQuery(e.Buffer);
					DNS_ClientTransaction dns_ClientTransaction = null;
					bool flag = this.m_pTransactions.TryGetValue(dnsServerResponse.ID, out dns_ClientTransaction);
					if (flag)
					{
						bool flag2 = dns_ClientTransaction.State == DNS_ClientTransactionState.Active;
						if (flag2)
						{
							bool flag3 = Dns_Client.m_UseDnsCache && dnsServerResponse.ResponseCode == DNS_RCode.NO_ERROR;
							if (flag3)
							{
								this.m_pCache.AddToCache(dns_ClientTransaction.QName, (int)dns_ClientTransaction.QType, dnsServerResponse);
							}
							dns_ClientTransaction.ProcessResponse(dnsServerResponse);
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x000875B0 File Offset: 0x000865B0
		internal static bool GetQName(byte[] reply, ref int offset, ref string name)
		{
			bool qnameI = Dns_Client.GetQNameI(reply, ref offset, ref name);
			bool flag = name.Length > 0;
			if (flag)
			{
				IdnMapping idnMapping = new IdnMapping();
				name = idnMapping.GetUnicode(name);
			}
			return qnameI;
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x000875EC File Offset: 0x000865EC
		private static bool GetQNameI(byte[] reply, ref int offset, ref string name)
		{
			bool result;
			try
			{
				for (;;)
				{
					bool flag = offset >= reply.Length;
					if (flag)
					{
						break;
					}
					bool flag2 = reply[offset] == 0;
					if (flag2)
					{
						goto Block_3;
					}
					bool flag3 = (reply[offset] & 192) == 192;
					bool flag4 = flag3;
					if (flag4)
					{
						goto Block_4;
					}
					int num = (int)(reply[offset] & 63);
					offset++;
					name += Encoding.UTF8.GetString(reply, offset, num);
					offset += num;
					bool flag5 = reply[offset] > 0;
					if (flag5)
					{
						name += ".";
					}
				}
				return false;
				Block_3:
				offset++;
				return true;
				Block_4:
				int num2 = (int)(reply[offset] & 63) << 8;
				int num3 = offset + 1;
				offset = num3;
				int num4 = num2 | (int)reply[num3];
				offset++;
				result = Dns_Client.GetQNameI(reply, ref num4, ref name);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x000876E8 File Offset: 0x000866E8
		private DnsServerResponse ParseQuery(byte[] reply)
		{
			int id = (int)reply[0] << 8 | (int)reply[1];
			OPCODE opcode = (OPCODE)(reply[2] >> 3 & 15);
			DNS_RCode rcode = (DNS_RCode)(reply[3] & 15);
			int num = (int)reply[4] << 8 | (int)reply[5];
			int answerCount = (int)reply[6] << 8 | (int)reply[7];
			int answerCount2 = (int)reply[8] << 8 | (int)reply[9];
			int answerCount3 = (int)reply[10] << 8 | (int)reply[11];
			int num2 = 12;
			for (int i = 0; i < num; i++)
			{
				string text = "";
				Dns_Client.GetQName(reply, ref num2, ref text);
				num2 += 4;
			}
			List<DNS_rr> answers = this.ParseAnswers(reply, answerCount, ref num2);
			List<DNS_rr> authoritiveAnswers = this.ParseAnswers(reply, answerCount2, ref num2);
			List<DNS_rr> additionalAnswers = this.ParseAnswers(reply, answerCount3, ref num2);
			return new DnsServerResponse(true, id, rcode, answers, authoritiveAnswers, additionalAnswers);
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x000877B0 File Offset: 0x000867B0
		private List<DNS_rr> ParseAnswers(byte[] reply, int answerCount, ref int offset)
		{
			List<DNS_rr> list = new List<DNS_rr>();
			for (int i = 0; i < answerCount; i++)
			{
				string name = "";
				bool flag = !Dns_Client.GetQName(reply, ref offset, ref name);
				if (flag)
				{
					break;
				}
				int num = offset;
				offset = num + 1;
				int num2 = (int)reply[num] << 8;
				num = offset;
				offset = num + 1;
				int num3 = num2 | (int)reply[num];
				num = offset;
				offset = num + 1;
				int num4 = (int)reply[num] << 8;
				num = offset;
				offset = num + 1;
				int num5 = num4 | (int)reply[num];
				num = offset;
				offset = num + 1;
				int num6 = (int)reply[num] << 24;
				num = offset;
				offset = num + 1;
				int num7 = num6 | (int)reply[num] << 16;
				num = offset;
				offset = num + 1;
				int num8 = num7 | (int)reply[num] << 8;
				num = offset;
				offset = num + 1;
				int ttl = num8 | (int)reply[num];
				num = offset;
				offset = num + 1;
				int num9 = (int)reply[num] << 8;
				num = offset;
				offset = num + 1;
				int num10 = num9 | (int)reply[num];
				bool flag2 = num3 == 1;
				if (flag2)
				{
					list.Add(DNS_rr_A.Parse(name, reply, ref offset, num10, ttl));
				}
				else
				{
					bool flag3 = num3 == 2;
					if (flag3)
					{
						list.Add(DNS_rr_NS.Parse(name, reply, ref offset, num10, ttl));
					}
					else
					{
						bool flag4 = num3 == 5;
						if (flag4)
						{
							list.Add(DNS_rr_CNAME.Parse(name, reply, ref offset, num10, ttl));
						}
						else
						{
							bool flag5 = num3 == 6;
							if (flag5)
							{
								list.Add(DNS_rr_SOA.Parse(name, reply, ref offset, num10, ttl));
							}
							else
							{
								bool flag6 = num3 == 12;
								if (flag6)
								{
									list.Add(DNS_rr_PTR.Parse(name, reply, ref offset, num10, ttl));
								}
								else
								{
									bool flag7 = num3 == 13;
									if (flag7)
									{
										list.Add(DNS_rr_HINFO.Parse(name, reply, ref offset, num10, ttl));
									}
									else
									{
										bool flag8 = num3 == 15;
										if (flag8)
										{
											list.Add(DNS_rr_MX.Parse(name, reply, ref offset, num10, ttl));
										}
										else
										{
											bool flag9 = num3 == 16;
											if (flag9)
											{
												list.Add(DNS_rr_TXT.Parse(name, reply, ref offset, num10, ttl));
											}
											else
											{
												bool flag10 = num3 == 28;
												if (flag10)
												{
													list.Add(DNS_rr_AAAA.Parse(name, reply, ref offset, num10, ttl));
												}
												else
												{
													bool flag11 = num3 == 33;
													if (flag11)
													{
														list.Add(DNS_rr_SRV.Parse(name, reply, ref offset, num10, ttl));
													}
													else
													{
														bool flag12 = num3 == 35;
														if (flag12)
														{
															list.Add(DNS_rr_NAPTR.Parse(name, reply, ref offset, num10, ttl));
														}
														else
														{
															bool flag13 = num3 == 99;
															if (flag13)
															{
																list.Add(DNS_rr_SPF.Parse(name, reply, ref offset, num10, ttl));
															}
															else
															{
																offset += num10;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x00087A60 File Offset: 0x00086A60
		internal static string ReadCharacterString(byte[] data, ref int offset)
		{
			int num = offset;
			offset = num + 1;
			int num2 = (int)data[num];
			string @string = Encoding.Default.GetString(data, offset, num2);
			offset += num2;
			return @string;
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x060015CA RID: 5578 RVA: 0x00087A94 File Offset: 0x00086A94
		public static Dns_Client Static
		{
			get
			{
				bool flag = Dns_Client.m_pDnsClient == null;
				if (flag)
				{
					Dns_Client.m_pDnsClient = new Dns_Client();
				}
				return Dns_Client.m_pDnsClient;
			}
		}

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x060015CB RID: 5579 RVA: 0x00087AC4 File Offset: 0x00086AC4
		// (set) Token: 0x060015CC RID: 5580 RVA: 0x00087B0C File Offset: 0x00086B0C
		public static string[] DnsServers
		{
			get
			{
				string[] array = new string[Dns_Client.m_DnsServers.Length];
				for (int i = 0; i < Dns_Client.m_DnsServers.Length; i++)
				{
					array[i] = Dns_Client.m_DnsServers[i].ToString();
				}
				return array;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException();
				}
				IPAddress[] array = new IPAddress[value.Length];
				for (int i = 0; i < value.Length; i++)
				{
					array[i] = IPAddress.Parse(value[i]);
				}
				Dns_Client.m_DnsServers = array;
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x060015CD RID: 5581 RVA: 0x00087B58 File Offset: 0x00086B58
		// (set) Token: 0x060015CE RID: 5582 RVA: 0x00087B6F File Offset: 0x00086B6F
		public static bool UseDnsCache
		{
			get
			{
				return Dns_Client.m_UseDnsCache;
			}
			set
			{
				Dns_Client.m_UseDnsCache = value;
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x060015CF RID: 5583 RVA: 0x00087B78 File Offset: 0x00086B78
		public DNS_ClientCache Cache
		{
			get
			{
				return this.m_pCache;
			}
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x00087B90 File Offset: 0x00086B90
		[Obsolete("Use Dns_Client.GetHostAddresses instead.")]
		public static IPAddress[] Resolve(string[] hosts)
		{
			bool flag = hosts == null;
			if (flag)
			{
				throw new ArgumentNullException("hosts");
			}
			List<IPAddress> list = new List<IPAddress>();
			foreach (string host in hosts)
			{
				IPAddress[] array = Dns_Client.Resolve(host);
				foreach (IPAddress item in array)
				{
					bool flag2 = !list.Contains(item);
					if (flag2)
					{
						list.Add(item);
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015D1 RID: 5585 RVA: 0x00087C24 File Offset: 0x00086C24
		[Obsolete("Use Dns_Client.GetHostAddresses instead.")]
		public static IPAddress[] Resolve(string host)
		{
			bool flag = host == null;
			if (flag)
			{
				throw new ArgumentNullException("host");
			}
			try
			{
				return new IPAddress[]
				{
					IPAddress.Parse(host)
				};
			}
			catch
			{
			}
			bool flag2 = host.IndexOf(".") == -1;
			IPAddress[] result;
			if (flag2)
			{
				result = Dns.GetHostEntry(host).AddressList;
			}
			else
			{
				using (Dns_Client dns_Client = new Dns_Client())
				{
					DnsServerResponse dnsServerResponse = dns_Client.Query(host, DNS_QType.A);
					bool flag3 = dnsServerResponse.ResponseCode == DNS_RCode.NO_ERROR;
					if (!flag3)
					{
						throw new Exception(dnsServerResponse.ResponseCode.ToString());
					}
					DNS_rr_A[] arecords = dnsServerResponse.GetARecords();
					IPAddress[] array = new IPAddress[arecords.Length];
					for (int i = 0; i < arecords.Length; i++)
					{
						array[i] = arecords[i].IP;
					}
					result = array;
				}
			}
			return result;
		}

		// Token: 0x040008A0 RID: 2208
		private static Dns_Client m_pDnsClient = null;

		// Token: 0x040008A1 RID: 2209
		private static IPAddress[] m_DnsServers = null;

		// Token: 0x040008A2 RID: 2210
		private static bool m_UseDnsCache = true;

		// Token: 0x040008A3 RID: 2211
		private bool m_IsDisposed = false;

		// Token: 0x040008A4 RID: 2212
		private Dictionary<int, DNS_ClientTransaction> m_pTransactions = null;

		// Token: 0x040008A5 RID: 2213
		private Socket m_pIPv4Socket = null;

		// Token: 0x040008A6 RID: 2214
		private Socket m_pIPv6Socket = null;

		// Token: 0x040008A7 RID: 2215
		private List<UDP_DataReceiver> m_pReceivers = null;

		// Token: 0x040008A8 RID: 2216
		private Random m_pRandom = null;

		// Token: 0x040008A9 RID: 2217
		private DNS_ClientCache m_pCache = null;

		// Token: 0x02000381 RID: 897
		public class GetHostAddressesAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001BA5 RID: 7077 RVA: 0x000AACFC File Offset: 0x000A9CFC
			public GetHostAddressesAsyncOP(string hostNameOrIP)
			{
				bool flag = hostNameOrIP == null;
				if (flag)
				{
					throw new ArgumentNullException("hostNameOrIP");
				}
				this.m_HostNameOrIP = hostNameOrIP;
				this.m_pIPv4Addresses = new List<IPAddress>();
				this.m_pIPv6Addresses = new List<IPAddress>();
			}

			// Token: 0x06001BA6 RID: 7078 RVA: 0x000AAD88 File Offset: 0x000A9D88
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_HostNameOrIP = null;
					this.m_pIPv4Addresses = null;
					this.m_pIPv6Addresses = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001BA7 RID: 7079 RVA: 0x000AADD4 File Offset: 0x000A9DD4
			internal bool Start(Dns_Client dnsClient)
			{
				bool flag = dnsClient == null;
				if (flag)
				{
					throw new ArgumentNullException("dnsClient");
				}
				this.SetState(AsyncOP_State.Active);
				bool flag2 = Net_Utils.IsIPAddress(this.m_HostNameOrIP);
				if (flag2)
				{
					this.m_pIPv4Addresses.Add(IPAddress.Parse(this.m_HostNameOrIP));
					this.SetState(AsyncOP_State.Completed);
				}
				bool flag3 = this.m_HostNameOrIP.IndexOf(".") == -1;
				if (flag3)
				{
					try
					{
						AsyncCallback requestCallback = delegate(IAsyncResult ar)
						{
							try
							{
								foreach (IPAddress ipaddress in Dns.EndGetHostAddresses(ar))
								{
									bool flag5 = ipaddress.AddressFamily == AddressFamily.InterNetwork;
									if (flag5)
									{
										this.m_pIPv4Addresses.Add(ipaddress);
									}
									else
									{
										this.m_pIPv6Addresses.Add(ipaddress);
									}
								}
							}
							catch (Exception pException2)
							{
								this.m_pException = pException2;
							}
							this.SetState(AsyncOP_State.Completed);
						};
						Dns.BeginGetHostAddresses(this.m_HostNameOrIP, requestCallback, null);
					}
					catch (Exception pException)
					{
						this.m_pException = pException;
					}
				}
				else
				{
					DNS_ClientTransaction dns_ClientTransaction = dnsClient.CreateTransaction(DNS_QType.A, this.m_HostNameOrIP, 2000);
					dns_ClientTransaction.StateChanged += delegate(object s1, EventArgs<DNS_ClientTransaction> e1)
					{
						bool flag5 = e1.Value.State == DNS_ClientTransactionState.Completed;
						if (flag5)
						{
							object pLock2 = this.m_pLock;
							lock (pLock2)
							{
								bool flag7 = e1.Value.Response.ResponseCode > DNS_RCode.NO_ERROR;
								if (flag7)
								{
									this.m_pException = new DNS_ClientException(e1.Value.Response.ResponseCode);
								}
								else
								{
									foreach (DNS_rr_A dns_rr_A in e1.Value.Response.GetARecords())
									{
										this.m_pIPv4Addresses.Add(dns_rr_A.IP);
									}
								}
								this.m_Counter++;
								bool flag8 = this.m_Counter == 2;
								if (flag8)
								{
									this.SetState(AsyncOP_State.Completed);
								}
							}
						}
					};
					dns_ClientTransaction.Timeout += delegate(object s1, EventArgs e1)
					{
						object pLock2 = this.m_pLock;
						lock (pLock2)
						{
							this.m_pException = new IOException("DNS transaction timeout, no response from DNS server.");
							this.m_Counter++;
							bool flag6 = this.m_Counter == 2;
							if (flag6)
							{
								this.SetState(AsyncOP_State.Completed);
							}
						}
					};
					dns_ClientTransaction.Start();
					DNS_ClientTransaction dns_ClientTransaction2 = dnsClient.CreateTransaction(DNS_QType.AAAA, this.m_HostNameOrIP, 2000);
					dns_ClientTransaction2.StateChanged += delegate(object s1, EventArgs<DNS_ClientTransaction> e1)
					{
						bool flag5 = e1.Value.State == DNS_ClientTransactionState.Completed;
						if (flag5)
						{
							object pLock2 = this.m_pLock;
							lock (pLock2)
							{
								bool flag7 = e1.Value.Response.ResponseCode > DNS_RCode.NO_ERROR;
								if (flag7)
								{
									this.m_pException = new DNS_ClientException(e1.Value.Response.ResponseCode);
								}
								else
								{
									foreach (DNS_rr_AAAA dns_rr_AAAA in e1.Value.Response.GetAAAARecords())
									{
										this.m_pIPv6Addresses.Add(dns_rr_AAAA.IP);
									}
								}
								this.m_Counter++;
								bool flag8 = this.m_Counter == 2;
								if (flag8)
								{
									this.SetState(AsyncOP_State.Completed);
								}
							}
						}
					};
					dns_ClientTransaction2.Timeout += delegate(object s1, EventArgs e1)
					{
						object pLock2 = this.m_pLock;
						lock (pLock2)
						{
							this.m_pException = new IOException("DNS transaction timeout, no response from DNS server.");
							this.m_Counter++;
							bool flag6 = this.m_Counter == 2;
							if (flag6)
							{
								this.SetState(AsyncOP_State.Completed);
							}
						}
					};
					dns_ClientTransaction2.Start();
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001BA8 RID: 7080 RVA: 0x000AAF5C File Offset: 0x000A9F5C
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x17000894 RID: 2196
			// (get) Token: 0x06001BA9 RID: 7081 RVA: 0x000AAFD4 File Offset: 0x000A9FD4
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000895 RID: 2197
			// (get) Token: 0x06001BAA RID: 7082 RVA: 0x000AAFEC File Offset: 0x000A9FEC
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x17000896 RID: 2198
			// (get) Token: 0x06001BAB RID: 7083 RVA: 0x000AB040 File Offset: 0x000AA040
			public string HostNameOrIP
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_HostNameOrIP;
				}
			}

			// Token: 0x17000897 RID: 2199
			// (get) Token: 0x06001BAC RID: 7084 RVA: 0x000AB078 File Offset: 0x000AA078
			public IPAddress[] Addresses
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Addresses' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					List<IPAddress> list = new List<IPAddress>();
					list.AddRange(this.m_pIPv4Addresses);
					list.AddRange(this.m_pIPv6Addresses);
					return list.ToArray();
				}
			}

			// Token: 0x140000CE RID: 206
			// (add) Token: 0x06001BAD RID: 7085 RVA: 0x000AB104 File Offset: 0x000AA104
			// (remove) Token: 0x06001BAE RID: 7086 RVA: 0x000AB13C File Offset: 0x000AA13C
			
			public event EventHandler<EventArgs<Dns_Client.GetHostAddressesAsyncOP>> CompletedAsync = null;

			// Token: 0x06001BAF RID: 7087 RVA: 0x000AB174 File Offset: 0x000AA174
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<Dns_Client.GetHostAddressesAsyncOP>(this));
				}
			}

			// Token: 0x04000CDA RID: 3290
			private object m_pLock = new object();

			// Token: 0x04000CDB RID: 3291
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000CDC RID: 3292
			private Exception m_pException = null;

			// Token: 0x04000CDD RID: 3293
			private string m_HostNameOrIP = null;

			// Token: 0x04000CDE RID: 3294
			private List<IPAddress> m_pIPv4Addresses = null;

			// Token: 0x04000CDF RID: 3295
			private List<IPAddress> m_pIPv6Addresses = null;

			// Token: 0x04000CE0 RID: 3296
			private int m_Counter = 0;

			// Token: 0x04000CE1 RID: 3297
			private bool m_RiseCompleted = false;
		}

		// Token: 0x02000382 RID: 898
		public class GetHostsAddressesAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001BB5 RID: 7093 RVA: 0x000AB500 File Offset: 0x000AA500
			public GetHostsAddressesAsyncOP(string[] hostNames) : this(hostNames, false)
			{
			}

			// Token: 0x06001BB6 RID: 7094 RVA: 0x000AB50C File Offset: 0x000AA50C
			public GetHostsAddressesAsyncOP(string[] hostNames, bool resolveAny)
			{
				bool flag = hostNames == null;
				if (flag)
				{
					throw new ArgumentNullException("hostNames");
				}
				this.m_pHostNames = hostNames;
				this.m_ResolveAny = resolveAny;
				this.m_pIpLookupQueue = new Dictionary<int, Dns_Client.GetHostAddressesAsyncOP>();
			}

			// Token: 0x06001BB7 RID: 7095 RVA: 0x000AB598 File Offset: 0x000AA598
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pHostNames = null;
					this.m_pIpLookupQueue = null;
					this.m_pHostEntries = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001BB8 RID: 7096 RVA: 0x000AB5E4 File Offset: 0x000AA5E4
			internal bool Start(Dns_Client dnsClient)
			{
				bool flag = dnsClient == null;
				if (flag)
				{
					throw new ArgumentNullException("dnsClient");
				}
				this.SetState(AsyncOP_State.Active);
				this.m_pHostEntries = new HostEntry[this.m_pHostNames.Length];
				Dictionary<int, Dns_Client.GetHostAddressesAsyncOP> dictionary = new Dictionary<int, Dns_Client.GetHostAddressesAsyncOP>();
				for (int i = 0; i < this.m_pHostNames.Length; i++)
				{
					Dns_Client.GetHostAddressesAsyncOP value = new Dns_Client.GetHostAddressesAsyncOP(this.m_pHostNames[i]);
					this.m_pIpLookupQueue.Add(i, value);
					dictionary.Add(i, value);
				}
				foreach (KeyValuePair<int, Dns_Client.GetHostAddressesAsyncOP> keyValuePair in dictionary)
				{
					int index = keyValuePair.Key;
					keyValuePair.Value.CompletedAsync += delegate(object s1, EventArgs<Dns_Client.GetHostAddressesAsyncOP> e1)
					{
						this.GetHostAddressesCompleted(e1.Value, index);
					};
					bool flag2 = !dnsClient.GetHostAddressesAsync(keyValuePair.Value);
					if (flag2)
					{
						this.GetHostAddressesCompleted(keyValuePair.Value, index);
					}
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001BB9 RID: 7097 RVA: 0x000AB750 File Offset: 0x000AA750
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001BBA RID: 7098 RVA: 0x000AB7C8 File Offset: 0x000AA7C8
			private void GetHostAddressesCompleted(Dns_Client.GetHostAddressesAsyncOP op, int index)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					try
					{
						bool flag2 = op.Error != null;
						if (flag2)
						{
							bool flag3 = this.m_ResolveAny && (this.m_ResolvedCount > 0 || this.m_pIpLookupQueue.Count > 1);
							if (!flag3)
							{
								this.m_pException = op.Error;
							}
						}
						else
						{
							this.m_pHostEntries[index] = new HostEntry(op.HostNameOrIP, op.Addresses, null);
							this.m_ResolvedCount++;
						}
						this.m_pIpLookupQueue.Remove(index);
						bool flag4 = this.m_pIpLookupQueue.Count == 0;
						if (flag4)
						{
							bool resolveAny = this.m_ResolveAny;
							if (resolveAny)
							{
								List<HostEntry> list = new List<HostEntry>();
								foreach (HostEntry hostEntry in this.m_pHostEntries)
								{
									bool flag5 = hostEntry != null;
									if (flag5)
									{
										list.Add(hostEntry);
									}
								}
								this.m_pHostEntries = list.ToArray();
							}
							this.SetState(AsyncOP_State.Completed);
						}
					}
					catch (Exception pException)
					{
						this.m_pException = pException;
						this.SetState(AsyncOP_State.Completed);
					}
				}
				op.Dispose();
			}

			// Token: 0x17000898 RID: 2200
			// (get) Token: 0x06001BBB RID: 7099 RVA: 0x000AB94C File Offset: 0x000AA94C
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000899 RID: 2201
			// (get) Token: 0x06001BBC RID: 7100 RVA: 0x000AB964 File Offset: 0x000AA964
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x1700089A RID: 2202
			// (get) Token: 0x06001BBD RID: 7101 RVA: 0x000AB9B8 File Offset: 0x000AA9B8
			public string[] HostNames
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_pHostNames;
				}
			}

			// Token: 0x1700089B RID: 2203
			// (get) Token: 0x06001BBE RID: 7102 RVA: 0x000AB9F0 File Offset: 0x000AA9F0
			public HostEntry[] HostEntries
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'HostEntries' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					return this.m_pHostEntries;
				}
			}

			// Token: 0x140000CF RID: 207
			// (add) Token: 0x06001BBF RID: 7103 RVA: 0x000ABA58 File Offset: 0x000AAA58
			// (remove) Token: 0x06001BC0 RID: 7104 RVA: 0x000ABA90 File Offset: 0x000AAA90
			
			public event EventHandler<EventArgs<Dns_Client.GetHostsAddressesAsyncOP>> CompletedAsync = null;

			// Token: 0x06001BC1 RID: 7105 RVA: 0x000ABAC8 File Offset: 0x000AAAC8
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<Dns_Client.GetHostsAddressesAsyncOP>(this));
				}
			}

			// Token: 0x04000CE3 RID: 3299
			private object m_pLock = new object();

			// Token: 0x04000CE4 RID: 3300
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000CE5 RID: 3301
			private Exception m_pException = null;

			// Token: 0x04000CE6 RID: 3302
			private string[] m_pHostNames = null;

			// Token: 0x04000CE7 RID: 3303
			private bool m_ResolveAny = false;

			// Token: 0x04000CE8 RID: 3304
			private Dictionary<int, Dns_Client.GetHostAddressesAsyncOP> m_pIpLookupQueue = null;

			// Token: 0x04000CE9 RID: 3305
			private HostEntry[] m_pHostEntries = null;

			// Token: 0x04000CEA RID: 3306
			private bool m_RiseCompleted = false;

			// Token: 0x04000CEB RID: 3307
			private int m_ResolvedCount = 0;
		}

		// Token: 0x02000383 RID: 899
		public class GetEmailHostsAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001BC2 RID: 7106 RVA: 0x000ABAF8 File Offset: 0x000AAAF8
			public GetEmailHostsAsyncOP(string domain)
			{
				bool flag = domain == null;
				if (flag)
				{
					throw new ArgumentNullException("domain");
				}
				bool flag2 = domain == string.Empty;
				if (flag2)
				{
					throw new ArgumentException("Argument 'domain' value must be specified.", "domain");
				}
				this.m_Domain = domain;
				bool flag3 = domain.IndexOf("@") > -1;
				if (flag3)
				{
					this.m_Domain = domain.Split(new char[]
					{
						'@'
					}, 2)[1];
				}
			}

			// Token: 0x06001BC3 RID: 7107 RVA: 0x000ABBAC File Offset: 0x000AABAC
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_Domain = null;
					this.m_pHosts = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001BC4 RID: 7108 RVA: 0x000ABBF0 File Offset: 0x000AABF0
			internal bool Start(Dns_Client dnsClient)
			{
				bool flag = dnsClient == null;
				if (flag)
				{
					throw new ArgumentNullException("dnsClient");
				}
				this.SetState(AsyncOP_State.Active);
				try
				{
					this.LookupMX(dnsClient, this.m_Domain, false);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001BC5 RID: 7109 RVA: 0x000ABC94 File Offset: 0x000AAC94
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001BC6 RID: 7110 RVA: 0x000ABD0C File Offset: 0x000AAD0C
			private void LookupMX(Dns_Client dnsClient, string domain, bool domainIsCName)
			{
				bool flag = dnsClient == null;
				if (flag)
				{
					throw new ArgumentNullException("dnsClient");
				}
				bool flag2 = domain == null;
				if (flag2)
				{
					throw new ArgumentNullException("domain");
				}
				DNS_ClientTransaction transaction_MX = dnsClient.CreateTransaction(DNS_QType.MX, domain, 2000);
				transaction_MX.StateChanged += delegate(object s1, EventArgs<DNS_ClientTransaction> e1)
				{
					try
					{
						bool flag3 = e1.Value.State == DNS_ClientTransactionState.Completed;
						if (flag3)
						{
							bool flag4 = e1.Value.Response.ResponseCode == DNS_RCode.NO_ERROR;
							if (flag4)
							{
								List<DNS_rr_MX> list = new List<DNS_rr_MX>();
								foreach (DNS_rr_MX dns_rr_MX in e1.Value.Response.GetMXRecords())
								{
									bool flag5 = string.IsNullOrEmpty(dns_rr_MX.Host);
									if (!flag5)
									{
										list.Add(dns_rr_MX);
									}
								}
								bool flag6 = list.Count > 0;
								if (flag6)
								{
									this.m_pHosts = new HostEntry[list.Count];
									Dictionary<string, int> name_to_index_map = new Dictionary<string, int>();
									List<string> list2 = new List<string>();
									for (int j = 0; j < this.m_pHosts.Length; j++)
									{
										DNS_rr_MX dns_rr_MX2 = list[j];
										IPAddress[] array = this.Get_A_or_AAAA_FromResponse(dns_rr_MX2.Host, e1.Value.Response);
										bool flag7 = array.Length == 0;
										if (flag7)
										{
											name_to_index_map[dns_rr_MX2.Host] = j;
											list2.Add(dns_rr_MX2.Host);
										}
										else
										{
											this.m_pHosts[j] = new HostEntry(dns_rr_MX2.Host, array, null);
										}
									}
									bool flag8 = list2.Count > 0;
									if (flag8)
									{
										Dns_Client.GetHostsAddressesAsyncOP op = new Dns_Client.GetHostsAddressesAsyncOP(list2.ToArray(), true);
										op.CompletedAsync += delegate(object s2, EventArgs<Dns_Client.GetHostsAddressesAsyncOP> e2)
										{
											this.LookupCompleted(op, name_to_index_map);
										};
										bool flag9 = !dnsClient.GetHostsAddressesAsync(op);
										if (flag9)
										{
											this.LookupCompleted(op, name_to_index_map);
										}
									}
									else
									{
										this.SetState(AsyncOP_State.Completed);
									}
								}
								else
								{
									bool flag10 = e1.Value.Response.GetCNAMERecords().Length != 0;
									if (flag10)
									{
										bool domainIsCName2 = domainIsCName;
										if (domainIsCName2)
										{
											this.m_pException = new Exception("CNAME to CNAME loop dedected.");
											this.SetState(AsyncOP_State.Completed);
										}
										else
										{
											this.LookupMX(dnsClient, e1.Value.Response.GetCNAMERecords()[0].Alias, true);
										}
									}
									else
									{
										this.m_pHosts = new HostEntry[1];
										Dictionary<string, int> name_to_index_map = new Dictionary<string, int>();
										name_to_index_map.Add(domain, 0);
										Dns_Client.GetHostsAddressesAsyncOP op = new Dns_Client.GetHostsAddressesAsyncOP(new string[]
										{
											domain
										});
										op.CompletedAsync += delegate(object s2, EventArgs<Dns_Client.GetHostsAddressesAsyncOP> e2)
										{
											this.LookupCompleted(op, name_to_index_map);
										};
										bool flag11 = !dnsClient.GetHostsAddressesAsync(op);
										if (flag11)
										{
											this.LookupCompleted(op, name_to_index_map);
										}
									}
								}
							}
							else
							{
								this.m_pException = new DNS_ClientException(e1.Value.Response.ResponseCode);
								this.SetState(AsyncOP_State.Completed);
							}
						}
						transaction_MX.Timeout += delegate(object s2, EventArgs e2)
						{
							this.m_pException = new IOException("DNS transaction timeout, no response from DNS server.");
							this.SetState(AsyncOP_State.Completed);
						};
					}
					catch (Exception pException)
					{
						this.m_pException = pException;
						this.SetState(AsyncOP_State.Completed);
					}
				};
				transaction_MX.Start();
			}

			// Token: 0x06001BC7 RID: 7111 RVA: 0x000ABDB0 File Offset: 0x000AADB0
			private IPAddress[] Get_A_or_AAAA_FromResponse(string name, DnsServerResponse response)
			{
				bool flag = name == null;
				if (flag)
				{
					throw new ArgumentNullException("name");
				}
				bool flag2 = response == null;
				if (flag2)
				{
					throw new ArgumentNullException("response");
				}
				List<IPAddress> list = new List<IPAddress>();
				List<IPAddress> list2 = new List<IPAddress>();
				foreach (DNS_rr dns_rr in response.AdditionalAnswers)
				{
					bool flag3 = string.Equals(name, dns_rr.Name, StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						bool flag4 = dns_rr is DNS_rr_A;
						if (flag4)
						{
							list.Add(((DNS_rr_A)dns_rr).IP);
						}
						else
						{
							bool flag5 = dns_rr is DNS_rr_AAAA;
							if (flag5)
							{
								list2.Add(((DNS_rr_AAAA)dns_rr).IP);
							}
						}
					}
				}
				list.AddRange(list2);
				return list.ToArray();
			}

			// Token: 0x06001BC8 RID: 7112 RVA: 0x000ABE90 File Offset: 0x000AAE90
			private void LookupCompleted(Dns_Client.GetHostsAddressesAsyncOP op, Dictionary<string, int> name_to_index)
			{
				bool flag = op == null;
				if (flag)
				{
					throw new ArgumentNullException("op");
				}
				bool flag2 = op.Error != null;
				if (flag2)
				{
					bool flag3 = false;
					foreach (HostEntry hostEntry in this.m_pHosts)
					{
						bool flag4 = hostEntry != null;
						if (flag4)
						{
							flag3 = true;
							break;
						}
					}
					bool flag5 = !flag3;
					if (flag5)
					{
						this.m_pException = op.Error;
					}
				}
				else
				{
					foreach (HostEntry hostEntry2 in op.HostEntries)
					{
						this.m_pHosts[name_to_index[hostEntry2.HostName]] = hostEntry2;
					}
				}
				op.Dispose();
				List<HostEntry> list = new List<HostEntry>();
				foreach (HostEntry hostEntry3 in this.m_pHosts)
				{
					bool flag6 = hostEntry3 != null;
					if (flag6)
					{
						list.Add(hostEntry3);
					}
				}
				this.m_pHosts = list.ToArray();
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x1700089C RID: 2204
			// (get) Token: 0x06001BC9 RID: 7113 RVA: 0x000ABFAC File Offset: 0x000AAFAC
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700089D RID: 2205
			// (get) Token: 0x06001BCA RID: 7114 RVA: 0x000ABFC4 File Offset: 0x000AAFC4
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x1700089E RID: 2206
			// (get) Token: 0x06001BCB RID: 7115 RVA: 0x000AC018 File Offset: 0x000AB018
			public string EmailDomain
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_Domain;
				}
			}

			// Token: 0x1700089F RID: 2207
			// (get) Token: 0x06001BCC RID: 7116 RVA: 0x000AC050 File Offset: 0x000AB050
			public HostEntry[] Hosts
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					return this.m_pHosts;
				}
			}

			// Token: 0x140000D0 RID: 208
			// (add) Token: 0x06001BCD RID: 7117 RVA: 0x000AC0B8 File Offset: 0x000AB0B8
			// (remove) Token: 0x06001BCE RID: 7118 RVA: 0x000AC0F0 File Offset: 0x000AB0F0
			
			public event EventHandler<EventArgs<Dns_Client.GetEmailHostsAsyncOP>> CompletedAsync = null;

			// Token: 0x06001BCF RID: 7119 RVA: 0x000AC128 File Offset: 0x000AB128
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<Dns_Client.GetEmailHostsAsyncOP>(this));
				}
			}

			// Token: 0x04000CED RID: 3309
			private object m_pLock = new object();

			// Token: 0x04000CEE RID: 3310
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000CEF RID: 3311
			private Exception m_pException = null;

			// Token: 0x04000CF0 RID: 3312
			private string m_Domain = null;

			// Token: 0x04000CF1 RID: 3313
			private HostEntry[] m_pHosts = null;

			// Token: 0x04000CF2 RID: 3314
			private bool m_RiseCompleted = false;
		}
	}
}
