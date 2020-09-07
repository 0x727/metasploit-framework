using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200008B RID: 139
	public class SIP_ClientTransaction : SIP_Transaction
	{
		// Token: 0x06000524 RID: 1316 RVA: 0x0001AAEC File Offset: 0x00019AEC
		internal SIP_ClientTransaction(SIP_Stack stack, SIP_Flow flow, SIP_Request request) : base(stack, flow, request)
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
					"';IsServer=false] created."
				}));
			}
			base.SetState(SIP_TransactionState.WaitingToStart);
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001ABB4 File Offset: 0x00019BB4
		public override void Dispose()
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool isDisposed = base.IsDisposed;
				if (!isDisposed)
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
							"';IsServer=false] disposed."
						}));
					}
					bool flag3 = this.m_pTimerA != null;
					if (flag3)
					{
						this.m_pTimerA.Dispose();
						this.m_pTimerA = null;
					}
					bool flag4 = this.m_pTimerB != null;
					if (flag4)
					{
						this.m_pTimerB.Dispose();
						this.m_pTimerB = null;
					}
					bool flag5 = this.m_pTimerD != null;
					if (flag5)
					{
						this.m_pTimerD.Dispose();
						this.m_pTimerD = null;
					}
					bool flag6 = this.m_pTimerE != null;
					if (flag6)
					{
						this.m_pTimerE.Dispose();
						this.m_pTimerE = null;
					}
					bool flag7 = this.m_pTimerF != null;
					if (flag7)
					{
						this.m_pTimerF.Dispose();
						this.m_pTimerF = null;
					}
					bool flag8 = this.m_pTimerK != null;
					if (flag8)
					{
						this.m_pTimerK.Dispose();
						this.m_pTimerK = null;
					}
					bool flag9 = this.m_pTimerM != null;
					if (flag9)
					{
						this.m_pTimerM.Dispose();
						this.m_pTimerM = null;
					}
					this.ResponseReceived = null;
					base.Dispose();
				}
			}
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001AD7C File Offset: 0x00019D7C
		private void m_pTimerA_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Calling;
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
							"';IsServer=false] timer A(INVITE request retransmission) triggered."
						}));
					}
					try
					{
						base.Stack.TransportLayer.SendRequest(base.Flow, base.Request, this);
					}
					catch (Exception exception)
					{
						base.OnTransportError(exception);
						base.SetState(SIP_TransactionState.Terminated);
						return;
					}
					this.m_pTimerA.Interval *= 2.0;
					this.m_pTimerA.Enabled = true;
					bool flag4 = base.Stack.Logger != null;
					if (flag4)
					{
						base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
						{
							"Transaction [branch='",
							base.ID,
							"';method='",
							base.Method,
							"';IsServer=false] timer A(INVITE request retransmission) updated, will trigger after ",
							this.m_pTimerA.Interval,
							"."
						}));
					}
				}
			}
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0001AF34 File Offset: 0x00019F34
		private void m_pTimerB_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Calling;
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
							"';IsServer=false] timer B(INVITE calling state timeout) triggered."
						}));
					}
					base.OnTimedOut();
					base.SetState(SIP_TransactionState.Terminated);
					bool flag4 = this.m_pTimerA != null;
					if (flag4)
					{
						this.m_pTimerA.Dispose();
						this.m_pTimerA = null;
					}
					bool flag5 = this.m_pTimerB != null;
					if (flag5)
					{
						this.m_pTimerB.Dispose();
						this.m_pTimerB = null;
					}
				}
			}
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x0001B03C File Offset: 0x0001A03C
		private void m_pTimerD_Elapsed(object sender, ElapsedEventArgs e)
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
							"';IsServer=false] timer D(INVITE 3xx - 6xx response retransmission wait) triggered."
						}));
					}
					base.SetState(SIP_TransactionState.Terminated);
				}
			}
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0001B0F4 File Offset: 0x0001A0F4
		private void m_pTimerE_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Trying;
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
							"';IsServer=false] timer E(-NonINVITE request retransmission) triggered."
						}));
					}
					try
					{
						base.Stack.TransportLayer.SendRequest(base.Flow, base.Request, this);
					}
					catch (Exception exception)
					{
						base.OnTransportError(exception);
						base.SetState(SIP_TransactionState.Terminated);
						return;
					}
					this.m_pTimerE.Interval = Math.Min(this.m_pTimerE.Interval * 2.0, 4000.0);
					this.m_pTimerE.Enabled = true;
					bool flag4 = base.Stack.Logger != null;
					if (flag4)
					{
						base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
						{
							"Transaction [branch='",
							base.ID,
							"';method='",
							base.Method,
							"';IsServer=false] timer E(Non-INVITE request retransmission) updated, will trigger after ",
							this.m_pTimerE.Interval,
							"."
						}));
					}
				}
			}
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0001B2C0 File Offset: 0x0001A2C0
		private void m_pTimerF_Elapsed(object sender, ElapsedEventArgs e)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Trying || base.State == SIP_TransactionState.Proceeding;
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
							"';IsServer=false] timer F(Non-INVITE trying,proceeding state timeout) triggered."
						}));
					}
					base.OnTimedOut();
					bool flag4 = base.State == SIP_TransactionState.Disposed;
					if (!flag4)
					{
						base.SetState(SIP_TransactionState.Terminated);
						bool flag5 = this.m_pTimerE != null;
						if (flag5)
						{
							this.m_pTimerE.Dispose();
							this.m_pTimerE = null;
						}
						bool flag6 = this.m_pTimerF != null;
						if (flag6)
						{
							this.m_pTimerF.Dispose();
							this.m_pTimerF = null;
						}
					}
				}
			}
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0001B3EC File Offset: 0x0001A3EC
		private void m_pTimerK_Elapsed(object sender, ElapsedEventArgs e)
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
							"';IsServer=false] timer K(Non-INVITE 3xx - 6xx response retransmission wait) triggered."
						}));
					}
					base.SetState(SIP_TransactionState.Terminated);
				}
			}
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x0001B4A4 File Offset: 0x0001A4A4
		private void m_pTimerM_Elapsed(object sender, ElapsedEventArgs e)
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
						"';IsServer=false] timer M(2xx response retransmission wait) triggered."
					}));
				}
				base.SetState(SIP_TransactionState.Terminated);
			}
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x0001B54C File Offset: 0x0001A54C
		public void Start()
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_TransactionState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = base.State > SIP_TransactionState.WaitingToStart;
				if (flag3)
				{
					throw new InvalidOperationException("Start method is valid only in 'WaitingToStart' state.");
				}
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					object syncRoot2 = base.SyncRoot;
					lock (syncRoot2)
					{
						bool flag5 = base.Method == "INVITE";
						if (flag5)
						{
							base.SetState(SIP_TransactionState.Calling);
							try
							{
								base.Stack.TransportLayer.SendRequest(base.Flow, base.Request, this);
							}
							catch (Exception exception)
							{
								base.OnTransportError(exception);
								bool flag6 = base.State != SIP_TransactionState.Disposed;
								if (flag6)
								{
									base.SetState(SIP_TransactionState.Terminated);
								}
								return;
							}
							bool flag7 = !base.Flow.IsReliable;
							if (flag7)
							{
								this.m_pTimerA = new TimerEx(500.0, false);
								this.m_pTimerA.Elapsed += this.m_pTimerA_Elapsed;
								this.m_pTimerA.Enabled = true;
								bool flag8 = base.Stack.Logger != null;
								if (flag8)
								{
									base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
									{
										"Transaction [branch='",
										base.ID,
										"';method='",
										base.Method,
										"';IsServer=false] timer A(INVITE request retransmission) started, will trigger after ",
										this.m_pTimerA.Interval,
										"."
									}));
								}
							}
							this.m_pTimerB = new TimerEx(32000.0, false);
							this.m_pTimerB.Elapsed += this.m_pTimerB_Elapsed;
							this.m_pTimerB.Enabled = true;
							bool flag9 = base.Stack.Logger != null;
							if (flag9)
							{
								base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
								{
									"Transaction [branch='",
									base.ID,
									"';method='",
									base.Method,
									"';IsServer=false] timer B(INVITE calling state timeout) started, will trigger after ",
									this.m_pTimerB.Interval,
									"."
								}));
							}
						}
						else
						{
							base.SetState(SIP_TransactionState.Trying);
							this.m_pTimerF = new TimerEx(32000.0, false);
							this.m_pTimerF.Elapsed += this.m_pTimerF_Elapsed;
							this.m_pTimerF.Enabled = true;
							bool flag10 = base.Stack.Logger != null;
							if (flag10)
							{
								base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
								{
									"Transaction [branch='",
									base.ID,
									"';method='",
									base.Method,
									"';IsServer=false] timer F(Non-INVITE trying,proceeding state timeout) started, will trigger after ",
									this.m_pTimerF.Interval,
									"."
								}));
							}
							try
							{
								base.Stack.TransportLayer.SendRequest(base.Flow, base.Request, this);
							}
							catch (Exception exception2)
							{
								base.OnTransportError(exception2);
								bool flag11 = base.State != SIP_TransactionState.Disposed;
								if (flag11)
								{
									base.SetState(SIP_TransactionState.Terminated);
								}
								return;
							}
							bool flag12 = !base.Flow.IsReliable;
							if (flag12)
							{
								this.m_pTimerE = new TimerEx(500.0, false);
								this.m_pTimerE.Elapsed += this.m_pTimerE_Elapsed;
								this.m_pTimerE.Enabled = true;
								bool flag13 = base.Stack.Logger != null;
								if (flag13)
								{
									base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
									{
										"Transaction [branch='",
										base.ID,
										"';method='",
										base.Method,
										"';IsServer=false] timer E(Non-INVITE request retransmission) started, will trigger after ",
										this.m_pTimerE.Interval,
										"."
									}));
								}
							}
						}
					}
				});
			}
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x0001B5D8 File Offset: 0x0001A5D8
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
				bool flag3 = base.State == SIP_TransactionState.WaitingToStart;
				if (flag3)
				{
					base.SetState(SIP_TransactionState.Terminated);
				}
				else
				{
					bool isCanceling = this.m_IsCanceling;
					if (!isCanceling)
					{
						bool flag4 = base.State == SIP_TransactionState.Terminated;
						if (!flag4)
						{
							bool flag5 = base.FinalResponse != null;
							if (!flag5)
							{
								this.m_IsCanceling = true;
								bool flag6 = base.Responses.Length == 0;
								if (!flag6)
								{
									this.SendCancel();
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0001B6A8 File Offset: 0x0001A6A8
		internal void ProcessResponse(SIP_Flow flow, SIP_Response response)
		{
			bool flag = flow == null;
			if (flag)
			{
				throw new ArgumentNullException("flow");
			}
			bool flag2 = response == null;
			if (flag2)
			{
				throw new ArgumentNullException("response");
			}
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag4 = base.State == SIP_TransactionState.Disposed;
				if (!flag4)
				{
					bool flag5 = this.m_IsCanceling && response.StatusCodeType == SIP_StatusCodeType.Provisional;
					if (flag5)
					{
						this.SendCancel();
					}
					else
					{
						bool flag6 = base.Stack.Logger != null;
						if (flag6)
						{
							byte[] array = response.ToByteData();
							base.Stack.Logger.AddRead(Guid.NewGuid().ToString(), null, 0L, string.Concat(new object[]
							{
								"Response [transactionID='",
								base.ID,
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
								"'; received '",
								flow.LocalEP,
								"' <- '",
								flow.RemoteEP,
								"'."
							}), flow.LocalEP, flow.RemoteEP, array);
						}
						bool flag7 = base.Method == "INVITE";
						if (flag7)
						{
							bool flag8 = base.State == SIP_TransactionState.Calling;
							if (flag8)
							{
								base.AddResponse(response);
								bool flag9 = this.m_pTimerA != null;
								if (flag9)
								{
									this.m_pTimerA.Dispose();
									this.m_pTimerA = null;
									bool flag10 = base.Stack.Logger != null;
									if (flag10)
									{
										base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
										{
											"Transaction [branch='",
											base.ID,
											"';method='",
											base.Method,
											"';IsServer=false] timer A(INVITE request retransmission) stopped."
										}));
									}
								}
								bool flag11 = this.m_pTimerB != null;
								if (flag11)
								{
									this.m_pTimerB.Dispose();
									this.m_pTimerB = null;
									bool flag12 = base.Stack.Logger != null;
									if (flag12)
									{
										base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
										{
											"Transaction [branch='",
											base.ID,
											"';method='",
											base.Method,
											"';IsServer=false] timer B(INVITE calling state timeout) stopped."
										}));
									}
								}
								bool flag13 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
								if (flag13)
								{
									this.OnResponseReceived(response);
									base.SetState(SIP_TransactionState.Proceeding);
								}
								else
								{
									bool flag14 = response.StatusCodeType == SIP_StatusCodeType.Success;
									if (flag14)
									{
										this.OnResponseReceived(response);
										base.SetState(SIP_TransactionState.Accpeted);
										this.m_pTimerM = new TimerEx(32000.0);
										this.m_pTimerM.Elapsed += this.m_pTimerM_Elapsed;
										this.m_pTimerM.Enabled = true;
										bool flag15 = base.Stack.Logger != null;
										if (flag15)
										{
											base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
											{
												"Transaction [branch='",
												base.ID,
												"';method='",
												base.Method,
												"';IsServer=true] timer M(2xx retransmission wait) started, will trigger after ",
												this.m_pTimerM.Interval,
												"."
											}));
										}
									}
									else
									{
										this.SendAck(response);
										this.OnResponseReceived(response);
										base.SetState(SIP_TransactionState.Completed);
										this.m_pTimerD = new TimerEx((double)(base.Flow.IsReliable ? 0 : 32000), false);
										this.m_pTimerD.Elapsed += this.m_pTimerD_Elapsed;
										bool flag16 = base.Stack.Logger != null;
										if (flag16)
										{
											base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
											{
												"Transaction [branch='",
												base.ID,
												"';method='",
												base.Method,
												"';IsServer=false] timer D(INVITE 3xx - 6xx response retransmission wait) started, will trigger after ",
												this.m_pTimerD.Interval,
												"."
											}));
										}
										this.m_pTimerD.Enabled = true;
									}
								}
							}
							else
							{
								bool flag17 = base.State == SIP_TransactionState.Proceeding;
								if (flag17)
								{
									base.AddResponse(response);
									bool flag18 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
									if (flag18)
									{
										this.OnResponseReceived(response);
									}
									else
									{
										bool flag19 = response.StatusCodeType == SIP_StatusCodeType.Success;
										if (flag19)
										{
											this.OnResponseReceived(response);
											base.SetState(SIP_TransactionState.Accpeted);
											this.m_pTimerM = new TimerEx(32000.0);
											this.m_pTimerM.Elapsed += this.m_pTimerM_Elapsed;
											this.m_pTimerM.Enabled = true;
											bool flag20 = base.Stack.Logger != null;
											if (flag20)
											{
												base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
												{
													"Transaction [branch='",
													base.ID,
													"';method='",
													base.Method,
													"';IsServer=true] timer M(2xx retransmission wait) started, will trigger after ",
													this.m_pTimerM.Interval,
													"."
												}));
											}
										}
										else
										{
											this.SendAck(response);
											this.OnResponseReceived(response);
											base.SetState(SIP_TransactionState.Completed);
											this.m_pTimerD = new TimerEx((double)(base.Flow.IsReliable ? 0 : 32000), false);
											this.m_pTimerD.Elapsed += this.m_pTimerD_Elapsed;
											bool flag21 = base.Stack.Logger != null;
											if (flag21)
											{
												base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
												{
													"Transaction [branch='",
													base.ID,
													"';method='",
													base.Method,
													"';IsServer=false] timer D(INVITE 3xx - 6xx response retransmission wait) started, will trigger after ",
													this.m_pTimerD.Interval,
													"."
												}));
											}
											this.m_pTimerD.Enabled = true;
										}
									}
								}
								else
								{
									bool flag22 = base.State == SIP_TransactionState.Accpeted;
									if (flag22)
									{
										bool flag23 = response.StatusCodeType == SIP_StatusCodeType.Success;
										if (flag23)
										{
											this.OnResponseReceived(response);
										}
									}
									else
									{
										bool flag24 = base.State == SIP_TransactionState.Completed;
										if (flag24)
										{
											bool flag25 = response.StatusCode >= 300;
											if (flag25)
											{
												this.SendAck(response);
											}
										}
										else
										{
											bool flag26 = base.State == SIP_TransactionState.Terminated;
											if (flag26)
											{
											}
										}
									}
								}
							}
						}
						else
						{
							bool flag27 = base.State == SIP_TransactionState.Trying;
							if (flag27)
							{
								base.AddResponse(response);
								bool flag28 = this.m_pTimerE != null;
								if (flag28)
								{
									this.m_pTimerE.Dispose();
									this.m_pTimerE = null;
									bool flag29 = base.Stack.Logger != null;
									if (flag29)
									{
										base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
										{
											"Transaction [branch='",
											base.ID,
											"';method='",
											base.Method,
											"';IsServer=false] timer E(Non-INVITE request retransmission) stopped."
										}));
									}
								}
								bool flag30 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
								if (flag30)
								{
									this.OnResponseReceived(response);
									base.SetState(SIP_TransactionState.Proceeding);
								}
								else
								{
									bool flag31 = this.m_pTimerF != null;
									if (flag31)
									{
										this.m_pTimerF.Dispose();
										this.m_pTimerF = null;
										bool flag32 = base.Stack.Logger != null;
										if (flag32)
										{
											base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
											{
												"Transaction [branch='",
												base.ID,
												"';method='",
												base.Method,
												"';IsServer=false] timer F(Non-INVITE trying,proceeding state timeout) stopped."
											}));
										}
									}
									this.OnResponseReceived(response);
									base.SetState(SIP_TransactionState.Completed);
									this.m_pTimerK = new TimerEx((double)(base.Flow.IsReliable ? 1 : 5000), false);
									this.m_pTimerK.Elapsed += this.m_pTimerK_Elapsed;
									bool flag33 = base.Stack.Logger != null;
									if (flag33)
									{
										base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
										{
											"Transaction [branch='",
											base.ID,
											"';method='",
											base.Method,
											"';IsServer=false] timer K(Non-INVITE 3xx - 6xx response retransmission wait) started, will trigger after ",
											this.m_pTimerK.Interval,
											"."
										}));
									}
									this.m_pTimerK.Enabled = true;
								}
							}
							else
							{
								bool flag34 = base.State == SIP_TransactionState.Proceeding;
								if (flag34)
								{
									base.AddResponse(response);
									bool flag35 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
									if (flag35)
									{
										this.OnResponseReceived(response);
									}
									else
									{
										bool flag36 = this.m_pTimerF != null;
										if (flag36)
										{
											this.m_pTimerF.Dispose();
											this.m_pTimerF = null;
											bool flag37 = base.Stack.Logger != null;
											if (flag37)
											{
												base.Stack.Logger.AddText(base.ID, string.Concat(new string[]
												{
													"Transaction [branch='",
													base.ID,
													"';method='",
													base.Method,
													"';IsServer=false] timer F(Non-INVITE trying,proceeding state timeout) stopped."
												}));
											}
										}
										this.OnResponseReceived(response);
										base.SetState(SIP_TransactionState.Completed);
										this.m_pTimerK = new TimerEx((double)(base.Flow.IsReliable ? 0 : 5000), false);
										this.m_pTimerK.Elapsed += this.m_pTimerK_Elapsed;
										bool flag38 = base.Stack.Logger != null;
										if (flag38)
										{
											base.Stack.Logger.AddText(base.ID, string.Concat(new object[]
											{
												"Transaction [branch='",
												base.ID,
												"';method='",
												base.Method,
												"';IsServer=false] timer K(Non-INVITE 3xx - 6xx response retransmission wait) started, will trigger after ",
												this.m_pTimerK.Interval,
												"."
											}));
										}
										this.m_pTimerK.Enabled = true;
									}
								}
								else
								{
									bool flag39 = base.State == SIP_TransactionState.Completed;
									if (!flag39)
									{
										bool flag40 = base.State == SIP_TransactionState.Terminated;
										if (flag40)
										{
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x0001C1FC File Offset: 0x0001B1FC
		private void SendCancel()
		{
			SIP_Request sip_Request = new SIP_Request("CANCEL");
			sip_Request.RequestLine.Uri = base.Request.RequestLine.Uri;
			sip_Request.Via.Add(base.Request.Via.GetTopMostValue().ToStringValue());
			sip_Request.CallID = base.Request.CallID;
			sip_Request.From = base.Request.From;
			sip_Request.To = base.Request.To;
			sip_Request.CSeq = new SIP_t_CSeq(base.Request.CSeq.SequenceNumber, "CANCEL");
			foreach (SIP_t_AddressParam sip_t_AddressParam in base.Request.Route.GetAllValues())
			{
				sip_Request.Route.Add(sip_t_AddressParam.ToStringValue());
			}
			sip_Request.MaxForwards = 70;
			SIP_ClientTransaction sip_ClientTransaction = base.Stack.TransactionLayer.CreateClientTransaction(base.Flow, sip_Request, false);
			sip_ClientTransaction.Start();
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0001C30C File Offset: 0x0001B30C
		private void SendAck(SIP_Response response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("resposne");
			}
			SIP_Request sip_Request = new SIP_Request("ACK");
			sip_Request.RequestLine.Uri = base.Request.RequestLine.Uri;
			sip_Request.Via.AddToTop(base.Request.Via.GetTopMostValue().ToStringValue());
			sip_Request.CallID = base.Request.CallID;
			sip_Request.From = base.Request.From;
			sip_Request.To = response.To;
			sip_Request.CSeq = new SIP_t_CSeq(base.Request.CSeq.SequenceNumber, "ACK");
			foreach (SIP_HeaderField sip_HeaderField in response.Header.Get("Route:"))
			{
				sip_Request.Header.Add("Route:", sip_HeaderField.Value);
			}
			sip_Request.MaxForwards = 70;
			try
			{
				base.Stack.TransportLayer.SendRequest(base.Flow, sip_Request, this);
			}
			catch (SIP_TransportException exception)
			{
				base.OnTransportError(exception);
				base.SetState(SIP_TransactionState.Terminated);
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000532 RID: 1330 RVA: 0x0001C454 File Offset: 0x0001B454
		// (set) Token: 0x06000533 RID: 1331 RVA: 0x0001C46C File Offset: 0x0001B46C
		internal int RSeq
		{
			get
			{
				return this.m_RSeq;
			}
			set
			{
				this.m_RSeq = value;
			}
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06000534 RID: 1332 RVA: 0x0001C478 File Offset: 0x0001B478
		// (remove) Token: 0x06000535 RID: 1333 RVA: 0x0001C4B0 File Offset: 0x0001B4B0
		
		public event EventHandler<SIP_ResponseReceivedEventArgs> ResponseReceived = null;

		// Token: 0x06000536 RID: 1334 RVA: 0x0001C4E8 File Offset: 0x0001B4E8
		private void OnResponseReceived(SIP_Response response)
		{
			bool flag = this.ResponseReceived != null;
			if (flag)
			{
				this.ResponseReceived(this, new SIP_ResponseReceivedEventArgs(base.Stack, this, response));
			}
		}

		// Token: 0x040001B1 RID: 433
		private TimerEx m_pTimerA = null;

		// Token: 0x040001B2 RID: 434
		private TimerEx m_pTimerB = null;

		// Token: 0x040001B3 RID: 435
		private TimerEx m_pTimerD = null;

		// Token: 0x040001B4 RID: 436
		private TimerEx m_pTimerE = null;

		// Token: 0x040001B5 RID: 437
		private TimerEx m_pTimerF = null;

		// Token: 0x040001B6 RID: 438
		private TimerEx m_pTimerK = null;

		// Token: 0x040001B7 RID: 439
		private TimerEx m_pTimerM = null;

		// Token: 0x040001B8 RID: 440
		private bool m_IsCanceling = false;

		// Token: 0x040001B9 RID: 441
		private int m_RSeq = -1;
	}
}
