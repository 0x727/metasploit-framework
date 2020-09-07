using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Timers;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000093 RID: 147
	public class SIP_UA_Registration
	{
		// Token: 0x0600057D RID: 1405 RVA: 0x0001F598 File Offset: 0x0001E598
		internal SIP_UA_Registration(SIP_Stack stack, SIP_Uri server, string aor, AbsoluteUri contact, int expires)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
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
			this.m_pStack = stack;
			this.m_pServer = server;
			this.m_AOR = aor;
			this.m_pContact = contact;
			this.m_RefreshInterval = expires;
			this.m_pContacts = new List<AbsoluteUri>();
			this.m_pTimer = new TimerEx((double)((this.m_RefreshInterval - 15) * 1000));
			this.m_pTimer.AutoReset = false;
			this.m_pTimer.Elapsed += this.m_pTimer_Elapsed;
			this.m_pTimer.Enabled = false;
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x0001F728 File Offset: 0x0001E728
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.m_pStack = null;
				this.m_pTimer.Dispose();
				this.m_pTimer = null;
				this.SetState(SIP_UA_RegistrationState.Disposed);
				this.OnDisposed();
				this.Registered = null;
				this.Unregistered = null;
				this.Error = null;
				this.Disposed = null;
			}
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0001F790 File Offset: 0x0001E790
		private void m_pTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			bool flag = this.m_pStack.State == SIP_StackState.Started;
			if (flag)
			{
				this.BeginRegister(this.m_AutoRefresh);
			}
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x0001F7C0 File Offset: 0x0001E7C0
		private void m_pRegisterSender_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
		{
			this.m_pFlow = e.ClientTransaction.Flow;
			bool flag = e.Response.StatusCodeType == SIP_StatusCodeType.Provisional;
			if (!flag)
			{
				bool flag2 = e.Response.StatusCodeType == SIP_StatusCodeType.Success;
				if (flag2)
				{
					this.m_pContacts.Clear();
					foreach (SIP_t_ContactParam sip_t_ContactParam in e.Response.Contact.GetAllValues())
					{
						this.m_pContacts.Add(sip_t_ContactParam.Address.Uri);
					}
					this.SetState(SIP_UA_RegistrationState.Registered);
					this.OnRegistered();
					this.m_pFlow.SendKeepAlives = true;
				}
				else
				{
					this.SetState(SIP_UA_RegistrationState.Error);
					this.OnError(e);
				}
				bool flag3 = this.AutoFixContact && this.m_pContact is SIP_Uri;
				if (flag3)
				{
					SIP_Uri sip_Uri = (SIP_Uri)this.m_pContact;
					IPAddress ipaddress = Net_Utils.IsIPAddress(sip_Uri.Host) ? IPAddress.Parse(sip_Uri.Host) : null;
					SIP_t_ViaParm topMostValue = e.Response.Via.GetTopMostValue();
					bool flag4 = topMostValue != null && ipaddress != null;
					if (flag4)
					{
						IPEndPoint ipendPoint = new IPEndPoint((topMostValue.Received != null) ? topMostValue.Received : ipaddress, (topMostValue.RPort > 0) ? topMostValue.RPort : sip_Uri.Port);
						bool flag5 = !ipaddress.Equals(ipendPoint.Address) || sip_Uri.Port != topMostValue.RPort;
						if (flag5)
						{
							this.BeginUnregister(false);
							sip_Uri.Host = ipendPoint.Address.ToString();
							sip_Uri.Port = ipendPoint.Port;
							this.m_pRegisterSender.Dispose();
							this.m_pRegisterSender = null;
							this.BeginRegister(this.m_AutoRefresh);
							return;
						}
					}
				}
				bool autoRefresh = this.m_AutoRefresh;
				if (autoRefresh)
				{
					this.m_pTimer.Enabled = true;
				}
				this.m_pRegisterSender.Dispose();
				this.m_pRegisterSender = null;
			}
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x0001F9DC File Offset: 0x0001E9DC
		private void m_pUnregisterSender_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
		{
			this.SetState(SIP_UA_RegistrationState.Unregistered);
			this.OnUnregistered();
			bool autoDispose = this.m_AutoDispose;
			if (autoDispose)
			{
				this.Dispose();
			}
			this.m_pUnregisterSender = null;
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x0001FA14 File Offset: 0x0001EA14
		public void BeginRegister(bool autoRefresh)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = this.m_pStack.State > SIP_StackState.Started;
			if (flag)
			{
				this.m_pTimer.Enabled = true;
			}
			else
			{
				this.m_AutoRefresh = autoRefresh;
				this.SetState(SIP_UA_RegistrationState.Registering);
				SIP_Request sip_Request = this.m_pStack.CreateRequest("REGISTER", new SIP_t_NameAddress(this.m_pServer.Scheme + ":" + this.m_AOR), new SIP_t_NameAddress(this.m_pServer.Scheme + ":" + this.m_AOR));
				sip_Request.RequestLine.Uri = SIP_Uri.Parse(this.m_pServer.Scheme + ":" + this.m_AOR.Substring(this.m_AOR.IndexOf('@') + 1));
				sip_Request.Route.Add(this.m_pServer.ToString());
				sip_Request.Contact.Add(string.Concat(new object[]
				{
					"<",
					this.Contact,
					">;expires=",
					this.m_RefreshInterval
				}));
				this.m_pRegisterSender = this.m_pStack.CreateRequestSender(sip_Request, this.m_pFlow);
				this.m_pRegisterSender.ResponseReceived += this.m_pRegisterSender_ResponseReceived;
				this.m_pRegisterSender.Start();
			}
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x0001FB94 File Offset: 0x0001EB94
		public void BeginUnregister(bool dispose)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			this.m_AutoDispose = dispose;
			this.m_pTimer.Enabled = false;
			bool flag = this.m_State == SIP_UA_RegistrationState.Registered;
			if (flag)
			{
				SIP_Request sip_Request = this.m_pStack.CreateRequest("REGISTER", new SIP_t_NameAddress(this.m_pServer.Scheme + ":" + this.m_AOR), new SIP_t_NameAddress(this.m_pServer.Scheme + ":" + this.m_AOR));
				sip_Request.RequestLine.Uri = SIP_Uri.Parse(this.m_pServer.Scheme + ":" + this.m_AOR.Substring(this.m_AOR.IndexOf('@') + 1));
				sip_Request.Route.Add(this.m_pServer.ToString());
				sip_Request.Contact.Add("<" + this.Contact + ">;expires=0");
				this.m_pUnregisterSender = this.m_pStack.CreateRequestSender(sip_Request, this.m_pFlow);
				this.m_pUnregisterSender.ResponseReceived += this.m_pUnregisterSender_ResponseReceived;
				this.m_pUnregisterSender.Start();
			}
			else
			{
				this.SetState(SIP_UA_RegistrationState.Unregistered);
				this.OnUnregistered();
				bool autoDispose = this.m_AutoDispose;
				if (autoDispose)
				{
					this.Dispose();
				}
				this.m_pUnregisterSender = null;
			}
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x0001FD16 File Offset: 0x0001ED16
		private void SetState(SIP_UA_RegistrationState newState)
		{
			this.m_State = newState;
			this.OnStateChanged();
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x0001FD28 File Offset: 0x0001ED28
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x0001FD40 File Offset: 0x0001ED40
		public SIP_UA_RegistrationState State
		{
			get
			{
				return this.m_State;
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x0001FD58 File Offset: 0x0001ED58
		public int Expires
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return 3600;
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x0001FD8C File Offset: 0x0001ED8C
		public string AOR
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_AOR;
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x0001FDC0 File Offset: 0x0001EDC0
		public AbsoluteUri Contact
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pContact;
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x0600058A RID: 1418 RVA: 0x0001FDF4 File Offset: 0x0001EDF4
		public AbsoluteUri[] Contacts
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pContacts.ToArray();
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x0600058B RID: 1419 RVA: 0x0001FE30 File Offset: 0x0001EE30
		public bool AutoFixContact
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return false;
			}
		}

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x0600058C RID: 1420 RVA: 0x0001FE60 File Offset: 0x0001EE60
		// (remove) Token: 0x0600058D RID: 1421 RVA: 0x0001FE98 File Offset: 0x0001EE98
		
		public event EventHandler StateChanged = null;

		// Token: 0x0600058E RID: 1422 RVA: 0x0001FED0 File Offset: 0x0001EED0
		private void OnStateChanged()
		{
			bool flag = this.StateChanged != null;
			if (flag)
			{
				this.StateChanged(this, new EventArgs());
			}
		}

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x0600058F RID: 1423 RVA: 0x0001FF00 File Offset: 0x0001EF00
		// (remove) Token: 0x06000590 RID: 1424 RVA: 0x0001FF38 File Offset: 0x0001EF38
		
		public event EventHandler Registered = null;

		// Token: 0x06000591 RID: 1425 RVA: 0x0001FF70 File Offset: 0x0001EF70
		private void OnRegistered()
		{
			bool flag = this.Registered != null;
			if (flag)
			{
				this.Registered(this, new EventArgs());
			}
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06000592 RID: 1426 RVA: 0x0001FFA0 File Offset: 0x0001EFA0
		// (remove) Token: 0x06000593 RID: 1427 RVA: 0x0001FFD8 File Offset: 0x0001EFD8
		
		public event EventHandler Unregistered = null;

		// Token: 0x06000594 RID: 1428 RVA: 0x00020010 File Offset: 0x0001F010
		private void OnUnregistered()
		{
			bool flag = this.Unregistered != null;
			if (flag)
			{
				this.Unregistered(this, new EventArgs());
			}
		}

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06000595 RID: 1429 RVA: 0x00020040 File Offset: 0x0001F040
		// (remove) Token: 0x06000596 RID: 1430 RVA: 0x00020078 File Offset: 0x0001F078
		
		public event EventHandler<SIP_ResponseReceivedEventArgs> Error = null;

		// Token: 0x06000597 RID: 1431 RVA: 0x000200B0 File Offset: 0x0001F0B0
		private void OnError(SIP_ResponseReceivedEventArgs e)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, e);
			}
		}

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06000598 RID: 1432 RVA: 0x000200DC File Offset: 0x0001F0DC
		// (remove) Token: 0x06000599 RID: 1433 RVA: 0x00020114 File Offset: 0x0001F114
		
		public event EventHandler Disposed = null;

		// Token: 0x0600059A RID: 1434 RVA: 0x0002014C File Offset: 0x0001F14C
		private void OnDisposed()
		{
			bool flag = this.Disposed != null;
			if (flag)
			{
				this.Disposed(this, new EventArgs());
			}
		}

		// Token: 0x040001E5 RID: 485
		private bool m_IsDisposed = false;

		// Token: 0x040001E6 RID: 486
		private SIP_UA_RegistrationState m_State = SIP_UA_RegistrationState.Unregistered;

		// Token: 0x040001E7 RID: 487
		private SIP_Stack m_pStack = null;

		// Token: 0x040001E8 RID: 488
		private SIP_Uri m_pServer = null;

		// Token: 0x040001E9 RID: 489
		private string m_AOR = "";

		// Token: 0x040001EA RID: 490
		private AbsoluteUri m_pContact = null;

		// Token: 0x040001EB RID: 491
		private List<AbsoluteUri> m_pContacts = null;

		// Token: 0x040001EC RID: 492
		private int m_RefreshInterval = 300;

		// Token: 0x040001ED RID: 493
		private TimerEx m_pTimer = null;

		// Token: 0x040001EE RID: 494
		private SIP_RequestSender m_pRegisterSender = null;

		// Token: 0x040001EF RID: 495
		private SIP_RequestSender m_pUnregisterSender = null;

		// Token: 0x040001F0 RID: 496
		private bool m_AutoRefresh = true;

		// Token: 0x040001F1 RID: 497
		private bool m_AutoDispose = false;

		// Token: 0x040001F2 RID: 498
		private SIP_Flow m_pFlow = null;
	}
}
