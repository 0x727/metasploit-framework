using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.DNS;
using LumiSoft.Net.DNS.Client;
using LumiSoft.Net.Log;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x020000A1 RID: 161
	public class SIP_Stack
	{
		// Token: 0x060005F9 RID: 1529 RVA: 0x00021AF0 File Offset: 0x00020AF0
		public SIP_Stack()
		{
			this.m_pTransportLayer = new SIP_TransportLayer(this);
			this.m_pTransactionLayer = new SIP_TransactionLayer(this);
			this.m_pNonceManager = new Auth_HttpDigest_NonceManager();
			this.m_pProxyServers = new List<SIP_Uri>();
			this.m_pRegistrations = new List<SIP_UA_Registration>();
			this.m_pCredentials = new List<NetworkCredential>();
			this.m_RegisterCallID = SIP_t_CallID.CreateCallID();
			this.m_pAllow = new List<string>();
			this.m_pAllow.AddRange(new string[]
			{
				"INVITE",
				"ACK",
				"CANCEL",
				"BYE",
				"MESSAGE"
			});
			this.m_pSupported = new List<string>();
			this.m_pLogger = new Logger();
			this.m_pDnsClient = new Dns_Client();
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x00021C88 File Offset: 0x00020C88
		public void Dispose()
		{
			bool flag = this.m_State == SIP_StackState.Disposed;
			if (!flag)
			{
				this.Stop();
				this.m_State = SIP_StackState.Disposed;
				this.RequestReceived = null;
				this.ResponseReceived = null;
				this.Error = null;
				bool flag2 = this.m_pTransactionLayer != null;
				if (flag2)
				{
					this.m_pTransactionLayer.Dispose();
				}
				bool flag3 = this.m_pTransportLayer != null;
				if (flag3)
				{
					this.m_pTransportLayer.Dispose();
				}
				bool flag4 = this.m_pNonceManager != null;
				if (flag4)
				{
					this.m_pNonceManager.Dispose();
				}
				bool flag5 = this.m_pLogger != null;
				if (flag5)
				{
					this.m_pLogger.Dispose();
				}
			}
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x00021D3C File Offset: 0x00020D3C
		public void Start()
		{
			bool flag = this.m_State == SIP_StackState.Started;
			if (!flag)
			{
				this.m_State = SIP_StackState.Started;
				this.m_pTransportLayer.Start();
			}
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00021D70 File Offset: 0x00020D70
		public void Stop()
		{
			bool flag = this.m_State > SIP_StackState.Started;
			if (!flag)
			{
				this.m_State = SIP_StackState.Stopping;
				foreach (SIP_UA_Registration sip_UA_Registration in this.m_pRegistrations.ToArray())
				{
					sip_UA_Registration.BeginUnregister(true);
				}
				foreach (SIP_Dialog sip_Dialog in this.m_pTransactionLayer.Dialogs)
				{
					sip_Dialog.Terminate();
				}
				DateTime now = DateTime.Now;
				bool flag5;
				do
				{
					bool flag2 = false;
					foreach (SIP_Transaction sip_Transaction in this.m_pTransactionLayer.Transactions)
					{
						bool flag3 = sip_Transaction.State == SIP_TransactionState.WaitingToStart || sip_Transaction.State == SIP_TransactionState.Calling || sip_Transaction.State == SIP_TransactionState.Proceeding || sip_Transaction.State == SIP_TransactionState.Trying;
						if (flag3)
						{
							flag2 = true;
							break;
						}
					}
					bool flag4 = flag2;
					if (!flag4)
					{
						break;
					}
					Thread.Sleep(500);
					flag5 = ((DateTime.Now - now).Seconds > 10);
				}
				while (!flag5);
				foreach (SIP_Transaction sip_Transaction2 in this.m_pTransactionLayer.Transactions)
				{
					try
					{
						sip_Transaction2.Dispose();
					}
					catch
					{
					}
				}
				this.m_pTransportLayer.Stop();
				this.m_State = SIP_StackState.Stopped;
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00021F08 File Offset: 0x00020F08
		public SIP_Request CreateRequest(string method, SIP_t_NameAddress to, SIP_t_NameAddress from)
		{
			bool flag = this.m_State == SIP_StackState.Disposed;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag2 = method == null;
			if (flag2)
			{
				throw new ArgumentNullException("method");
			}
			bool flag3 = method == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'method' value must be specified.");
			}
			bool flag4 = to == null;
			if (flag4)
			{
				throw new ArgumentNullException("to");
			}
			bool flag5 = from == null;
			if (flag5)
			{
				throw new ArgumentNullException("from");
			}
			method = method.ToUpper();
			SIP_Request sip_Request = new SIP_Request(method);
			sip_Request.RequestLine.Uri = to.Uri;
			SIP_t_To to2 = new SIP_t_To(to);
			sip_Request.To = to2;
			sip_Request.From = new SIP_t_From(from)
			{
				Tag = SIP_Utils.CreateTag()
			};
			bool flag6 = method == "REGISTER";
			if (flag6)
			{
				sip_Request.CallID = this.m_RegisterCallID.ToStringValue();
			}
			else
			{
				sip_Request.CallID = SIP_t_CallID.CreateCallID().ToStringValue();
			}
			sip_Request.CSeq = new SIP_t_CSeq(this.ConsumeCSeq(), method);
			sip_Request.MaxForwards = this.m_MaxForwards;
			sip_Request.Allow.Add(SIP_Utils.ListToString(this.m_pAllow));
			bool flag7 = this.m_pSupported.Count > 0;
			if (flag7)
			{
				sip_Request.Supported.Add(SIP_Utils.ListToString(this.m_pAllow));
			}
			foreach (SIP_Uri sip_Uri in this.m_pProxyServers)
			{
				sip_Request.Route.Add(sip_Uri.ToString());
			}
			bool flag8 = !string.IsNullOrEmpty(this.m_UserAgent);
			if (flag8)
			{
				sip_Request.UserAgent = this.m_UserAgent;
			}
			return sip_Request;
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00022100 File Offset: 0x00021100
		public SIP_RequestSender CreateRequestSender(SIP_Request request)
		{
			return this.CreateRequestSender(request, null);
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x0002211C File Offset: 0x0002111C
		internal SIP_RequestSender CreateRequestSender(SIP_Request request, SIP_Flow flow)
		{
			bool flag = this.m_State == SIP_StackState.Disposed;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag2 = request == null;
			if (flag2)
			{
				throw new ArgumentNullException("request");
			}
			SIP_RequestSender sip_RequestSender = new SIP_RequestSender(this, request, flow);
			sip_RequestSender.Credentials.AddRange(this.m_pCredentials);
			return sip_RequestSender;
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00022180 File Offset: 0x00021180
		public int ConsumeCSeq()
		{
			int cseq = this.m_CSeq;
			this.m_CSeq = cseq + 1;
			return cseq;
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x000221A4 File Offset: 0x000211A4
		public SIP_Response CreateResponse(string statusCode_reasonText, SIP_Request request)
		{
			return this.CreateResponse(statusCode_reasonText, request, null);
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x000221C0 File Offset: 0x000211C0
		public SIP_Response CreateResponse(string statusCode_reasonText, SIP_Request request, SIP_Flow flow)
		{
			bool flag = request == null;
			if (flag)
			{
				throw new ArgumentNullException("request");
			}
			bool flag2 = request.RequestLine.Method == "ACK";
			if (flag2)
			{
				throw new InvalidOperationException("ACK is responseless request !");
			}
			SIP_Response sip_Response = new SIP_Response(request);
			sip_Response.StatusCode_ReasonPhrase = statusCode_reasonText;
			foreach (SIP_t_ViaParm sip_t_ViaParm in request.Via.GetAllValues())
			{
				sip_Response.Via.Add(sip_t_ViaParm.ToStringValue());
			}
			sip_Response.From = request.From;
			sip_Response.To = request.To;
			bool flag3 = request.To.Tag == null;
			if (flag3)
			{
				sip_Response.To.Tag = SIP_Utils.CreateTag();
			}
			sip_Response.CallID = request.CallID;
			sip_Response.CSeq = request.CSeq;
			sip_Response.Allow.Add(SIP_Utils.ListToString(this.m_pAllow));
			bool flag4 = this.m_pSupported.Count > 0;
			if (flag4)
			{
				sip_Response.Supported.Add(SIP_Utils.ListToString(this.m_pAllow));
			}
			bool flag5 = !string.IsNullOrEmpty(this.m_UserAgent);
			if (flag5)
			{
				request.UserAgent = this.m_UserAgent;
			}
			bool flag6 = SIP_Utils.MethodCanEstablishDialog(request.RequestLine.Method);
			if (flag6)
			{
				foreach (SIP_t_AddressParam sip_t_AddressParam in request.RecordRoute.GetAllValues())
				{
					sip_Response.RecordRoute.Add(sip_t_AddressParam.ToStringValue());
				}
				bool flag7 = sip_Response.Contact.GetTopMostValue() == null && flow != null;
				if (flag7)
				{
					string user = ((SIP_Uri)sip_Response.To.Address.Uri).User;
					sip_Response.Contact.Add((flow.IsSecure ? "sips:" : "sip:") + user + "@" + flow.LocalPublicEP.ToString());
				}
			}
			return sip_Response;
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x000223E0 File Offset: 0x000213E0
		public SIP_Hop[] GetHops(SIP_Uri uri, int messageSize, bool forceTLS)
		{
			bool flag = uri == null;
			if (flag)
			{
				throw new ArgumentNullException("uri");
			}
			List<SIP_Hop> list = new List<SIP_Hop>();
			string text = "";
			bool flag2 = false;
			List<DNS_rr_SRV> list2 = new List<DNS_rr_SRV>();
			if (forceTLS)
			{
				flag2 = true;
				text = "TLS";
			}
			else
			{
				bool flag3 = uri.Param_Transport != null;
				if (flag3)
				{
					flag2 = true;
					text = uri.Param_Transport;
				}
				else
				{
					bool flag4 = Net_Utils.IsIPAddress(uri.Host) || uri.Port != -1;
					if (flag4)
					{
						bool isSecure = uri.IsSecure;
						if (isSecure)
						{
							text = "TLS";
						}
						else
						{
							bool flag5 = messageSize > this.MTU;
							if (flag5)
							{
								text = "TCP";
							}
							else
							{
								text = "UDP";
							}
						}
					}
					else
					{
						Dictionary<string, DNS_rr_SRV[]> dictionary = new Dictionary<string, DNS_rr_SRV[]>();
						bool flag6 = false;
						DnsServerResponse dnsServerResponse = this.m_pDnsClient.Query("_sips._tcp." + uri.Host, DNS_QType.SRV);
						bool flag7 = dnsServerResponse.GetSRVRecords().Length != 0;
						if (flag7)
						{
							flag6 = true;
							dictionary.Add("TLS", dnsServerResponse.GetSRVRecords());
						}
						dnsServerResponse = this.m_pDnsClient.Query("_sip._tcp." + uri.Host, DNS_QType.SRV);
						bool flag8 = dnsServerResponse.GetSRVRecords().Length != 0;
						if (flag8)
						{
							flag6 = true;
							dictionary.Add("TCP", dnsServerResponse.GetSRVRecords());
						}
						dnsServerResponse = this.m_pDnsClient.Query("_sip._udp." + uri.Host, DNS_QType.SRV);
						bool flag9 = dnsServerResponse.GetSRVRecords().Length != 0;
						if (flag9)
						{
							flag6 = true;
							dictionary.Add("UDP", dnsServerResponse.GetSRVRecords());
						}
						bool flag10 = flag6;
						if (flag10)
						{
							bool isSecure2 = uri.IsSecure;
							if (isSecure2)
							{
								bool flag11 = dictionary.ContainsKey("TLS");
								if (flag11)
								{
									text = "TLS";
									list2.AddRange(dictionary["TLS"]);
								}
							}
							else
							{
								bool flag12 = messageSize > this.MTU;
								if (flag12)
								{
									bool flag13 = dictionary.ContainsKey("TCP");
									if (flag13)
									{
										text = "TCP";
										list2.AddRange(dictionary["TCP"]);
									}
									else
									{
										bool flag14 = dictionary.ContainsKey("TLS");
										if (flag14)
										{
											text = "TLS";
											list2.AddRange(dictionary["TLS"]);
										}
									}
								}
								else
								{
									bool flag15 = dictionary.ContainsKey("UDP");
									if (flag15)
									{
										text = "UDP";
										list2.AddRange(dictionary["UDP"]);
									}
									else
									{
										bool flag16 = dictionary.ContainsKey("TCP");
										if (flag16)
										{
											text = "TCP";
											list2.AddRange(dictionary["TCP"]);
										}
										else
										{
											text = "TLS";
											list2.AddRange(dictionary["TLS"]);
										}
									}
								}
							}
						}
						else
						{
							bool isSecure3 = uri.IsSecure;
							if (isSecure3)
							{
								text = "TLS";
							}
							else
							{
								bool flag17 = messageSize > this.MTU;
								if (flag17)
								{
									text = "TCP";
								}
								else
								{
									text = "UDP";
								}
							}
						}
					}
				}
			}
			bool flag18 = Net_Utils.IsIPAddress(uri.Host);
			if (flag18)
			{
				bool flag19 = uri.Port != -1;
				if (flag19)
				{
					list.Add(new SIP_Hop(IPAddress.Parse(uri.Host), uri.Port, text));
				}
				else
				{
					bool flag20 = forceTLS || uri.IsSecure;
					if (flag20)
					{
						list.Add(new SIP_Hop(IPAddress.Parse(uri.Host), 5061, text));
					}
					else
					{
						list.Add(new SIP_Hop(IPAddress.Parse(uri.Host), 5060, text));
					}
				}
			}
			else
			{
				bool flag21 = uri.Port != -1;
				if (flag21)
				{
					foreach (IPAddress ip in this.m_pDnsClient.GetHostAddresses(uri.Host))
					{
						list.Add(new SIP_Hop(ip, uri.Port, text));
					}
				}
				else
				{
					bool flag22 = flag2;
					if (flag22)
					{
						bool flag23 = text == "TLS";
						DnsServerResponse dnsServerResponse2;
						if (flag23)
						{
							dnsServerResponse2 = this.m_pDnsClient.Query("_sips._tcp." + uri.Host, DNS_QType.SRV);
						}
						else
						{
							bool flag24 = text == "TCP";
							if (flag24)
							{
								dnsServerResponse2 = this.m_pDnsClient.Query("_sip._tcp." + uri.Host, DNS_QType.SRV);
							}
							else
							{
								dnsServerResponse2 = this.m_pDnsClient.Query("_sip._udp." + uri.Host, DNS_QType.SRV);
							}
						}
						list2.AddRange(dnsServerResponse2.GetSRVRecords());
					}
					bool flag25 = list2.Count > 0;
					if (flag25)
					{
						foreach (DNS_rr_SRV dns_rr_SRV in list2)
						{
							bool flag26 = Net_Utils.IsIPAddress(dns_rr_SRV.Target);
							if (flag26)
							{
								list.Add(new SIP_Hop(IPAddress.Parse(dns_rr_SRV.Target), dns_rr_SRV.Port, text));
							}
							else
							{
								foreach (IPAddress ip2 in this.m_pDnsClient.GetHostAddresses(dns_rr_SRV.Target))
								{
									list.Add(new SIP_Hop(ip2, dns_rr_SRV.Port, text));
								}
							}
						}
					}
					else
					{
						int port = 5060;
						bool flag27 = text == "TLS";
						if (flag27)
						{
							port = 5061;
						}
						foreach (IPAddress ip3 in this.m_pDnsClient.GetHostAddresses(uri.Host))
						{
							list.Add(new SIP_Hop(ip3, port, text));
						}
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x00022A04 File Offset: 0x00021A04
		public SIP_UA_Registration CreateRegistration(SIP_Uri server, string aor, AbsoluteUri contact, int expires)
		{
			bool flag = this.m_State == SIP_StackState.Disposed;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag2 = server == null;
			if (flag2)
			{
				throw new ArgumentNullException("server");
			}
			bool flag3 = aor == null;
			if (flag3)
			{
				throw new ArgumentNullException("aor");
			}
			bool flag4 = aor == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("Argument 'aor' value must be specified.");
			}
			bool flag5 = contact == null;
			if (flag5)
			{
				throw new ArgumentNullException("contact");
			}
			List<SIP_UA_Registration> pRegistrations = this.m_pRegistrations;
			bool flag6 = false;
			SIP_UA_Registration registration2;
			try
			{
				Monitor.Enter(pRegistrations, ref flag6);
				SIP_UA_Registration registration = new SIP_UA_Registration(this, server, aor, contact, expires);
				registration.Disposed += delegate(object s, EventArgs e)
				{
					bool flag7 = this.m_State != SIP_StackState.Disposed;
					if (flag7)
					{
						this.m_pRegistrations.Remove(registration);
					}
				};
				this.m_pRegistrations.Add(registration);
				registration2 = registration;
			}
			finally
			{
				if (flag6)
				{
					Monitor.Exit(pRegistrations);
				}
			}
			return registration2;
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x00022B18 File Offset: 0x00021B18
		public SIP_StackState State
		{
			get
			{
				return this.m_State;
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x00022B30 File Offset: 0x00021B30
		public SIP_TransportLayer TransportLayer
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pTransportLayer;
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000607 RID: 1543 RVA: 0x00022B68 File Offset: 0x00021B68
		public SIP_TransactionLayer TransactionLayer
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pTransactionLayer;
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x00022BA0 File Offset: 0x00021BA0
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x00022BD8 File Offset: 0x00021BD8
		public string UserAgent
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_UserAgent;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_UserAgent = value;
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x00022C0C File Offset: 0x00021C0C
		public Auth_HttpDigest_NonceManager DigestNonceManager
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pNonceManager;
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x0600060B RID: 1547 RVA: 0x00022C44 File Offset: 0x00021C44
		// (set) Token: 0x0600060C RID: 1548 RVA: 0x00022C61 File Offset: 0x00021C61
		public string StunServer
		{
			get
			{
				return this.m_pTransportLayer.StunServer;
			}
			set
			{
				this.m_pTransportLayer.StunServer = value;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x0600060D RID: 1549 RVA: 0x00022C74 File Offset: 0x00021C74
		public List<SIP_Uri> ProxyServers
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pProxyServers;
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x00022CAC File Offset: 0x00021CAC
		// (set) Token: 0x0600060F RID: 1551 RVA: 0x00022CE4 File Offset: 0x00021CE4
		public string Realm
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Realm;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = value == null;
				if (flag2)
				{
					throw new ArgumentNullException();
				}
				this.m_Realm = value;
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x00022D28 File Offset: 0x00021D28
		// (set) Token: 0x06000611 RID: 1553 RVA: 0x00022D60 File Offset: 0x00021D60
		public int MaxForwards
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxForwards;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = value < 1;
				if (flag2)
				{
					throw new ArgumentException("Value must be > 0.");
				}
				this.m_MaxForwards = value;
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x00022DA8 File Offset: 0x00021DA8
		// (set) Token: 0x06000613 RID: 1555 RVA: 0x00022DE0 File Offset: 0x00021DE0
		public int MinimumExpireTime
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MinExpireTime;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = value < 10;
				if (flag2)
				{
					throw new ArgumentException("Property MinimumExpireTime value must be >= 10 !");
				}
				this.m_MinExpireTime = value;
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x00022E2C File Offset: 0x00021E2C
		public List<string> Allow
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAllow;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000615 RID: 1557 RVA: 0x00022E64 File Offset: 0x00021E64
		public List<string> Supported
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSupported;
			}
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x00022E9C File Offset: 0x00021E9C
		// (set) Token: 0x06000617 RID: 1559 RVA: 0x00022ED4 File Offset: 0x00021ED4
		public int MaximumConnections
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaximumConnections;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = value < 1;
				if (flag2)
				{
					this.m_MaximumConnections = 0;
				}
				else
				{
					this.m_MaximumConnections = value;
				}
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x00022F20 File Offset: 0x00021F20
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x00022F58 File Offset: 0x00021F58
		public int MaximumMessageSize
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaximumMessageSize;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = value < 1;
				if (flag2)
				{
					this.m_MaximumMessageSize = 0;
				}
				else
				{
					this.m_MaximumMessageSize = value;
				}
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x00022FA4 File Offset: 0x00021FA4
		// (set) Token: 0x0600061B RID: 1563 RVA: 0x00022FDC File Offset: 0x00021FDC
		public int MinimumSessionExpries
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MinSessionExpires;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = value < 90;
				if (flag2)
				{
					throw new ArgumentException("Minimum session expires value must be >= 90 !");
				}
				this.m_MinSessionExpires = value;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x0600061C RID: 1564 RVA: 0x00023028 File Offset: 0x00022028
		// (set) Token: 0x0600061D RID: 1565 RVA: 0x00023060 File Offset: 0x00022060
		public int SessionExpries
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SessionExpires;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = value < 90;
				if (flag2)
				{
					throw new ArgumentException("Session expires value can't be < MinimumSessionExpries value !");
				}
				this.m_SessionExpires = value;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x000230AC File Offset: 0x000220AC
		public List<NetworkCredential> Credentials
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pCredentials;
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x000230E4 File Offset: 0x000220E4
		// (set) Token: 0x06000620 RID: 1568 RVA: 0x00023120 File Offset: 0x00022120
		public IPBindInfo[] BindInfo
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pTransportLayer.BindInfo;
			}
			set
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_pTransportLayer.BindInfo = value;
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x0002315C File Offset: 0x0002215C
		public Dns_Client Dns
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pDnsClient;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x00023194 File Offset: 0x00022194
		public Logger Logger
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pLogger;
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000623 RID: 1571 RVA: 0x000231CC File Offset: 0x000221CC
		public SIP_UA_Registration[] Registrations
		{
			get
			{
				bool flag = this.m_State == SIP_StackState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRegistrations.ToArray();
			}
		}

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06000624 RID: 1572 RVA: 0x00023208 File Offset: 0x00022208
		// (remove) Token: 0x06000625 RID: 1573 RVA: 0x00023240 File Offset: 0x00022240
		
		public event EventHandler<SIP_ValidateRequestEventArgs> ValidateRequest = null;

		// Token: 0x06000626 RID: 1574 RVA: 0x00023278 File Offset: 0x00022278
		internal SIP_ValidateRequestEventArgs OnValidateRequest(SIP_Request request, IPEndPoint remoteEndPoint)
		{
			SIP_ValidateRequestEventArgs sip_ValidateRequestEventArgs = new SIP_ValidateRequestEventArgs(request, remoteEndPoint);
			bool flag = this.ValidateRequest != null;
			if (flag)
			{
				this.ValidateRequest(this, sip_ValidateRequestEventArgs);
			}
			return sip_ValidateRequestEventArgs;
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06000627 RID: 1575 RVA: 0x000232B0 File Offset: 0x000222B0
		// (remove) Token: 0x06000628 RID: 1576 RVA: 0x000232E8 File Offset: 0x000222E8
		
		public event EventHandler<SIP_RequestReceivedEventArgs> RequestReceived = null;

		// Token: 0x06000629 RID: 1577 RVA: 0x00023320 File Offset: 0x00022320
		internal void OnRequestReceived(SIP_RequestReceivedEventArgs e)
		{
			bool flag = this.RequestReceived != null;
			if (flag)
			{
				this.RequestReceived(this, e);
			}
		}

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x0600062A RID: 1578 RVA: 0x0002334C File Offset: 0x0002234C
		// (remove) Token: 0x0600062B RID: 1579 RVA: 0x00023384 File Offset: 0x00022384
		
		public event EventHandler<SIP_ResponseReceivedEventArgs> ResponseReceived = null;

		// Token: 0x0600062C RID: 1580 RVA: 0x000233BC File Offset: 0x000223BC
		internal void OnResponseReceived(SIP_ResponseReceivedEventArgs e)
		{
			bool flag = this.ResponseReceived != null;
			if (flag)
			{
				this.ResponseReceived(this, e);
			}
		}

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x0600062D RID: 1581 RVA: 0x000233E8 File Offset: 0x000223E8
		// (remove) Token: 0x0600062E RID: 1582 RVA: 0x00023420 File Offset: 0x00022420
		
		public event EventHandler<ExceptionEventArgs> Error = null;

		// Token: 0x0600062F RID: 1583 RVA: 0x00023458 File Offset: 0x00022458
		internal void OnError(Exception x)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, new ExceptionEventArgs(x));
			}
		}

		// Token: 0x04000279 RID: 633
		private SIP_StackState m_State = SIP_StackState.Stopped;

		// Token: 0x0400027A RID: 634
		private SIP_TransportLayer m_pTransportLayer = null;

		// Token: 0x0400027B RID: 635
		private SIP_TransactionLayer m_pTransactionLayer = null;

		// Token: 0x0400027C RID: 636
		private string m_UserAgent = null;

		// Token: 0x0400027D RID: 637
		private Auth_HttpDigest_NonceManager m_pNonceManager = null;

		// Token: 0x0400027E RID: 638
		private List<SIP_Uri> m_pProxyServers = null;

		// Token: 0x0400027F RID: 639
		private string m_Realm = "";

		// Token: 0x04000280 RID: 640
		private int m_CSeq = 1;

		// Token: 0x04000281 RID: 641
		private int m_MaxForwards = 70;

		// Token: 0x04000282 RID: 642
		private int m_MinExpireTime = 1800;

		// Token: 0x04000283 RID: 643
		private List<string> m_pAllow = null;

		// Token: 0x04000284 RID: 644
		private List<string> m_pSupported = null;

		// Token: 0x04000285 RID: 645
		private int m_MaximumConnections = 0;

		// Token: 0x04000286 RID: 646
		private int m_MaximumMessageSize = 1000000;

		// Token: 0x04000287 RID: 647
		private int m_MinSessionExpires = 90;

		// Token: 0x04000288 RID: 648
		private int m_SessionExpires = 1800;

		// Token: 0x04000289 RID: 649
		private List<NetworkCredential> m_pCredentials = null;

		// Token: 0x0400028A RID: 650
		private List<SIP_UA_Registration> m_pRegistrations = null;

		// Token: 0x0400028B RID: 651
		private SIP_t_CallID m_RegisterCallID = null;

		// Token: 0x0400028C RID: 652
		private Logger m_pLogger = null;

		// Token: 0x0400028D RID: 653
		private Dns_Client m_pDnsClient = null;

		// Token: 0x0400028E RID: 654
		private int MTU = 1400;
	}
}
