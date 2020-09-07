using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using LumiSoft.Net.DNS;
using LumiSoft.Net.DNS.Client;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.TCP;
using LumiSoft.Net.UDP;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x020000A3 RID: 163
	public class SIP_TransportLayer
	{
		// Token: 0x06000640 RID: 1600 RVA: 0x00024178 File Offset: 0x00023178
		internal SIP_TransportLayer(SIP_Stack stack)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			this.m_pStack = stack;
			this.m_pUdpServer = new UDP_Server();
			this.m_pUdpServer.PacketReceived += this.m_pUdpServer_PacketReceived;
			this.m_pUdpServer.Error += this.m_pUdpServer_Error;
			this.m_pTcpServer = new TCP_Server<TCP_ServerSession>();
			this.m_pTcpServer.SessionCreated += this.m_pTcpServer_SessionCreated;
			this.m_pFlowManager = new SIP_TransportLayer.SIP_FlowManager(this);
			this.m_pBinds = new IPBindInfo[0];
			this.m_pRandom = new Random();
			this.m_pLocalIPv4 = new CircleCollection<IPAddress>();
			this.m_pLocalIPv6 = new CircleCollection<IPAddress>();
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x0002428C File Offset: 0x0002328C
		internal void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.Stop();
				this.m_IsRunning = false;
				this.m_pBinds = null;
				this.m_pRandom = null;
				this.m_pTcpServer.Dispose();
				this.m_pTcpServer = null;
				this.m_pUdpServer.Dispose();
				this.m_pUdpServer = null;
			}
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x000242F0 File Offset: 0x000232F0
		private void m_pUdpServer_PacketReceived(object sender, UDP_e_PacketReceived e)
		{
			try
			{
				SIP_Flow orCreateFlow = this.m_pFlowManager.GetOrCreateFlow(true, (IPEndPoint)e.Socket.LocalEndPoint, e.RemoteEP, "UDP");
				orCreateFlow.OnUdpPacketReceived(e);
			}
			catch (Exception x)
			{
				this.m_pStack.OnError(x);
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00024354 File Offset: 0x00023354
		private void m_pUdpServer_Error(object sender, Error_EventArgs e)
		{
			this.m_pStack.OnError(e.Exception);
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00024369 File Offset: 0x00023369
		private void m_pTcpServer_SessionCreated(object sender, TCP_ServerSessionEventArgs<TCP_ServerSession> e)
		{
			this.m_pFlowManager.CreateFromSession(e.Session);
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00024380 File Offset: 0x00023380
		internal void Start()
		{
			bool isRunning = this.m_IsRunning;
			if (!isRunning)
			{
				this.m_IsRunning = true;
				this.m_pUdpServer.Start();
				this.m_pTcpServer.Start();
			}
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000243BC File Offset: 0x000233BC
		internal void Stop()
		{
			bool flag = !this.m_IsRunning;
			if (!flag)
			{
				this.m_IsRunning = false;
				this.m_pUdpServer.Stop();
				this.m_pTcpServer.Stop();
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000243FC File Offset: 0x000233FC
		internal void OnMessageReceived(SIP_Flow flow, byte[] message)
		{
			bool flag = flow == null;
			if (flag)
			{
				throw new ArgumentNullException("flow");
			}
			bool flag2 = message == null;
			if (flag2)
			{
				throw new ArgumentNullException("message");
			}
			try
			{
				bool flag3 = message.Length == 4;
				if (flag3)
				{
					bool flag4 = this.Stack.Logger != null;
					if (flag4)
					{
						this.Stack.Logger.AddRead("", null, 2L, "Flow [id='" + flow.ID + "'] received \"ping\"", flow.LocalEP, flow.RemoteEP);
					}
					flow.SendInternal(new byte[]
					{
						13,
						10
					});
					bool flag5 = this.Stack.Logger != null;
					if (flag5)
					{
						this.Stack.Logger.AddWrite("", null, 2L, "Flow [id='" + flow.ID + "'] sent \"pong\"", flow.LocalEP, flow.RemoteEP);
					}
				}
				else
				{
					bool flag6 = message.Length == 2;
					if (flag6)
					{
						bool flag7 = this.Stack.Logger != null;
						if (flag7)
						{
							this.Stack.Logger.AddRead("", null, 2L, "Flow [id='" + flow.ID + "'] received \"pong\"", flow.LocalEP, flow.RemoteEP);
						}
					}
					else
					{
						bool flag8 = Encoding.UTF8.GetString(message, 0, 3).ToUpper().StartsWith("SIP");
						if (flag8)
						{
							SIP_Response sip_Response = null;
							try
							{
								sip_Response = SIP_Response.Parse(message);
							}
							catch (Exception ex)
							{
								bool flag9 = this.m_pStack.Logger != null;
								if (flag9)
								{
									this.m_pStack.Logger.AddText("Skipping message, parse error: " + ex.ToString());
								}
								return;
							}
							try
							{
								sip_Response.Validate();
							}
							catch (Exception ex2)
							{
								bool flag10 = this.m_pStack.Logger != null;
								if (flag10)
								{
									this.m_pStack.Logger.AddText("Response validation failed: " + ex2.ToString());
								}
								return;
							}
							SIP_ClientTransaction sip_ClientTransaction = this.m_pStack.TransactionLayer.MatchClientTransaction(sip_Response);
							bool flag11 = sip_ClientTransaction != null;
							if (flag11)
							{
								sip_ClientTransaction.ProcessResponse(flow, sip_Response);
							}
							else
							{
								SIP_Dialog sip_Dialog = this.m_pStack.TransactionLayer.MatchDialog(sip_Response);
								bool flag12 = sip_Dialog != null;
								if (flag12)
								{
									sip_Dialog.ProcessResponse(sip_Response);
								}
								else
								{
									this.m_pStack.OnResponseReceived(new SIP_ResponseReceivedEventArgs(this.m_pStack, null, sip_Response));
								}
							}
						}
						else
						{
							SIP_Request sip_Request = null;
							try
							{
								sip_Request = SIP_Request.Parse(message);
							}
							catch (Exception ex3)
							{
								bool flag13 = this.m_pStack.Logger != null;
								if (flag13)
								{
									this.m_pStack.Logger.AddText("Skipping message, parse error: " + ex3.Message);
								}
								return;
							}
							try
							{
								sip_Request.Validate();
							}
							catch (Exception ex4)
							{
								bool flag14 = this.m_pStack.Logger != null;
								if (flag14)
								{
									this.m_pStack.Logger.AddText("Request validation failed: " + ex4.ToString());
								}
								this.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x400_Bad_Request + ". " + ex4.Message, sip_Request));
								return;
							}
							SIP_ValidateRequestEventArgs sip_ValidateRequestEventArgs = this.m_pStack.OnValidateRequest(sip_Request, flow.RemoteEP);
							bool flag15 = sip_ValidateRequestEventArgs.ResponseCode != null;
							if (flag15)
							{
								this.SendResponse(this.m_pStack.CreateResponse(sip_ValidateRequestEventArgs.ResponseCode, sip_Request));
							}
							else
							{
								sip_Request.Flow = flow;
								sip_Request.LocalEndPoint = flow.LocalEP;
								sip_Request.RemoteEndPoint = flow.RemoteEP;
								SIP_t_ViaParm topMostValue = sip_Request.Via.GetTopMostValue();
								topMostValue.Received = flow.RemoteEP.Address;
								bool flag16 = topMostValue.RPort == 0;
								if (flag16)
								{
									topMostValue.RPort = flow.RemoteEP.Port;
								}
								bool flag17 = false;
								SIP_ServerTransaction sip_ServerTransaction = this.m_pStack.TransactionLayer.MatchServerTransaction(sip_Request);
								bool flag18 = sip_ServerTransaction != null;
								if (flag18)
								{
									sip_ServerTransaction.ProcessRequest(flow, sip_Request);
									flag17 = true;
								}
								else
								{
									SIP_Dialog sip_Dialog2 = this.m_pStack.TransactionLayer.MatchDialog(sip_Request);
									bool flag19 = sip_Dialog2 != null;
									if (flag19)
									{
										flag17 = sip_Dialog2.ProcessRequest(new SIP_RequestReceivedEventArgs(this.m_pStack, flow, sip_Request));
									}
								}
								bool flag20 = !flag17;
								if (flag20)
								{
									bool flag21 = this.m_pStack.Logger != null;
									if (flag21)
									{
										byte[] array = sip_Request.ToByteData();
										this.m_pStack.Logger.AddRead(Guid.NewGuid().ToString(), null, 0L, string.Concat(new object[]
										{
											"Request [method='",
											sip_Request.RequestLine.Method,
											"'; cseq='",
											sip_Request.CSeq.SequenceNumber,
											"'; transport='",
											flow.Transport,
											"'; size='",
											array.Length,
											"'; received '",
											flow.RemoteEP,
											"' -> '",
											flow.LocalEP,
											"'."
										}), flow.LocalEP, flow.RemoteEP, array);
									}
									this.m_pStack.OnRequestReceived(new SIP_RequestReceivedEventArgs(this.m_pStack, flow, sip_Request));
								}
							}
						}
					}
				}
			}
			catch (SocketException ex5)
			{
				string message2 = ex5.Message;
			}
			catch (Exception x)
			{
				this.m_pStack.OnError(x);
			}
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00024A54 File Offset: 0x00023A54
		public SIP_Flow GetOrCreateFlow(string transport, IPEndPoint localEP, IPEndPoint remoteEP)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = remoteEP == null;
			if (flag)
			{
				throw new ArgumentNullException("remoteEP");
			}
			bool flag2 = localEP == null;
			if (flag2)
			{
				bool flag3 = string.Equals(transport, "UDP", StringComparison.InvariantCultureIgnoreCase);
				if (flag3)
				{
					localEP = this.m_pUdpServer.GetLocalEndPoint(remoteEP);
				}
				else
				{
					bool flag4 = string.Equals(transport, "TCP", StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						bool flag5 = remoteEP.AddressFamily == AddressFamily.InterNetwork;
						if (flag5)
						{
							localEP = new IPEndPoint(this.m_pLocalIPv4.Next(), this.m_pRandom.Next(10000, 65000));
						}
						else
						{
							localEP = new IPEndPoint(this.m_pLocalIPv4.Next(), this.m_pRandom.Next(10000, 65000));
						}
					}
					else
					{
						bool flag6 = string.Equals(transport, "TLS", StringComparison.InvariantCultureIgnoreCase);
						if (!flag6)
						{
							throw new ArgumentException("Not supported transoprt '" + transport + "'.");
						}
						bool flag7 = remoteEP.AddressFamily == AddressFamily.InterNetwork;
						if (flag7)
						{
							localEP = new IPEndPoint(this.m_pLocalIPv4.Next(), this.m_pRandom.Next(10000, 65000));
						}
						else
						{
							localEP = new IPEndPoint(this.m_pLocalIPv4.Next(), this.m_pRandom.Next(10000, 65000));
						}
					}
				}
			}
			return this.m_pFlowManager.GetOrCreateFlow(false, localEP, remoteEP, transport);
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00024BE8 File Offset: 0x00023BE8
		public SIP_Flow GetFlow(string flowID)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = flowID == null;
			if (flag)
			{
				throw new ArgumentNullException("flowID");
			}
			return this.m_pFlowManager.GetFlow(flowID);
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00024C38 File Offset: 0x00023C38
		public void SendRequest(SIP_Request request)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = request == null;
			if (flag)
			{
				throw new ArgumentNullException("request");
			}
			SIP_Hop[] hops = this.m_pStack.GetHops((SIP_Uri)request.RequestLine.Uri, request.ToByteData().Length, false);
			bool flag2 = hops.Length == 0;
			if (flag2)
			{
				throw new SIP_TransportException("No target hops for URI '" + request.RequestLine.Uri.ToString() + "'.");
			}
			SIP_TransportException ex = null;
			foreach (SIP_Hop hop in hops)
			{
				try
				{
					this.SendRequest(request, null, hop);
					return;
				}
				catch (SIP_TransportException ex2)
				{
					ex = ex2;
				}
			}
			throw ex;
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00024D18 File Offset: 0x00023D18
		public void SendRequest(SIP_Request request, IPEndPoint localEP, SIP_Hop hop)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = request == null;
			if (flag)
			{
				throw new ArgumentNullException("request");
			}
			bool flag2 = hop == null;
			if (flag2)
			{
				throw new ArgumentNullException("hop");
			}
			this.SendRequest(this.GetOrCreateFlow(hop.Transport, localEP, hop.EndPoint), request);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00024D85 File Offset: 0x00023D85
		public void SendRequest(SIP_Flow flow, SIP_Request request)
		{
			this.SendRequest(flow, request, null);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00024D94 File Offset: 0x00023D94
		internal void SendRequest(SIP_Flow flow, SIP_Request request, SIP_ClientTransaction transaction)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = flow == null;
			if (flag)
			{
				throw new ArgumentNullException("flow");
			}
			bool flag2 = request == null;
			if (flag2)
			{
				throw new ArgumentNullException("request");
			}
			bool flag3 = request.Via.GetTopMostValue() == null;
			if (flag3)
			{
				throw new ArgumentException("Argument 'request' doesn't contain required Via: header field.");
			}
			SIP_t_ViaParm topMostValue = request.Via.GetTopMostValue();
			topMostValue.ProtocolTransport = flow.Transport;
			HostEndPoint hostEndPoint = null;
			foreach (IPBindInfo ipbindInfo in this.BindInfo)
			{
				bool flag4 = flow.Transport == "UDP" && ipbindInfo.Protocol == BindInfoProtocol.UDP;
				if (flag4)
				{
					bool flag5 = !string.IsNullOrEmpty(ipbindInfo.HostName);
					if (flag5)
					{
						hostEndPoint = new HostEndPoint(ipbindInfo.HostName, ipbindInfo.Port);
					}
					else
					{
						hostEndPoint = new HostEndPoint(flow.LocalEP.Address.ToString(), ipbindInfo.Port);
					}
					break;
				}
				bool flag6 = flow.Transport == "TLS" && ipbindInfo.Protocol == BindInfoProtocol.TCP && ipbindInfo.SslMode == SslMode.SSL;
				if (flag6)
				{
					bool flag7 = !string.IsNullOrEmpty(ipbindInfo.HostName);
					if (flag7)
					{
						hostEndPoint = new HostEndPoint(ipbindInfo.HostName, ipbindInfo.Port);
					}
					else
					{
						hostEndPoint = new HostEndPoint(flow.LocalEP.Address.ToString(), ipbindInfo.Port);
					}
					break;
				}
				bool flag8 = flow.Transport == "TCP" && ipbindInfo.Protocol == BindInfoProtocol.TCP;
				if (flag8)
				{
					bool flag9 = !string.IsNullOrEmpty(ipbindInfo.HostName);
					if (flag9)
					{
						hostEndPoint = new HostEndPoint(ipbindInfo.HostName, ipbindInfo.Port);
					}
					else
					{
						hostEndPoint = new HostEndPoint(flow.LocalEP.Address.ToString(), ipbindInfo.Port);
					}
					break;
				}
			}
			bool flag10 = hostEndPoint == null;
			if (flag10)
			{
				topMostValue.SentBy = new HostEndPoint(flow.LocalEP);
			}
			else
			{
				topMostValue.SentBy = hostEndPoint;
			}
			flow.Send(request);
			bool flag11 = this.m_pStack.Logger != null;
			if (flag11)
			{
				byte[] array = request.ToByteData();
				this.m_pStack.Logger.AddWrite(Guid.NewGuid().ToString(), null, 0L, string.Concat(new object[]
				{
					"Request [",
					(transaction == null) ? "" : ("transactionID='" + transaction.ID + "';"),
					"method='",
					request.RequestLine.Method,
					"'; cseq='",
					request.CSeq.SequenceNumber,
					"'; transport='",
					flow.Transport,
					"'; size='",
					array.Length,
					"'; sent '",
					flow.LocalEP,
					"' -> '",
					flow.RemoteEP,
					"'."
				}), flow.LocalEP, flow.RemoteEP, array);
			}
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x000250FE File Offset: 0x000240FE
		public void SendResponse(SIP_Response response)
		{
			this.SendResponse(response, null);
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x0002510A File Offset: 0x0002410A
		public void SendResponse(SIP_Response response, IPEndPoint localEP)
		{
			this.SendResponseInternal(null, response, localEP);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00025118 File Offset: 0x00024118
		internal void SendResponse(SIP_ServerTransaction transaction, SIP_Response response)
		{
			bool flag = transaction == null;
			if (flag)
			{
				throw new ArgumentNullException("transaction");
			}
			this.SendResponseInternal(transaction, response, null);
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00025144 File Offset: 0x00024144
		private void SendResponseInternal(SIP_ServerTransaction transaction, SIP_Response response, IPEndPoint localEP)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.m_IsRunning;
			if (flag)
			{
				throw new InvalidOperationException("Stack has not been started.");
			}
			bool flag2 = response == null;
			if (flag2)
			{
				throw new ArgumentNullException("response");
			}
			SIP_t_ViaParm topMostValue = response.Via.GetTopMostValue();
			bool flag3 = topMostValue == null;
			if (flag3)
			{
				throw new ArgumentException("Argument 'response' does not contain required Via: header field.");
			}
			string text = Guid.NewGuid().ToString();
			string text2 = (transaction == null) ? "" : transaction.ID;
			bool flag4 = transaction != null && transaction.Request.LocalEndPoint != null;
			if (flag4)
			{
				localEP = transaction.Request.LocalEndPoint;
			}
			else
			{
				bool flag5 = topMostValue.Parameters["localEP"] != null;
				if (flag5)
				{
					localEP = Net_Utils.ParseIPEndPoint(topMostValue.Parameters["localEP"].Value);
				}
			}
			byte[] array = response.ToByteData();
			bool flag6 = transaction != null;
			if (flag6)
			{
				try
				{
					SIP_Flow flow = transaction.Flow;
					flow.Send(response);
					bool flag7 = this.m_pStack.Logger != null;
					if (flag7)
					{
						this.m_pStack.Logger.AddWrite(text, null, 0L, string.Concat(new object[]
						{
							"Response [flowReuse=true; transactionID='",
							text2,
							"'; method='",
							response.CSeq.RequestMethod,
							"'; cseq='",
							response.CSeq.SequenceNumber,
							"'; transport='",
							flow.Transport,
							"'; size='",
							array.Length,
							"'; statusCode='",
							response.StatusCode,
							"'; reason='",
							response.ReasonPhrase,
							"'; sent '",
							flow.LocalEP,
							"' -> '",
							flow.RemoteEP,
							"'."
						}), localEP, flow.RemoteEP, array);
					}
					return;
				}
				catch
				{
				}
			}
			bool flag8 = SIP_Utils.IsReliableTransport(topMostValue.ProtocolTransport);
			if (flag8)
			{
				IPEndPoint ipendPoint = null;
				bool flag9 = transaction != null && transaction.Request.RemoteEndPoint != null;
				if (flag9)
				{
					ipendPoint = transaction.Request.RemoteEndPoint;
				}
				else
				{
					bool flag10 = topMostValue.Received != null;
					if (flag10)
					{
						ipendPoint = new IPEndPoint(topMostValue.Received, (topMostValue.SentBy.Port == -1) ? 5060 : topMostValue.SentBy.Port);
					}
				}
				try
				{
					SIP_Flow sip_Flow = null;
					bool flag11 = transaction != null;
					if (flag11)
					{
						bool flag12 = transaction.Request.Flow != null && !transaction.Request.Flow.IsDisposed;
						if (flag12)
						{
							sip_Flow = transaction.Request.Flow;
						}
					}
					else
					{
						string value = topMostValue.Parameters["connectionID"].Value;
						bool flag13 = value != null;
						if (flag13)
						{
							sip_Flow = this.m_pFlowManager[value];
						}
					}
					bool flag14 = sip_Flow != null;
					if (flag14)
					{
						sip_Flow.Send(response);
						bool flag15 = this.m_pStack.Logger != null;
						if (flag15)
						{
							this.m_pStack.Logger.AddWrite(text, null, 0L, string.Concat(new object[]
							{
								"Response [flowReuse=true; transactionID='",
								text2,
								"'; method='",
								response.CSeq.RequestMethod,
								"'; cseq='",
								response.CSeq.SequenceNumber,
								"'; transport='",
								sip_Flow.Transport,
								"'; size='",
								array.Length,
								"'; statusCode='",
								response.StatusCode,
								"'; reason='",
								response.ReasonPhrase,
								"'; sent '",
								sip_Flow.RemoteEP,
								"' -> '",
								sip_Flow.LocalEP,
								"'."
							}), localEP, ipendPoint, array);
						}
						return;
					}
				}
				catch
				{
				}
				bool flag16 = ipendPoint != null;
				if (flag16)
				{
					try
					{
						this.SendResponseToHost(text, text2, null, ipendPoint.Address.ToString(), ipendPoint.Port, topMostValue.ProtocolTransport, response);
					}
					catch
					{
					}
				}
				this.SendResponse_RFC_3263_5(text, text2, localEP, response);
			}
			else
			{
				bool flag17 = topMostValue.Maddr != null;
				if (flag17)
				{
					throw new SIP_TransportException("Sending responses to multicast address(Via: 'maddr') is not supported.");
				}
				bool flag18 = topMostValue.Maddr == null && topMostValue.Received != null && topMostValue.RPort > 0;
				if (flag18)
				{
					this.SendResponseToHost(text, text2, localEP, topMostValue.Received.ToString(), topMostValue.RPort, topMostValue.ProtocolTransport, response);
				}
				else
				{
					bool flag19 = topMostValue.Received != null;
					if (flag19)
					{
						this.SendResponseToHost(text, text2, localEP, topMostValue.Received.ToString(), topMostValue.SentByPortWithDefault, topMostValue.ProtocolTransport, response);
					}
					else
					{
						this.SendResponse_RFC_3263_5(text, text2, localEP, response);
					}
				}
			}
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000256E0 File Offset: 0x000246E0
		private void SendResponse_RFC_3263_5(string logID, string transactionID, IPEndPoint localEP, SIP_Response response)
		{
			SIP_t_ViaParm topMostValue = response.Via.GetTopMostValue();
			bool isIPAddress = topMostValue.SentBy.IsIPAddress;
			if (isIPAddress)
			{
				this.SendResponseToHost(logID, transactionID, localEP, topMostValue.SentBy.Host, topMostValue.SentByPortWithDefault, topMostValue.ProtocolTransport, response);
			}
			else
			{
				bool flag = topMostValue.SentBy.Port != -1;
				if (flag)
				{
					this.SendResponseToHost(logID, transactionID, localEP, topMostValue.SentBy.Host, topMostValue.SentByPortWithDefault, topMostValue.ProtocolTransport, response);
				}
				else
				{
					try
					{
						string queryText = "";
						bool flag2 = topMostValue.ProtocolTransport == "UDP";
						if (flag2)
						{
							queryText = "_sip._udp." + topMostValue.SentBy.Host;
						}
						else
						{
							bool flag3 = topMostValue.ProtocolTransport == "TCP";
							if (flag3)
							{
								queryText = "_sip._tcp." + topMostValue.SentBy.Host;
							}
							else
							{
								bool flag4 = topMostValue.ProtocolTransport == "UDP";
								if (flag4)
								{
									queryText = "_sips._tcp." + topMostValue.SentBy.Host;
								}
							}
						}
						DnsServerResponse dnsServerResponse = this.m_pStack.Dns.Query(queryText, DNS_QType.SRV);
						bool flag5 = dnsServerResponse.ResponseCode > DNS_RCode.NO_ERROR;
						if (flag5)
						{
							throw new SIP_TransportException("Dns error: " + dnsServerResponse.ResponseCode.ToString());
						}
						DNS_rr_SRV[] srvrecords = dnsServerResponse.GetSRVRecords();
						bool flag6 = srvrecords.Length != 0;
						if (flag6)
						{
							for (int i = 0; i < srvrecords.Length; i++)
							{
								DNS_rr_SRV dns_rr_SRV = srvrecords[i];
								try
								{
									bool flag7 = this.m_pStack.Logger != null;
									if (flag7)
									{
										this.m_pStack.Logger.AddText(logID, "Starts sending response to DNS SRV record '" + dns_rr_SRV.Target + "'.");
									}
									this.SendResponseToHost(logID, transactionID, localEP, dns_rr_SRV.Target, dns_rr_SRV.Port, topMostValue.ProtocolTransport, response);
								}
								catch
								{
									bool flag8 = i == srvrecords.Length - 1;
									if (flag8)
									{
										bool flag9 = this.m_pStack.Logger != null;
										if (flag9)
										{
											this.m_pStack.Logger.AddText(logID, "Failed to send response to DNS SRV record '" + dns_rr_SRV.Target + "'.");
										}
										throw new SIP_TransportException("Host '" + topMostValue.SentBy.Host + "' is not accessible.");
									}
									bool flag10 = this.m_pStack.Logger != null;
									if (flag10)
									{
										this.m_pStack.Logger.AddText(logID, "Failed to send response to DNS SRV record '" + dns_rr_SRV.Target + "', will try next.");
									}
								}
							}
						}
						else
						{
							bool flag11 = this.m_pStack.Logger != null;
							if (flag11)
							{
								this.m_pStack.Logger.AddText(logID, "No DNS SRV records found, starts sending to Via: sent-by host '" + topMostValue.SentBy.Host + "'.");
							}
							this.SendResponseToHost(logID, transactionID, localEP, topMostValue.SentBy.Host, topMostValue.SentByPortWithDefault, topMostValue.ProtocolTransport, response);
						}
					}
					catch (DNS_ClientException ex)
					{
						throw new SIP_TransportException("Dns error: " + ex.ErrorCode.ToString());
					}
				}
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00025A7C File Offset: 0x00024A7C
		private void SendResponseToHost(string logID, string transactionID, IPEndPoint localEP, string host, int port, string transport, SIP_Response response)
		{
			try
			{
				IPAddress[] array = null;
				bool flag = Net_Utils.IsIPAddress(host);
				if (flag)
				{
					array = new IPAddress[]
					{
						IPAddress.Parse(host)
					};
				}
				else
				{
					array = this.m_pStack.Dns.GetHostAddresses(host);
					bool flag2 = array.Length == 0;
					if (flag2)
					{
						throw new SIP_TransportException("Invalid Via: Sent-By host name '" + host + "' could not be resolved.");
					}
				}
				byte[] array2 = response.ToByteData();
				for (int i = 0; i < array.Length; i++)
				{
					IPEndPoint ipendPoint = new IPEndPoint(array[i], port);
					try
					{
						SIP_Flow orCreateFlow = this.GetOrCreateFlow(transport, localEP, ipendPoint);
						orCreateFlow.Send(response);
						bool flag3 = this.m_pStack.Logger != null;
						if (flag3)
						{
							this.m_pStack.Logger.AddWrite(logID, null, 0L, string.Concat(new object[]
							{
								"Response [transactionID='",
								transactionID,
								"'; method='",
								response.CSeq.RequestMethod,
								"'; cseq='",
								response.CSeq.SequenceNumber,
								"'; transport='",
								transport,
								"'; size='",
								array2.Length,
								"'; statusCode='",
								response.StatusCode,
								"'; reason='",
								response.ReasonPhrase,
								"'; sent '",
								localEP,
								"' -> '",
								ipendPoint,
								"'."
							}), localEP, ipendPoint, array2);
						}
						break;
					}
					catch
					{
						bool flag4 = i == array.Length - 1;
						if (flag4)
						{
							bool flag5 = this.m_pStack.Logger != null;
							if (flag5)
							{
								this.m_pStack.Logger.AddText(logID, string.Concat(new object[]
								{
									"Failed to send response to host '",
									host,
									"' IP end point '",
									ipendPoint,
									"'."
								}));
							}
							throw new SIP_TransportException(string.Concat(new object[]
							{
								"Host '",
								host,
								":",
								port,
								"' is not accessible."
							}));
						}
						bool flag6 = this.m_pStack.Logger != null;
						if (flag6)
						{
							this.m_pStack.Logger.AddText(logID, string.Concat(new object[]
							{
								"Failed to send response to host '",
								host,
								"' IP end point '",
								ipendPoint,
								"', will try next A record."
							}));
						}
					}
				}
			}
			catch (DNS_ClientException ex)
			{
				throw new SIP_TransportException("Dns error: " + ex.ErrorCode.ToString());
			}
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00025D84 File Offset: 0x00024D84
		internal HostEndPoint GetContactHost(SIP_Flow flow)
		{
			bool flag = flow == null;
			if (flag)
			{
				throw new ArgumentNullException("flow");
			}
			HostEndPoint hostEndPoint = null;
			foreach (IPBindInfo ipbindInfo in this.BindInfo)
			{
				bool flag2 = ipbindInfo.Protocol == BindInfoProtocol.UDP && flow.Transport == "UDP";
				if (flag2)
				{
					bool flag3 = ipbindInfo.IP.AddressFamily == flow.LocalEP.AddressFamily && ipbindInfo.Port == flow.LocalEP.Port;
					if (flag3)
					{
						hostEndPoint = new HostEndPoint(string.IsNullOrEmpty(ipbindInfo.HostName) ? flow.LocalEP.Address.ToString() : ipbindInfo.HostName, ipbindInfo.Port);
						break;
					}
				}
				else
				{
					bool flag4 = ipbindInfo.Protocol == BindInfoProtocol.TCP && ipbindInfo.SslMode == SslMode.SSL && flow.Transport == "TLS";
					if (flag4)
					{
						bool flag5 = ipbindInfo.IP.AddressFamily == flow.LocalEP.AddressFamily;
						if (flag5)
						{
							bool flag6 = ipbindInfo.IP == IPAddress.Any || ipbindInfo.IP == IPAddress.IPv6Any;
							if (flag6)
							{
								hostEndPoint = new HostEndPoint(string.IsNullOrEmpty(ipbindInfo.HostName) ? flow.LocalEP.Address.ToString() : ipbindInfo.HostName, ipbindInfo.Port);
							}
							else
							{
								hostEndPoint = new HostEndPoint(string.IsNullOrEmpty(ipbindInfo.HostName) ? ipbindInfo.IP.ToString() : ipbindInfo.HostName, ipbindInfo.Port);
							}
							break;
						}
					}
					else
					{
						bool flag7 = ipbindInfo.Protocol == BindInfoProtocol.TCP && flow.Transport == "TCP";
						if (flag7)
						{
							bool flag8 = ipbindInfo.IP.AddressFamily == flow.LocalEP.AddressFamily;
							if (flag8)
							{
								bool flag9 = ipbindInfo.IP.Equals(IPAddress.Any) || ipbindInfo.IP.Equals(IPAddress.IPv6Any);
								if (flag9)
								{
									hostEndPoint = new HostEndPoint(string.IsNullOrEmpty(ipbindInfo.HostName) ? flow.LocalEP.Address.ToString() : ipbindInfo.HostName, ipbindInfo.Port);
								}
								else
								{
									hostEndPoint = new HostEndPoint(string.IsNullOrEmpty(ipbindInfo.HostName) ? ipbindInfo.IP.ToString() : ipbindInfo.HostName, ipbindInfo.Port);
								}
								break;
							}
						}
					}
				}
			}
			bool flag10 = hostEndPoint == null;
			if (flag10)
			{
				hostEndPoint = new HostEndPoint(flow.LocalEP);
			}
			bool flag11 = hostEndPoint.IsIPAddress && Net_Utils.IsPrivateIP(IPAddress.Parse(hostEndPoint.Host)) && !Net_Utils.IsPrivateIP(flow.RemoteEP.Address);
			if (flag11)
			{
				hostEndPoint = new HostEndPoint(flow.LocalPublicEP);
			}
			return hostEndPoint;
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00026098 File Offset: 0x00025098
		internal string GetRecordRoute(string transport)
		{
			foreach (IPBindInfo ipbindInfo in this.m_pBinds)
			{
				bool flag = !string.IsNullOrEmpty(ipbindInfo.HostName);
				if (flag)
				{
					bool flag2 = ipbindInfo.Protocol == BindInfoProtocol.TCP && ipbindInfo.SslMode != SslMode.None && transport == "TLS";
					string result;
					if (flag2)
					{
						result = string.Concat(new object[]
						{
							"<sips:",
							ipbindInfo.HostName,
							":",
							ipbindInfo.Port,
							";lr>"
						});
					}
					else
					{
						bool flag3 = ipbindInfo.Protocol == BindInfoProtocol.TCP && transport == "TCP";
						if (flag3)
						{
							result = string.Concat(new object[]
							{
								"<sip:",
								ipbindInfo.HostName,
								":",
								ipbindInfo.Port,
								";lr>"
							});
						}
						else
						{
							bool flag4 = ipbindInfo.Protocol == BindInfoProtocol.UDP && transport == "UDP";
							if (!flag4)
							{
								goto IL_149;
							}
							result = string.Concat(new object[]
							{
								"<sip:",
								ipbindInfo.HostName,
								":",
								ipbindInfo.Port,
								";lr>"
							});
						}
					}
					return result;
				}
				IL_149:;
			}
			return null;
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000656 RID: 1622 RVA: 0x00026204 File Offset: 0x00025204
		public bool IsRunning
		{
			get
			{
				return this.m_IsRunning;
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000657 RID: 1623 RVA: 0x0002621C File Offset: 0x0002521C
		public SIP_Stack Stack
		{
			get
			{
				return this.m_pStack;
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x00026234 File Offset: 0x00025234
		// (set) Token: 0x06000659 RID: 1625 RVA: 0x0002624C File Offset: 0x0002524C
		public IPBindInfo[] BindInfo
		{
			get
			{
				return this.m_pBinds;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("BindInfo");
				}
				bool flag2 = false;
				bool flag3 = this.m_pBinds.Length != value.Length;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					for (int i = 0; i < this.m_pBinds.Length; i++)
					{
						bool flag4 = !this.m_pBinds[i].Equals(value[i]);
						if (flag4)
						{
							flag2 = true;
							break;
						}
					}
				}
				bool flag5 = flag2;
				if (flag5)
				{
					this.m_pBinds = value;
					List<IPEndPoint> list = new List<IPEndPoint>();
					List<IPBindInfo> list2 = new List<IPBindInfo>();
					foreach (IPBindInfo ipbindInfo in this.m_pBinds)
					{
						bool flag6 = ipbindInfo.Protocol == BindInfoProtocol.UDP;
						if (flag6)
						{
							list.Add(new IPEndPoint(ipbindInfo.IP, ipbindInfo.Port));
						}
						else
						{
							list2.Add(ipbindInfo);
						}
					}
					this.m_pUdpServer.Bindings = list.ToArray();
					this.m_pTcpServer.Bindings = list2.ToArray();
					foreach (IPEndPoint ipendPoint in this.m_pTcpServer.LocalEndPoints)
					{
						bool flag7 = ipendPoint.AddressFamily == AddressFamily.InterNetwork;
						if (flag7)
						{
							this.m_pLocalIPv4.Add(ipendPoint.Address);
						}
						else
						{
							bool flag8 = ipendPoint.AddressFamily == AddressFamily.InterNetwork;
							if (flag8)
							{
								this.m_pLocalIPv6.Add(ipendPoint.Address);
							}
						}
					}
				}
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x000263E4 File Offset: 0x000253E4
		public SIP_Flow[] Flows
		{
			get
			{
				return this.m_pFlowManager.Flows;
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x0600065B RID: 1627 RVA: 0x00026404 File Offset: 0x00025404
		internal UDP_Server UdpServer
		{
			get
			{
				return this.m_pUdpServer;
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x0600065C RID: 1628 RVA: 0x0002641C File Offset: 0x0002541C
		// (set) Token: 0x0600065D RID: 1629 RVA: 0x00026434 File Offset: 0x00025434
		internal string StunServer
		{
			get
			{
				return this.m_StunServer;
			}
			set
			{
				this.m_StunServer = value;
			}
		}

		// Token: 0x04000298 RID: 664
		private bool m_IsDisposed = false;

		// Token: 0x04000299 RID: 665
		private bool m_IsRunning = false;

		// Token: 0x0400029A RID: 666
		private SIP_Stack m_pStack = null;

		// Token: 0x0400029B RID: 667
		private IPBindInfo[] m_pBinds = null;

		// Token: 0x0400029C RID: 668
		private UDP_Server m_pUdpServer = null;

		// Token: 0x0400029D RID: 669
		private TCP_Server<TCP_ServerSession> m_pTcpServer = null;

		// Token: 0x0400029E RID: 670
		private SIP_TransportLayer.SIP_FlowManager m_pFlowManager = null;

		// Token: 0x0400029F RID: 671
		private string m_StunServer = null;

		// Token: 0x040002A0 RID: 672
		private CircleCollection<IPAddress> m_pLocalIPv4 = null;

		// Token: 0x040002A1 RID: 673
		private CircleCollection<IPAddress> m_pLocalIPv6 = null;

		// Token: 0x040002A2 RID: 674
		private Random m_pRandom = null;

		// Token: 0x0200028F RID: 655
		private class SIP_FlowManager : IDisposable
		{
			// Token: 0x0600170B RID: 5899 RVA: 0x0008E984 File Offset: 0x0008D984
			internal SIP_FlowManager(SIP_TransportLayer owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pOwner = owner;
				this.m_pFlows = new Dictionary<string, SIP_Flow>();
				this.m_pTimeoutTimer = new TimerEx(15000.0);
				this.m_pTimeoutTimer.AutoReset = true;
				this.m_pTimeoutTimer.Elapsed += this.m_pTimeoutTimer_Elapsed;
				this.m_pTimeoutTimer.Enabled = true;
			}

			// Token: 0x0600170C RID: 5900 RVA: 0x0008EA38 File Offset: 0x0008DA38
			public void Dispose()
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool isDisposed = this.m_IsDisposed;
					if (!isDisposed)
					{
						this.m_IsDisposed = true;
						foreach (SIP_Flow sip_Flow in this.Flows)
						{
							sip_Flow.Dispose();
						}
						this.m_pOwner = null;
						this.m_pFlows = null;
						this.m_pTimeoutTimer.Dispose();
						this.m_pTimeoutTimer = null;
					}
				}
			}

			// Token: 0x0600170D RID: 5901 RVA: 0x0008EAD8 File Offset: 0x0008DAD8
			private void m_pTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool isDisposed = this.m_IsDisposed;
					if (!isDisposed)
					{
						foreach (SIP_Flow sip_Flow in this.Flows)
						{
							try
							{
								bool flag2 = sip_Flow.LastActivity.AddSeconds((double)this.m_IdelTimeout) < DateTime.Now;
								if (flag2)
								{
									sip_Flow.Dispose();
								}
							}
							catch (ObjectDisposedException ex)
							{
								string message = ex.Message;
							}
						}
					}
				}
			}

			// Token: 0x0600170E RID: 5902 RVA: 0x0008EB98 File Offset: 0x0008DB98
			internal SIP_Flow GetOrCreateFlow(bool isServer, IPEndPoint localEP, IPEndPoint remoteEP, string transport)
			{
				bool flag = localEP == null;
				if (flag)
				{
					throw new ArgumentNullException("localEP");
				}
				bool flag2 = remoteEP == null;
				if (flag2)
				{
					throw new ArgumentNullException("remoteEP");
				}
				bool flag3 = transport == null;
				if (flag3)
				{
					throw new ArgumentNullException("transport");
				}
				string flowID = string.Concat(new string[]
				{
					localEP.ToString(),
					"-",
					remoteEP.ToString(),
					"-",
					transport.ToString()
				});
				object pLock = this.m_pLock;
				SIP_Flow result;
				lock (pLock)
				{
					SIP_Flow sip_Flow = null;
					bool flag5 = this.m_pFlows.TryGetValue(flowID, out sip_Flow);
					if (flag5)
					{
						result = sip_Flow;
					}
					else
					{
						sip_Flow = new SIP_Flow(this.m_pOwner.Stack, isServer, localEP, remoteEP, transport);
						this.m_pFlows.Add(sip_Flow.ID, sip_Flow);
						sip_Flow.IsDisposing += delegate(object s, EventArgs e)
						{
							object pLock2 = this.m_pLock;
							lock (pLock2)
							{
								this.m_pFlows.Remove(flowID);
							}
						};
						sip_Flow.Start();
						result = sip_Flow;
					}
				}
				return result;
			}

			// Token: 0x0600170F RID: 5903 RVA: 0x0008ECD4 File Offset: 0x0008DCD4
			public SIP_Flow GetFlow(string flowID)
			{
				bool flag = flowID == null;
				if (flag)
				{
					throw new ArgumentNullException("flowID");
				}
				Dictionary<string, SIP_Flow> pFlows = this.m_pFlows;
				SIP_Flow result;
				lock (pFlows)
				{
					SIP_Flow sip_Flow = null;
					this.m_pFlows.TryGetValue(flowID, out sip_Flow);
					result = sip_Flow;
				}
				return result;
			}

			// Token: 0x06001710 RID: 5904 RVA: 0x0008ED3C File Offset: 0x0008DD3C
			internal SIP_Flow CreateFromSession(TCP_ServerSession session)
			{
				bool flag = session == null;
				if (flag)
				{
					throw new ArgumentNullException("session");
				}
				string flowID = string.Concat(new string[]
				{
					session.LocalEndPoint.ToString(),
					"-",
					session.RemoteEndPoint.ToString(),
					"-",
					session.IsSecureConnection ? "TLS" : "TCP"
				});
				object pLock = this.m_pLock;
				SIP_Flow result;
				lock (pLock)
				{
					SIP_Flow sip_Flow = new SIP_Flow(this.m_pOwner.Stack, session);
					this.m_pFlows.Add(flowID, sip_Flow);
					sip_Flow.IsDisposing += delegate(object s, EventArgs e)
					{
						object pLock2 = this.m_pLock;
						lock (pLock2)
						{
							this.m_pFlows.Remove(flowID);
						}
					};
					sip_Flow.Start();
					result = sip_Flow;
				}
				return result;
			}

			// Token: 0x17000781 RID: 1921
			// (get) Token: 0x06001711 RID: 5905 RVA: 0x0008EE38 File Offset: 0x0008DE38
			public bool IsDisposed
			{
				get
				{
					return this.m_IsDisposed;
				}
			}

			// Token: 0x17000782 RID: 1922
			// (get) Token: 0x06001712 RID: 5906 RVA: 0x0008EE50 File Offset: 0x0008DE50
			public int Count
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_pFlows.Count;
				}
			}

			// Token: 0x17000783 RID: 1923
			public SIP_Flow this[string flowID]
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag = flowID == null;
					if (flag)
					{
						throw new ArgumentNullException("flowID");
					}
					bool flag2 = this.m_pFlows.ContainsKey(flowID);
					SIP_Flow result;
					if (flag2)
					{
						result = this.m_pFlows[flowID];
					}
					else
					{
						result = null;
					}
					return result;
				}
			}

			// Token: 0x17000784 RID: 1924
			// (get) Token: 0x06001714 RID: 5908 RVA: 0x0008EEF0 File Offset: 0x0008DEF0
			public SIP_Flow[] Flows
			{
				get
				{
					object pLock = this.m_pLock;
					SIP_Flow[] result;
					lock (pLock)
					{
						SIP_Flow[] array = new SIP_Flow[this.m_pFlows.Count];
						this.m_pFlows.Values.CopyTo(array, 0);
						result = array;
					}
					return result;
				}
			}

			// Token: 0x17000785 RID: 1925
			// (get) Token: 0x06001715 RID: 5909 RVA: 0x0008EF58 File Offset: 0x0008DF58
			internal SIP_TransportLayer TransportLayer
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_pOwner;
				}
			}

			// Token: 0x0400099C RID: 2460
			private bool m_IsDisposed = false;

			// Token: 0x0400099D RID: 2461
			private SIP_TransportLayer m_pOwner = null;

			// Token: 0x0400099E RID: 2462
			private Dictionary<string, SIP_Flow> m_pFlows = null;

			// Token: 0x0400099F RID: 2463
			private TimerEx m_pTimeoutTimer = null;

			// Token: 0x040009A0 RID: 2464
			private int m_IdelTimeout = 300;

			// Token: 0x040009A1 RID: 2465
			private object m_pLock = new object();
		}
	}
}
