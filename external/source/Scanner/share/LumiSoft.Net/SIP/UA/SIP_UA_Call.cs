using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using LumiSoft.Net.SDP;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.UA
{
	// Token: 0x0200007E RID: 126
	[Obsolete("Use SIP stack instead.")]
	public class SIP_UA_Call : IDisposable
	{
		// Token: 0x0600049F RID: 1183 RVA: 0x00017374 File Offset: 0x00016374
		internal SIP_UA_Call(SIP_UA ua, SIP_Request invite)
		{
			bool flag = ua == null;
			if (flag)
			{
				throw new ArgumentNullException("ua");
			}
			bool flag2 = invite == null;
			if (flag2)
			{
				throw new ArgumentNullException("invite");
			}
			bool flag3 = invite.RequestLine.Method != "INVITE";
			if (flag3)
			{
				throw new ArgumentException("Argument 'invite' is not INVITE request.");
			}
			this.m_pUA = ua;
			this.m_pInvite = invite;
			this.m_pLocalUri = invite.From.Address.Uri;
			this.m_pRemoteUri = invite.To.Address.Uri;
			this.m_State = SIP_UA_CallState.WaitingForStart;
			this.m_pEarlyDialogs = new List<SIP_Dialog_Invite>();
			this.m_pTags = new Dictionary<string, object>();
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001749C File Offset: 0x0001649C
		internal SIP_UA_Call(SIP_UA ua, SIP_ServerTransaction invite)
		{
			bool flag = ua == null;
			if (flag)
			{
				throw new ArgumentNullException("ua");
			}
			bool flag2 = invite == null;
			if (flag2)
			{
				throw new ArgumentNullException("invite");
			}
			this.m_pUA = ua;
			this.m_pInitialInviteTransaction = invite;
			this.m_pLocalUri = invite.Request.To.Address.Uri;
			this.m_pRemoteUri = invite.Request.From.Address.Uri;
			this.m_pInitialInviteTransaction.Canceled += delegate(object sender, EventArgs e)
			{
				this.SetState(SIP_UA_CallState.Terminated);
			};
			bool flag3 = invite.Request.ContentType != null && invite.Request.ContentType.ToLower().IndexOf("application/sdp") > -1;
			if (flag3)
			{
				this.m_pRemoteSDP = SDP_Message.Parse(Encoding.UTF8.GetString(invite.Request.Data));
			}
			this.m_pTags = new Dictionary<string, object>();
			this.m_State = SIP_UA_CallState.WaitingToAccept;
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x00017608 File Offset: 0x00016608
		public void Dispose()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.m_State == SIP_UA_CallState.Disposed;
				if (!flag2)
				{
					this.SetState(SIP_UA_CallState.Disposed);
					this.StateChanged = null;
				}
			}
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x00017668 File Offset: 0x00016668
		private void m_pDialog_StateChanged(object sender, EventArgs e)
		{
			bool flag = this.State == SIP_UA_CallState.Terminated;
			if (!flag)
			{
				bool flag2 = this.m_pDialog.State == SIP_DialogState.Terminated;
				if (flag2)
				{
					this.SetState(SIP_UA_CallState.Terminated);
					this.m_pDialog.Dispose();
				}
			}
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x000176B0 File Offset: 0x000166B0
		private void m_pInitialInviteSender_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
		{
			try
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool flag2 = e.Response.ContentType != null && e.Response.ContentType.ToLower().IndexOf("application/sdp") > -1;
					if (flag2)
					{
						this.m_pRemoteSDP = SDP_Message.Parse(Encoding.UTF8.GetString(e.Response.Data));
					}
					bool flag3 = e.Response.StatusCodeType == SIP_StatusCodeType.Provisional;
					if (flag3)
					{
						bool flag4 = e.Response.StatusCode == 180;
						if (flag4)
						{
							this.SetState(SIP_UA_CallState.Ringing);
						}
						else
						{
							bool flag5 = e.Response.StatusCode == 182;
							if (flag5)
							{
								this.SetState(SIP_UA_CallState.Queued);
							}
						}
						bool flag6 = e.Response.StatusCode > 100 && e.Response.To.Tag != null;
						if (flag6)
						{
							this.m_pEarlyDialogs.Add((SIP_Dialog_Invite)this.m_pUA.Stack.TransactionLayer.GetOrCreateDialog(e.ClientTransaction, e.Response));
						}
					}
					else
					{
						bool flag7 = e.Response.StatusCodeType == SIP_StatusCodeType.Success;
						if (flag7)
						{
							this.m_StartTime = DateTime.Now;
							this.SetState(SIP_UA_CallState.Active);
							this.m_pDialog = this.m_pUA.Stack.TransactionLayer.GetOrCreateDialog(e.ClientTransaction, e.Response);
							this.m_pDialog.StateChanged += this.m_pDialog_StateChanged;
							foreach (SIP_Dialog_Invite sip_Dialog_Invite in this.m_pEarlyDialogs.ToArray())
							{
								bool flag8 = !this.m_pDialog.Equals(sip_Dialog_Invite);
								if (flag8)
								{
									sip_Dialog_Invite.Terminate("Another forking leg accepted.", true);
								}
							}
						}
						else
						{
							foreach (SIP_Dialog_Invite sip_Dialog_Invite2 in this.m_pEarlyDialogs.ToArray())
							{
								sip_Dialog_Invite2.Terminate("All early dialogs are considered terminated upon reception of the non-2xx final response. (RFC 3261 13.2.2.3)", false);
							}
							this.m_pEarlyDialogs.Clear();
							this.Error();
							this.SetState(SIP_UA_CallState.Terminated);
						}
					}
				}
			}
			catch (Exception x)
			{
				this.m_pUA.Stack.OnError(x);
			}
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x00017958 File Offset: 0x00016958
		public void Start()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.m_State == SIP_UA_CallState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = this.m_State > SIP_UA_CallState.WaitingForStart;
				if (flag3)
				{
					throw new InvalidOperationException("Start method can be called only in 'SIP_UA_CallState.WaitingForStart' state.");
				}
				this.SetState(SIP_UA_CallState.Calling);
				this.m_pInitialInviteSender = this.m_pUA.Stack.CreateRequestSender(this.m_pInvite);
				this.m_pInitialInviteSender.ResponseReceived += this.m_pInitialInviteSender_ResponseReceived;
				this.m_pInitialInviteSender.Start();
			}
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x00017A18 File Offset: 0x00016A18
		public void SendRinging(SDP_Message sdp)
		{
			bool flag = this.m_State == SIP_UA_CallState.Disposed;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag2 = this.m_State != SIP_UA_CallState.WaitingToAccept;
			if (flag2)
			{
				throw new InvalidOperationException("Accept method can be called only in 'SIP_UA_CallState.WaitingToAccept' state.");
			}
			SIP_Response sip_Response = this.m_pUA.Stack.CreateResponse(SIP_ResponseCodes.x180_Ringing, this.m_pInitialInviteTransaction.Request, this.m_pInitialInviteTransaction.Flow);
			bool flag3 = sdp != null;
			if (flag3)
			{
				sip_Response.ContentType = "application/sdp";
				sip_Response.Data = sdp.ToByte();
				this.m_pLocalSDP = sdp;
			}
			this.m_pInitialInviteTransaction.SendResponse(sip_Response);
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x00017AC4 File Offset: 0x00016AC4
		public void Accept(SDP_Message sdp)
		{
			bool flag = this.m_State == SIP_UA_CallState.Disposed;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag2 = this.m_State != SIP_UA_CallState.WaitingToAccept;
			if (flag2)
			{
				throw new InvalidOperationException("Accept method can be called only in 'SIP_UA_CallState.WaitingToAccept' state.");
			}
			bool flag3 = sdp == null;
			if (flag3)
			{
				throw new ArgumentNullException("sdp");
			}
			this.m_pLocalSDP = sdp;
			SIP_Response sip_Response = this.m_pUA.Stack.CreateResponse(SIP_ResponseCodes.x200_Ok, this.m_pInitialInviteTransaction.Request, this.m_pInitialInviteTransaction.Flow);
			sip_Response.ContentType = "application/sdp";
			sip_Response.Data = sdp.ToByte();
			this.m_pInitialInviteTransaction.SendResponse(sip_Response);
			this.SetState(SIP_UA_CallState.Active);
			this.m_pDialog = this.m_pUA.Stack.TransactionLayer.GetOrCreateDialog(this.m_pInitialInviteTransaction, sip_Response);
			this.m_pDialog.StateChanged += this.m_pDialog_StateChanged;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00017BBC File Offset: 0x00016BBC
		public void Reject(string statusCode_reason)
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.State == SIP_UA_CallState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = this.State != SIP_UA_CallState.WaitingToAccept;
				if (flag3)
				{
					throw new InvalidOperationException("Call is not in valid state.");
				}
				bool flag4 = statusCode_reason == null;
				if (flag4)
				{
					throw new ArgumentNullException("statusCode_reason");
				}
				this.m_pInitialInviteTransaction.SendResponse(this.m_pUA.Stack.CreateResponse(statusCode_reason, this.m_pInitialInviteTransaction.Request));
				this.SetState(SIP_UA_CallState.Terminated);
			}
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00017C7C File Offset: 0x00016C7C
		public void Redirect(SIP_t_ContactParam[] contacts)
		{
			object pLock = this.m_pLock;
			bool flag = false;
			try
			{
				Monitor.Enter(pLock, ref flag);
				bool flag2 = this.State == SIP_UA_CallState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = this.State != SIP_UA_CallState.WaitingToAccept;
				if (flag3)
				{
					throw new InvalidOperationException("Call is not in valid state.");
				}
				bool flag4 = contacts == null;
				if (flag4)
				{
					throw new ArgumentNullException("contacts");
				}
				bool flag5 = contacts.Length == 0;
				if (flag5)
				{
					throw new ArgumentException("Arguments 'contacts' must contain at least 1 value.");
				}
				throw new NotImplementedException();
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(pLock);
					goto IL_8C;
				}
				goto IL_8C;
				IL_8C:;
			}
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00017D28 File Offset: 0x00016D28
		public void Terminate()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.m_State == SIP_UA_CallState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = this.m_State == SIP_UA_CallState.Terminating || this.m_State == SIP_UA_CallState.Terminated;
				if (!flag3)
				{
					bool flag4 = this.m_State == SIP_UA_CallState.WaitingForStart;
					if (flag4)
					{
						this.SetState(SIP_UA_CallState.Terminated);
					}
					else
					{
						bool flag5 = this.m_State == SIP_UA_CallState.WaitingToAccept;
						if (flag5)
						{
							this.m_pInitialInviteTransaction.SendResponse(this.m_pUA.Stack.CreateResponse(SIP_ResponseCodes.x487_Request_Terminated, this.m_pInitialInviteTransaction.Request));
							this.SetState(SIP_UA_CallState.Terminated);
						}
						else
						{
							bool flag6 = this.m_State == SIP_UA_CallState.Active;
							if (flag6)
							{
								this.m_pDialog.Terminate();
								this.SetState(SIP_UA_CallState.Terminated);
							}
							else
							{
								bool flag7 = this.m_pInitialInviteSender != null;
								if (flag7)
								{
									this.SetState(SIP_UA_CallState.Terminating);
									this.m_pInitialInviteSender.Cancel();
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00017E58 File Offset: 0x00016E58
		public void ToggleOnHold()
		{
			throw new NotImplementedException();
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00017E58 File Offset: 0x00016E58
		public void Transfer()
		{
			throw new NotImplementedException();
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00017E60 File Offset: 0x00016E60
		private void SetState(SIP_UA_CallState state)
		{
			this.m_State = state;
			this.OnStateChanged(state);
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060004AD RID: 1197 RVA: 0x00017E74 File Offset: 0x00016E74
		public bool IsDisposed
		{
			get
			{
				return this.m_State == SIP_UA_CallState.Disposed;
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x00017E90 File Offset: 0x00016E90
		public SIP_UA_CallState State
		{
			get
			{
				return this.m_State;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060004AF RID: 1199 RVA: 0x00017EA8 File Offset: 0x00016EA8
		public DateTime StartTime
		{
			get
			{
				return this.m_StartTime;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x00017EC0 File Offset: 0x00016EC0
		public AbsoluteUri LocalUri
		{
			get
			{
				return this.m_pLocalUri;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060004B1 RID: 1201 RVA: 0x00017ED8 File Offset: 0x00016ED8
		public AbsoluteUri RemoteUri
		{
			get
			{
				return this.m_pRemoteUri;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x00017EF0 File Offset: 0x00016EF0
		public SDP_Message LocalSDP
		{
			get
			{
				return this.m_pLocalSDP;
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060004B3 RID: 1203 RVA: 0x00017F08 File Offset: 0x00016F08
		public SDP_Message RemoteSDP
		{
			get
			{
				return this.m_pRemoteSDP;
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x00017F20 File Offset: 0x00016F20
		public int Duration
		{
			get
			{
				return (DateTime.Now - this.m_StartTime).Seconds;
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x00017F4C File Offset: 0x00016F4C
		public bool IsRedirected
		{
			get
			{
				return this.m_IsRedirected;
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x00017F64 File Offset: 0x00016F64
		public bool IsOnhold
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060004B7 RID: 1207 RVA: 0x00017F78 File Offset: 0x00016F78
		public Dictionary<string, object> Tag
		{
			get
			{
				return this.m_pTags;
			}
		}

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x060004B8 RID: 1208 RVA: 0x00017F90 File Offset: 0x00016F90
		// (remove) Token: 0x060004B9 RID: 1209 RVA: 0x00017FC8 File Offset: 0x00016FC8
		
		public event EventHandler StateChanged = null;

		// Token: 0x060004BA RID: 1210 RVA: 0x00018000 File Offset: 0x00017000
		private void OnStateChanged(SIP_UA_CallState state)
		{
			bool flag = this.StateChanged != null;
			if (flag)
			{
				this.StateChanged(this, new EventArgs());
			}
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x000091B8 File Offset: 0x000081B8
		private void Error()
		{
		}

		// Token: 0x04000163 RID: 355
		private SIP_UA_CallState m_State = SIP_UA_CallState.WaitingForStart;

		// Token: 0x04000164 RID: 356
		private SIP_UA m_pUA = null;

		// Token: 0x04000165 RID: 357
		private SIP_Request m_pInvite = null;

		// Token: 0x04000166 RID: 358
		private SDP_Message m_pLocalSDP = null;

		// Token: 0x04000167 RID: 359
		private SDP_Message m_pRemoteSDP = null;

		// Token: 0x04000168 RID: 360
		private DateTime m_StartTime;

		// Token: 0x04000169 RID: 361
		private List<SIP_Dialog_Invite> m_pEarlyDialogs = null;

		// Token: 0x0400016A RID: 362
		private SIP_Dialog m_pDialog = null;

		// Token: 0x0400016B RID: 363
		private bool m_IsRedirected = false;

		// Token: 0x0400016C RID: 364
		private SIP_RequestSender m_pInitialInviteSender = null;

		// Token: 0x0400016D RID: 365
		private SIP_ServerTransaction m_pInitialInviteTransaction = null;

		// Token: 0x0400016E RID: 366
		private AbsoluteUri m_pLocalUri = null;

		// Token: 0x0400016F RID: 367
		private AbsoluteUri m_pRemoteUri = null;

		// Token: 0x04000170 RID: 368
		private Dictionary<string, object> m_pTags = null;

		// Token: 0x04000171 RID: 369
		private object m_pLock = "";
	}
}
