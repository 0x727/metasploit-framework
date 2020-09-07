using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;

namespace LumiSoft.Net.DNS.Client
{
	// Token: 0x02000258 RID: 600
	public class DNS_ClientTransaction
	{
		// Token: 0x0600157D RID: 5501 RVA: 0x000856A0 File Offset: 0x000846A0
		internal DNS_ClientTransaction(Dns_Client owner, IPAddress[] dnsServers, int id, DNS_QType qtype, string qname, int timeout)
		{
			bool flag = owner == null;
			if (flag)
			{
				throw new ArgumentNullException("owner");
			}
			bool flag2 = dnsServers == null;
			if (flag2)
			{
				throw new ArgumentNullException("dnsServers");
			}
			bool flag3 = qname == null;
			if (flag3)
			{
				throw new ArgumentNullException("qname");
			}
			this.m_pOwner = owner;
			this.m_pDnsServers = dnsServers;
			this.m_ID = id;
			this.m_QName = qname;
			this.m_QType = qtype;
			this.m_CreateTime = DateTime.Now;
			this.m_pTimeoutTimer = new TimerEx((double)timeout);
			this.m_pTimeoutTimer.Elapsed += this.m_pTimeoutTimer_Elapsed;
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x000857A4 File Offset: 0x000847A4
		public void Dispose()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.State == DNS_ClientTransactionState.Disposed;
				if (!flag2)
				{
					this.SetState(DNS_ClientTransactionState.Disposed);
					bool flag3 = this.m_pTimeoutTimer != null;
					if (flag3)
					{
						this.m_pTimeoutTimer.Dispose();
						this.m_pTimeoutTimer = null;
					}
					this.m_pOwner = null;
					this.m_pResponse = null;
					this.StateChanged = null;
					this.Timeout = null;
				}
			}
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x0008583C File Offset: 0x0008483C
		private void m_pTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				this.OnTimeout();
			}
			catch
			{
			}
			finally
			{
				this.Dispose();
			}
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x00085884 File Offset: 0x00084884
		public void Start()
		{
			bool flag = this.State > DNS_ClientTransactionState.WaitingForStart;
			if (flag)
			{
				throw new InvalidOperationException("DNS_ClientTransaction.Start may be called only in 'WaitingForStart' transaction state.");
			}
			this.SetState(DNS_ClientTransactionState.Active);
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				try
				{
					bool useDnsCache = Dns_Client.UseDnsCache;
					if (useDnsCache)
					{
						DnsServerResponse fromCache = this.m_pOwner.Cache.GetFromCache(this.m_QName, (int)this.m_QType);
						bool flag2 = fromCache != null;
						if (flag2)
						{
							this.m_pResponse = fromCache;
							this.SetState(DNS_ClientTransactionState.Completed);
							this.Dispose();
							return;
						}
					}
					byte[] array = new byte[1400];
					int count = this.CreateQuery(array, this.m_ID, this.m_QName, this.m_QType, 1);
					foreach (IPAddress target in this.m_pDnsServers)
					{
						this.m_pOwner.Send(target, array, count);
					}
					this.m_pTimeoutTimer.Start();
				}
				catch
				{
					try
					{
						IdnMapping idnMapping = new IdnMapping();
						idnMapping.GetAscii(this.m_QName);
					}
					catch
					{
						this.m_pResponse = new DnsServerResponse(true, this.m_ID, DNS_RCode.NAME_ERROR, new List<DNS_rr>(), new List<DNS_rr>(), new List<DNS_rr>());
					}
					this.SetState(DNS_ClientTransactionState.Completed);
				}
			});
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x000858C8 File Offset: 0x000848C8
		internal void ProcessResponse(DnsServerResponse response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			try
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool flag3 = this.State != DNS_ClientTransactionState.Active;
					if (!flag3)
					{
						this.m_ResponseCount++;
						bool flag4 = this.m_pResponse != null;
						if (!flag4)
						{
							bool flag5 = response.ResponseCode == DNS_RCode.REFUSED && this.m_ResponseCount < Dns_Client.DnsServers.Length;
							if (!flag5)
							{
								this.m_pResponse = response;
								this.SetState(DNS_ClientTransactionState.Completed);
							}
						}
					}
				}
			}
			finally
			{
				bool flag6 = this.State == DNS_ClientTransactionState.Completed;
				if (flag6)
				{
					this.Dispose();
				}
			}
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x000859B0 File Offset: 0x000849B0
		private void SetState(DNS_ClientTransactionState state)
		{
			bool flag = this.State == DNS_ClientTransactionState.Disposed;
			if (!flag)
			{
				this.m_State = state;
				this.OnStateChanged();
			}
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x000859DC File Offset: 0x000849DC
		private int CreateQuery(byte[] buffer, int ID, string qname, DNS_QType qtype, int qclass)
		{
			buffer[0] = (byte)(ID >> 8);
			buffer[1] = (byte)(ID & 255);
			buffer[2] = 1;
			buffer[3] = 0;
			buffer[4] = 0;
			buffer[5] = 1;
			buffer[6] = 0;
			buffer[7] = 0;
			buffer[8] = 0;
			buffer[9] = 0;
			buffer[10] = 0;
			buffer[11] = 0;
			IdnMapping idnMapping = new IdnMapping();
			qname = idnMapping.GetAscii(qname);
			string[] array = qname.Split(new char[]
			{
				'.'
			});
			int num = 12;
			foreach (string s in array)
			{
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				buffer[num++] = (byte)bytes.Length;
				bytes.CopyTo(buffer, num);
				num += bytes.Length;
			}
			buffer[num++] = 0;
			buffer[num++] = 0;
			buffer[num++] = (byte)qtype;
			buffer[num++] = 0;
			buffer[num++] = (byte)qclass;
			return num;
		}

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06001584 RID: 5508 RVA: 0x00085AC8 File Offset: 0x00084AC8
		public DNS_ClientTransactionState State
		{
			get
			{
				return this.m_State;
			}
		}

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06001585 RID: 5509 RVA: 0x00085AE0 File Offset: 0x00084AE0
		public DateTime CreateTime
		{
			get
			{
				return this.m_CreateTime;
			}
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06001586 RID: 5510 RVA: 0x00085AF8 File Offset: 0x00084AF8
		public int ID
		{
			get
			{
				return this.m_ID;
			}
		}

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x06001587 RID: 5511 RVA: 0x00085B10 File Offset: 0x00084B10
		public string QName
		{
			get
			{
				return this.m_QName;
			}
		}

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x06001588 RID: 5512 RVA: 0x00085B28 File Offset: 0x00084B28
		public DNS_QType QType
		{
			get
			{
				return this.m_QType;
			}
		}

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x06001589 RID: 5513 RVA: 0x00085B40 File Offset: 0x00084B40
		public DnsServerResponse Response
		{
			get
			{
				return this.m_pResponse;
			}
		}

		// Token: 0x14000098 RID: 152
		// (add) Token: 0x0600158A RID: 5514 RVA: 0x00085B58 File Offset: 0x00084B58
		// (remove) Token: 0x0600158B RID: 5515 RVA: 0x00085B90 File Offset: 0x00084B90
		
		public event EventHandler<EventArgs<DNS_ClientTransaction>> StateChanged = null;

		// Token: 0x0600158C RID: 5516 RVA: 0x00085BC8 File Offset: 0x00084BC8
		private void OnStateChanged()
		{
			bool flag = this.StateChanged != null;
			if (flag)
			{
				this.StateChanged(this, new EventArgs<DNS_ClientTransaction>(this));
			}
		}

		// Token: 0x14000099 RID: 153
		// (add) Token: 0x0600158D RID: 5517 RVA: 0x00085BF8 File Offset: 0x00084BF8
		// (remove) Token: 0x0600158E RID: 5518 RVA: 0x00085C30 File Offset: 0x00084C30
		
		public event EventHandler Timeout = null;

		// Token: 0x0600158F RID: 5519 RVA: 0x00085C68 File Offset: 0x00084C68
		private void OnTimeout()
		{
			bool flag = this.Timeout != null;
			if (flag)
			{
				this.Timeout(this, new EventArgs());
			}
		}

		// Token: 0x0400087F RID: 2175
		private object m_pLock = new object();

		// Token: 0x04000880 RID: 2176
		private DNS_ClientTransactionState m_State = DNS_ClientTransactionState.WaitingForStart;

		// Token: 0x04000881 RID: 2177
		private DateTime m_CreateTime;

		// Token: 0x04000882 RID: 2178
		private Dns_Client m_pOwner = null;

		// Token: 0x04000883 RID: 2179
		private IPAddress[] m_pDnsServers = null;

		// Token: 0x04000884 RID: 2180
		private int m_ID = 1;

		// Token: 0x04000885 RID: 2181
		private string m_QName = "";

		// Token: 0x04000886 RID: 2182
		private DNS_QType m_QType = (DNS_QType)0;

		// Token: 0x04000887 RID: 2183
		private TimerEx m_pTimeoutTimer = null;

		// Token: 0x04000888 RID: 2184
		private DnsServerResponse m_pResponse = null;

		// Token: 0x04000889 RID: 2185
		private int m_ResponseCount = 0;
	}
}
