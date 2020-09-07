using System;
using System.Diagnostics;
using System.Timers;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000AE RID: 174
	public class SIP_Registrar
	{
		// Token: 0x060006B0 RID: 1712 RVA: 0x00027888 File Offset: 0x00026888
		internal SIP_Registrar(SIP_Proxy proxy)
		{
			bool flag = proxy == null;
			if (flag)
			{
				throw new ArgumentNullException("proxy");
			}
			this.m_pProxy = proxy;
			this.m_pStack = this.m_pProxy.Stack;
			this.m_pRegistrations = new SIP_RegistrationCollection();
			this.m_pTimer = new Timer(15000.0);
			this.m_pTimer.Elapsed += this.m_pTimer_Elapsed;
			this.m_pTimer.Enabled = true;
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0002794C File Offset: 0x0002694C
		internal void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.CanRegister = null;
				this.AorRegistered = null;
				this.AorUnregistered = null;
				this.AorUpdated = null;
				this.m_pProxy = null;
				this.m_pStack = null;
				this.m_pRegistrations = null;
				bool flag = this.m_pTimer != null;
				if (flag)
				{
					this.m_pTimer.Dispose();
					this.m_pTimer = null;
				}
			}
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x000279C1 File Offset: 0x000269C1
		private void m_pTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.m_pRegistrations.RemoveExpired();
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x000279D0 File Offset: 0x000269D0
		public SIP_Registration GetRegistration(string aor)
		{
			return this.m_pRegistrations[aor];
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x000279EE File Offset: 0x000269EE
		public void SetRegistration(string aor, SIP_t_ContactParam[] contacts)
		{
			this.SetRegistration(aor, contacts, null);
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x000279FC File Offset: 0x000269FC
		public void SetRegistration(string aor, SIP_t_ContactParam[] contacts, SIP_Flow flow)
		{
			SIP_RegistrationCollection pRegistrations = this.m_pRegistrations;
			lock (pRegistrations)
			{
				SIP_Registration sip_Registration = this.m_pRegistrations[aor];
				bool flag2 = sip_Registration == null;
				if (flag2)
				{
					sip_Registration = new SIP_Registration("system", aor);
					this.m_pRegistrations.Add(sip_Registration);
					this.OnAorRegistered(sip_Registration);
				}
				sip_Registration.AddOrUpdateBindings(flow, "", 1, contacts);
			}
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00027A84 File Offset: 0x00026A84
		public void DeleteRegistration(string addressOfRecord)
		{
			this.m_pRegistrations.Remove(addressOfRecord);
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00027A94 File Offset: 0x00026A94
		internal void Register(SIP_RequestReceivedEventArgs e)
		{
			SIP_ServerTransaction serverTransaction = e.ServerTransaction;
			SIP_Request request = e.Request;
			string userName = "";
			bool flag = SIP_Utils.IsSipOrSipsUri(request.To.Address.Uri.ToString());
			if (flag)
			{
				SIP_Uri sip_Uri = (SIP_Uri)request.To.Address.Uri;
				bool flag2 = !this.m_pProxy.AuthenticateRequest(e, out userName);
				if (!flag2)
				{
					bool flag3 = !this.m_pProxy.OnAddressExists(sip_Uri.Address);
					if (flag3)
					{
						serverTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x404_Not_Found, request));
					}
					else
					{
						bool flag4 = !this.OnCanRegister(userName, sip_Uri.Address);
						if (flag4)
						{
							serverTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x403_Forbidden, request));
						}
						else
						{
							SIP_t_ContactParam sip_t_ContactParam = null;
							foreach (SIP_t_ContactParam sip_t_ContactParam2 in request.Contact.GetAllValues())
							{
								bool isStarContact = sip_t_ContactParam2.IsStarContact;
								if (isStarContact)
								{
									sip_t_ContactParam = sip_t_ContactParam2;
									break;
								}
							}
							bool flag5 = sip_t_ContactParam != null;
							if (flag5)
							{
								bool flag6 = request.Contact.GetAllValues().Length > 1;
								if (flag6)
								{
									serverTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x400_Bad_Request + ": RFC 3261 10.3.6 -> If star(*) present, only 1 contact allowed.", request));
									return;
								}
								bool flag7 = sip_t_ContactParam.Expires != 0;
								if (flag7)
								{
									serverTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x400_Bad_Request + ": RFC 3261 10.3.6 -> star(*) contact parameter 'expires' value must be always '0'.", request));
									return;
								}
								SIP_Registration sip_Registration = this.m_pRegistrations[sip_Uri.Address];
								bool flag8 = sip_Registration != null;
								if (flag8)
								{
									foreach (SIP_RegistrationBinding sip_RegistrationBinding in sip_Registration.Bindings)
									{
										bool flag9 = request.CallID != sip_RegistrationBinding.CallID || request.CSeq.SequenceNumber > sip_RegistrationBinding.CSeqNo;
										if (flag9)
										{
											sip_RegistrationBinding.Remove();
										}
									}
								}
							}
							bool flag10 = sip_t_ContactParam == null;
							if (flag10)
							{
								bool flag11 = false;
								SIP_Registration sip_Registration2 = this.m_pRegistrations[sip_Uri.Address];
								bool flag12 = sip_Registration2 == null;
								if (flag12)
								{
									flag11 = true;
									sip_Registration2 = new SIP_Registration(userName, sip_Uri.Address);
									this.m_pRegistrations.Add(sip_Registration2);
								}
								foreach (SIP_t_ContactParam sip_t_ContactParam3 in request.Contact.GetAllValues())
								{
									bool flag13 = sip_t_ContactParam3.Expires == -1;
									if (flag13)
									{
										sip_t_ContactParam3.Expires = request.Expires;
									}
									bool flag14 = sip_t_ContactParam3.Expires == -1;
									if (flag14)
									{
										sip_t_ContactParam3.Expires = this.m_pProxy.Stack.MinimumExpireTime;
									}
									bool flag15 = sip_t_ContactParam3.Expires != 0 && sip_t_ContactParam3.Expires < this.m_pProxy.Stack.MinimumExpireTime;
									if (flag15)
									{
										SIP_Response sip_Response = this.m_pStack.CreateResponse(SIP_ResponseCodes.x423_Interval_Too_Brief, request);
										sip_Response.MinExpires = this.m_pProxy.Stack.MinimumExpireTime;
										serverTransaction.SendResponse(sip_Response);
										return;
									}
									SIP_RegistrationBinding binding = sip_Registration2.GetBinding(sip_t_ContactParam3.Address.Uri);
									bool flag16 = binding != null && binding.CallID == request.CallID && request.CSeq.SequenceNumber < binding.CSeqNo;
									if (flag16)
									{
										serverTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x400_Bad_Request + ": CSeq value out of order.", request));
										return;
									}
								}
								sip_Registration2.AddOrUpdateBindings(e.ServerTransaction.Flow, request.CallID, request.CSeq.SequenceNumber, request.Contact.GetAllValues());
								bool flag17 = flag11;
								if (flag17)
								{
									this.OnAorRegistered(sip_Registration2);
								}
								else
								{
									this.OnAorUpdated(sip_Registration2);
								}
							}
							SIP_Response sip_Response2 = this.m_pStack.CreateResponse(SIP_ResponseCodes.x200_Ok, request);
							sip_Response2.Date = DateTime.Now;
							SIP_Registration sip_Registration3 = this.m_pRegistrations[sip_Uri.Address];
							bool flag18 = sip_Registration3 != null;
							if (flag18)
							{
								foreach (SIP_RegistrationBinding sip_RegistrationBinding2 in sip_Registration3.Bindings)
								{
									bool flag19 = sip_RegistrationBinding2.TTL > 1;
									if (flag19)
									{
										sip_Response2.Header.Add("Contact:", sip_RegistrationBinding2.ToContactValue());
									}
								}
							}
							sip_Response2.AuthenticationInfo.Add("qop=\"auth\",nextnonce=\"" + this.m_pStack.DigestNonceManager.CreateNonce() + "\"");
							serverTransaction.SendResponse(sip_Response2);
						}
					}
				}
			}
			else
			{
				serverTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x400_Bad_Request + ": To: value must be SIP or SIPS URI.", request));
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x00027FA4 File Offset: 0x00026FA4
		public SIP_Proxy Proxy
		{
			get
			{
				return this.m_pProxy;
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x00027FBC File Offset: 0x00026FBC
		public SIP_Registration[] Registrations
		{
			get
			{
				SIP_RegistrationCollection pRegistrations = this.m_pRegistrations;
				SIP_Registration[] result;
				lock (pRegistrations)
				{
					SIP_Registration[] array = new SIP_Registration[this.m_pRegistrations.Count];
					this.m_pRegistrations.Values.CopyTo(array, 0);
					result = array;
				}
				return result;
			}
		}

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x060006BA RID: 1722 RVA: 0x00028024 File Offset: 0x00027024
		// (remove) Token: 0x060006BB RID: 1723 RVA: 0x0002805C File Offset: 0x0002705C
		
		public event SIP_CanRegisterEventHandler CanRegister = null;

		// Token: 0x060006BC RID: 1724 RVA: 0x00028094 File Offset: 0x00027094
		internal bool OnCanRegister(string userName, string address)
		{
			bool flag = this.CanRegister != null;
			return flag && this.CanRegister(userName, address);
		}

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x060006BD RID: 1725 RVA: 0x000280C8 File Offset: 0x000270C8
		// (remove) Token: 0x060006BE RID: 1726 RVA: 0x00028100 File Offset: 0x00027100
		
		public event EventHandler<SIP_RegistrationEventArgs> AorRegistered = null;

		// Token: 0x060006BF RID: 1727 RVA: 0x00028138 File Offset: 0x00027138
		private void OnAorRegistered(SIP_Registration registration)
		{
			bool flag = this.AorRegistered != null;
			if (flag)
			{
				this.AorRegistered(this, new SIP_RegistrationEventArgs(registration));
			}
		}

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x060006C0 RID: 1728 RVA: 0x00028168 File Offset: 0x00027168
		// (remove) Token: 0x060006C1 RID: 1729 RVA: 0x000281A0 File Offset: 0x000271A0
		
		public event EventHandler<SIP_RegistrationEventArgs> AorUnregistered = null;

		// Token: 0x060006C2 RID: 1730 RVA: 0x000281D8 File Offset: 0x000271D8
		private void OnAorUnregistered(SIP_Registration registration)
		{
			bool flag = this.AorUnregistered != null;
			if (flag)
			{
				this.AorUnregistered(this, new SIP_RegistrationEventArgs(registration));
			}
		}

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x060006C3 RID: 1731 RVA: 0x00028208 File Offset: 0x00027208
		// (remove) Token: 0x060006C4 RID: 1732 RVA: 0x00028240 File Offset: 0x00027240
		
		public event EventHandler<SIP_RegistrationEventArgs> AorUpdated = null;

		// Token: 0x060006C5 RID: 1733 RVA: 0x00028278 File Offset: 0x00027278
		private void OnAorUpdated(SIP_Registration registration)
		{
			bool flag = this.AorUpdated != null;
			if (flag)
			{
				this.AorUpdated(this, new SIP_RegistrationEventArgs(registration));
			}
		}

		// Token: 0x040002C8 RID: 712
		private bool m_IsDisposed = false;

		// Token: 0x040002C9 RID: 713
		private SIP_Proxy m_pProxy = null;

		// Token: 0x040002CA RID: 714
		private SIP_Stack m_pStack = null;

		// Token: 0x040002CB RID: 715
		private SIP_RegistrationCollection m_pRegistrations = null;

		// Token: 0x040002CC RID: 716
		private Timer m_pTimer = null;
	}
}
