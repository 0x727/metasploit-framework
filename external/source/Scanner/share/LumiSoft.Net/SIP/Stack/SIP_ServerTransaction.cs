using System;
using System.Diagnostics;
using System.Timers;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200008E RID: 142
	public class SIP_ServerTransaction : SIP_Transaction
	{
		// Token: 0x06000543 RID: 1347 RVA: 0x0001CBF0 File Offset: 0x0001BBF0
		public SIP_ServerTransaction(SIP_Stack stack, SIP_Flow flow, SIP_Request request) : base(stack, flow, request)
		{
			bool flag = base.Stack.Logger != null;
			if (flag)
			{
				base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
				{
					"Transaction [branch='",
					base.ID,
					"';method='",
					base.Method,
					"';IsServer=true] created."
				}));
			}
			this.Start();
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0001CCA8 File Offset: 0x0001BCA8
		public override void Dispose()
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = this.m_pTimer100 != null;
				if (flag2)
				{
					this.m_pTimer100.Dispose();
					this.m_pTimer100 = null;
				}
				bool flag3 = this.m_pTimerG != null;
				if (flag3)
				{
					this.m_pTimerG.Dispose();
					this.m_pTimerG = null;
				}
				bool flag4 = this.m_pTimerH != null;
				if (flag4)
				{
					this.m_pTimerH.Dispose();
					this.m_pTimerH = null;
				}
				bool flag5 = this.m_pTimerI != null;
				if (flag5)
				{
					this.m_pTimerI.Dispose();
					this.m_pTimerI = null;
				}
				bool flag6 = this.m_pTimerJ != null;
				if (flag6)
				{
					this.m_pTimerJ.Dispose();
					this.m_pTimerJ = null;
				}
				bool flag7 = this.m_pTimerL != null;
				if (flag7)
				{
					this.m_pTimerL.Dispose();
					this.m_pTimerL = null;
				}
			}
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001CDBC File Offset: 0x0001BDBC
		private void m_pTimer100_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Proceeding && base.Responses.Length == 0;
				if (flag2)
				{
					SIP_Response sip_Response = base.Stack.CreateResponse(SIP_ResponseCodes.x100_Trying, base.Request);
					bool flag3 = base.Request.Timestamp != null;
					if (flag3)
					{
						sip_Response.Timestamp = new SIP_t_Timestamp(base.Request.Timestamp.Time, (DateTime.Now - base.CreateTime).Seconds);
					}
					try
					{
						base.Stack.TransportLayer.SendResponse(this, sip_Response);
					}
					catch (Exception exception)
					{
						base.OnTransportError(exception);
						base.SetState(SIP_TransactionState.Terminated);
						return;
					}
				}
				bool flag4 = this.m_pTimer100 != null;
				if (flag4)
				{
					this.m_pTimer100.Dispose();
					this.m_pTimer100 = null;
				}
			}
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001CEE0 File Offset: 0x0001BEE0
		private void m_pTimerG_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Completed;
				if (flag2)
				{
					bool flag3 = base.Stack.Logger != null;
					if (flag3)
					{
						base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
						{
							"Transaction [branch='",
							base.ID,
							"';method='",
							base.Method,
							"';IsServer=true] timer G(INVITE response(3xx - 6xx) retransmission) triggered."
						}));
					}
					try
					{
						base.Stack.TransportLayer.SendResponse(this, base.FinalResponse);
						this.m_pTimerG.Interval *= Math.Min(this.m_pTimerG.Interval * 2.0, 4000.0);
						this.m_pTimerG.Enabled = true;
						bool flag4 = base.Stack.Logger != null;
						if (flag4)
						{
							base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
							{
								"Transaction [branch='",
								base.ID,
								"';method='",
								base.Method,
								"';IsServer=false] timer G(INVITE response(3xx - 6xx) retransmission) updated, will trigger after ",
								this.m_pTimerG.Interval,
								"."
							}));
						}
					}
					catch (Exception exception)
					{
						base.OnTransportError(exception);
						base.SetState(SIP_TransactionState.Terminated);
					}
				}
			}
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001D0AC File Offset: 0x0001C0AC
		private void m_pTimerH_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Completed;
				if (flag2)
				{
					bool flag3 = base.Stack.Logger != null;
					if (flag3)
					{
						base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
						{
							"Transaction [branch='",
							base.ID,
							"';method='",
							base.Method,
							"';IsServer=true] timer H(INVITE ACK wait) triggered."
						}));
					}
					base.OnTransactionError("ACK was never received.");
					base.SetState(SIP_TransactionState.Terminated);
				}
			}
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001D170 File Offset: 0x0001C170
		private void m_pTimerI_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.Stack.Logger != null;
				if (flag2)
				{
					base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
					{
						"Transaction [branch='",
						base.ID,
						"';method='",
						base.Method,
						"';IsServer=true] timer I(INVITE ACK retransmission wait) triggered."
					}));
				}
				base.SetState(SIP_TransactionState.Terminated);
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001D218 File Offset: 0x0001C218
		private void m_pTimerJ_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.Stack.Logger != null;
				if (flag2)
				{
					base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
					{
						"Transaction [branch='",
						base.ID,
						"';method='",
						base.Method,
						"';IsServer=true] timer I(Non-INVITE request retransmission wait) triggered."
					}));
				}
				base.SetState(SIP_TransactionState.Terminated);
			}
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001D2C0 File Offset: 0x0001C2C0
		private void m_pTimerL_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.Stack.Logger != null;
				if (flag2)
				{
					base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
					{
						"Transaction [branch='",
						base.ID,
						"';method='",
						base.Method,
						"';IsServer=true] timer L(ACK wait) triggered."
					}));
				}
				base.SetState(SIP_TransactionState.Terminated);
			}
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0001D368 File Offset: 0x0001C368
		private void Start()
		{
			bool flag = base.Method == "INVITE";
			if (flag)
			{
				base.SetState(SIP_TransactionState.Proceeding);
				this.m_pTimer100 = new TimerEx(200.0, false);
				this.m_pTimer100.Elapsed += this.m_pTimer100_Elapsed;
				this.m_pTimer100.Enabled = true;
			}
			else
			{
				base.SetState(SIP_TransactionState.Trying);
			}
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001D3DC File Offset: 0x0001C3DC
		public void SendResponse(SIP_Response response)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = response == null;
				if (flag3)
				{
					throw new ArgumentNullException("response");
				}
				try
				{
					bool flag4 = base.Method == "INVITE";
					if (flag4)
					{
						bool flag5 = base.State == SIP_TransactionState.Proceeding;
						if (flag5)
						{
							base.AddResponse(response);
							bool flag6 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
							if (flag6)
							{
								base.Stack.TransportLayer.SendResponse(this, response);
								this.OnResponseSent(response);
							}
							else
							{
								bool flag7 = response.StatusCodeType == SIP_StatusCodeType.Success;
								if (flag7)
								{
									base.Stack.TransportLayer.SendResponse(this, response);
									this.OnResponseSent(response);
									base.SetState(SIP_TransactionState.Accpeted);
									this.m_pTimerL = new TimerEx(32000.0);
									this.m_pTimerL.Elapsed += this.m_pTimerL_Elapsed;
									this.m_pTimerL.Enabled = true;
									bool flag8 = base.Stack.Logger != null;
									if (flag8)
									{
										base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
										{
											"Transaction [branch='",
											base.ID,
											"';method='",
											base.Method,
											"';IsServer=true] timer L(ACK wait) started, will trigger after ",
											this.m_pTimerL.Interval,
											"."
										}));
									}
								}
								else
								{
									base.Stack.TransportLayer.SendResponse(this, response);
									this.OnResponseSent(response);
									base.SetState(SIP_TransactionState.Completed);
									bool flag9 = !base.Flow.IsReliable;
									if (flag9)
									{
										this.m_pTimerG = new TimerEx(500.0, false);
										this.m_pTimerG.Elapsed += this.m_pTimerG_Elapsed;
										this.m_pTimerG.Enabled = true;
										bool flag10 = base.Stack.Logger != null;
										if (flag10)
										{
											base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
											{
												"Transaction [branch='",
												base.ID,
												"';method='",
												base.Method,
												"';IsServer=true] timer G(INVITE response(3xx - 6xx) retransmission) started, will trigger after ",
												this.m_pTimerG.Interval,
												"."
											}));
										}
									}
									this.m_pTimerH = new TimerEx(32000.0);
									this.m_pTimerH.Elapsed += this.m_pTimerH_Elapsed;
									this.m_pTimerH.Enabled = true;
									bool flag11 = base.Stack.Logger != null;
									if (flag11)
									{
										base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
										{
											"Transaction [branch='",
											base.ID,
											"';method='",
											base.Method,
											"';IsServer=true] timer H(INVITE ACK wait) started, will trigger after ",
											this.m_pTimerH.Interval,
											"."
										}));
									}
								}
							}
						}
						else
						{
							bool flag12 = base.State == SIP_TransactionState.Accpeted;
							if (flag12)
							{
								base.Stack.TransportLayer.SendResponse(this, response);
								this.OnResponseSent(response);
							}
							else
							{
								bool flag13 = base.State == SIP_TransactionState.Completed;
								if (!flag13)
								{
									bool flag14 = base.State == SIP_TransactionState.Confirmed;
									if (!flag14)
									{
										bool flag15 = base.State == SIP_TransactionState.Terminated;
										if (flag15)
										{
										}
									}
								}
							}
						}
					}
					else
					{
						bool flag16 = base.State == SIP_TransactionState.Trying;
						if (flag16)
						{
							base.AddResponse(response);
							bool flag17 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
							if (flag17)
							{
								base.Stack.TransportLayer.SendResponse(this, response);
								this.OnResponseSent(response);
								base.SetState(SIP_TransactionState.Proceeding);
							}
							else
							{
								base.Stack.TransportLayer.SendResponse(this, response);
								this.OnResponseSent(response);
								base.SetState(SIP_TransactionState.Completed);
								this.m_pTimerJ = new TimerEx(32000.0, false);
								this.m_pTimerJ.Elapsed += this.m_pTimerJ_Elapsed;
								this.m_pTimerJ.Enabled = true;
								bool flag18 = base.Stack.Logger != null;
								if (flag18)
								{
									base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
									{
										"Transaction [branch='",
										base.ID,
										"';method='",
										base.Method,
										"';IsServer=true] timer J(Non-INVITE request retransmission wait) started, will trigger after ",
										this.m_pTimerJ.Interval,
										"."
									}));
								}
							}
						}
						else
						{
							bool flag19 = base.State == SIP_TransactionState.Proceeding;
							if (flag19)
							{
								base.AddResponse(response);
								bool flag20 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
								if (flag20)
								{
									base.Stack.TransportLayer.SendResponse(this, response);
									this.OnResponseSent(response);
								}
								else
								{
									base.Stack.TransportLayer.SendResponse(this, response);
									this.OnResponseSent(response);
									base.SetState(SIP_TransactionState.Completed);
									this.m_pTimerJ = new TimerEx(32000.0, false);
									this.m_pTimerJ.Elapsed += this.m_pTimerJ_Elapsed;
									this.m_pTimerJ.Enabled = true;
									bool flag21 = base.Stack.Logger != null;
									if (flag21)
									{
										base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
										{
											"Transaction [branch='",
											base.ID,
											"';method='",
											base.Method,
											"';IsServer=true] timer J(Non-INVITE request retransmission wait) started, will trigger after ",
											this.m_pTimerJ.Interval,
											"."
										}));
									}
								}
							}
							else
							{
								bool flag22 = base.State == SIP_TransactionState.Completed;
								if (!flag22)
								{
									bool flag23 = base.State == SIP_TransactionState.Terminated;
									if (flag23)
									{
									}
								}
							}
						}
					}
				}
				catch (SIP_TransportException ex)
				{
					bool flag24 = base.Stack.Logger != null;
					if (flag24)
					{
						base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
						{
							"Transaction [branch='",
							base.ID,
							"';method='",
							base.Method,
							"';IsServer=true] transport exception: ",
							ex.Message
						}));
					}
					base.OnTransportError(ex);
				}
			}
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001DAEC File Offset: 0x0001CAEC
		public override void Cancel()
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = base.FinalResponse != null;
				if (flag3)
				{
					throw new InvalidOperationException("Final response is already sent, CANCEL not allowed.");
				}
				try
				{
					SIP_Response response = base.Stack.CreateResponse(SIP_ResponseCodes.x487_Request_Terminated, base.Request);
					base.Stack.TransportLayer.SendResponse(this, response);
					this.OnCanceled();
				}
				catch (SIP_TransportException ex)
				{
					bool flag4 = base.Stack.Logger != null;
					if (flag4)
					{
						base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
						{
							"Transaction [branch='",
							base.ID,
							"';method='",
							base.Method,
							"';IsServer=true] transport exception: ",
							ex.Message
						}));
					}
					base.OnTransportError(ex);
					base.SetState(SIP_TransactionState.Terminated);
				}
			}
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001DC48 File Offset: 0x0001CC48
		internal void ProcessRequest(SIP_Flow flow, SIP_Request request)
		{
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
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag4 = base.State == SIP_TransactionState.Disposed;
				if (!flag4)
				{
					try
					{
						bool flag5 = base.Stack.Logger != null;
						if (flag5)
						{
							byte[] array = request.ToByteData();
							base.Stack.Logger.AddRead(Guid.NewGuid().ToString(), null, 0L, string.Concat(new object[]
							{
								"Request [transactionID='",
								base.ID,
								"'; method='",
								request.RequestLine.Method,
								"'; cseq='",
								request.CSeq.SequenceNumber,
								"'; transport='",
								flow.Transport,
								"'; size='",
								array.Length,
								"'; received '",
								flow.LocalEP,
								"' <- '",
								flow.RemoteEP,
								"'."
							}), flow.LocalEP, flow.RemoteEP, array);
						}
						bool flag6 = base.Method == "INVITE";
						if (flag6)
						{
							bool flag7 = request.RequestLine.Method == "INVITE";
							if (flag7)
							{
								bool flag8 = base.State == SIP_TransactionState.Proceeding;
								if (flag8)
								{
									SIP_Response lastProvisionalResponse = base.LastProvisionalResponse;
									bool flag9 = lastProvisionalResponse != null;
									if (flag9)
									{
										base.Stack.TransportLayer.SendResponse(this, lastProvisionalResponse);
									}
								}
								else
								{
									bool flag10 = base.State == SIP_TransactionState.Completed;
									if (flag10)
									{
										base.Stack.TransportLayer.SendResponse(this, base.FinalResponse);
									}
								}
							}
							else
							{
								bool flag11 = request.RequestLine.Method == "ACK";
								if (flag11)
								{
									bool flag12 = base.State == SIP_TransactionState.Accpeted;
									if (!flag12)
									{
										bool flag13 = base.State == SIP_TransactionState.Completed;
										if (flag13)
										{
											base.SetState(SIP_TransactionState.Confirmed);
											bool flag14 = this.m_pTimerG != null;
											if (flag14)
											{
												this.m_pTimerG.Dispose();
												this.m_pTimerG = null;
												bool flag15 = base.Stack.Logger != null;
												if (flag15)
												{
													base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
													{
														"Transaction [branch='",
														base.ID,
														"';method='",
														base.Method,
														"';IsServer=true] timer G(INVITE response(3xx - 6xx) retransmission) stopped."
													}));
												}
											}
											bool flag16 = this.m_pTimerH != null;
											if (flag16)
											{
												this.m_pTimerH.Dispose();
												this.m_pTimerH = null;
												bool flag17 = base.Stack.Logger != null;
												if (flag17)
												{
													base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
													{
														"Transaction [branch='",
														base.ID,
														"';method='",
														base.Method,
														"';IsServer=true] timer H(INVITE ACK wait) stopped."
													}));
												}
											}
											this.m_pTimerI = new TimerEx((double)(flow.IsReliable ? 0 : 5000), false);
											this.m_pTimerI.Elapsed += this.m_pTimerI_Elapsed;
											bool flag18 = base.Stack.Logger != null;
											if (flag18)
											{
												base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
												{
													"Transaction [branch='",
													base.ID,
													"';method='",
													base.Method,
													"';IsServer=true] timer I(INVITE ACK retransission wait) started, will trigger after ",
													this.m_pTimerI.Interval,
													"."
												}));
											}
											this.m_pTimerI.Enabled = true;
										}
									}
								}
							}
						}
						else
						{
							bool flag19 = base.Method == request.RequestLine.Method;
							if (flag19)
							{
								bool flag20 = base.State == SIP_TransactionState.Proceeding;
								if (flag20)
								{
									base.Stack.TransportLayer.SendResponse(this, base.LastProvisionalResponse);
								}
								else
								{
									bool flag21 = base.State == SIP_TransactionState.Completed;
									if (flag21)
									{
										base.Stack.TransportLayer.SendResponse(this, base.FinalResponse);
									}
								}
							}
						}
					}
					catch (SIP_TransportException ex)
					{
						bool flag22 = base.Stack.Logger != null;
						if (flag22)
						{
							base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
							{
								"Transaction [branch='",
								base.ID,
								"';method='",
								base.Method,
								"';IsServer=true] transport exception: ",
								ex.Message
							}));
						}
						base.OnTransportError(ex);
					}
				}
			}
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x0600054F RID: 1359 RVA: 0x0001E198 File Offset: 0x0001D198
		// (remove) Token: 0x06000550 RID: 1360 RVA: 0x0001E1D0 File Offset: 0x0001D1D0
		
		public event EventHandler<SIP_ResponseSentEventArgs> ResponseSent = null;

		// Token: 0x06000551 RID: 1361 RVA: 0x0001E208 File Offset: 0x0001D208
		private void OnResponseSent(SIP_Response response)
		{
			bool flag = this.ResponseSent != null;
			if (flag)
			{
				this.ResponseSent(this, new SIP_ResponseSentEventArgs(this, response));
			}
		}

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x06000552 RID: 1362 RVA: 0x0001E23C File Offset: 0x0001D23C
		// (remove) Token: 0x06000553 RID: 1363 RVA: 0x0001E274 File Offset: 0x0001D274
		
		public event EventHandler Canceled = null;

		// Token: 0x06000554 RID: 1364 RVA: 0x0001E2AC File Offset: 0x0001D2AC
		private void OnCanceled()
		{
			bool flag = this.Canceled != null;
			if (flag)
			{
				this.Canceled(this, new EventArgs());
			}
		}

		// Token: 0x040001C0 RID: 448
		private TimerEx m_pTimer100 = null;

		// Token: 0x040001C1 RID: 449
		private TimerEx m_pTimerG = null;

		// Token: 0x040001C2 RID: 450
		private TimerEx m_pTimerH = null;

		// Token: 0x040001C3 RID: 451
		private TimerEx m_pTimerI = null;

		// Token: 0x040001C4 RID: 452
		private TimerEx m_pTimerJ = null;

		// Token: 0x040001C5 RID: 453
		private TimerEx m_pTimerL = null;
	}
}
