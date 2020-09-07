using System;
using System.Diagnostics;
using System.Net;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000D0 RID: 208
	public abstract class RTP_Source
	{
		// Token: 0x06000809 RID: 2057 RVA: 0x0002FF38 File Offset: 0x0002EF38
		internal RTP_Source(RTP_Session session, uint ssrc)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			this.m_pSession = session;
			this.m_SSRC = ssrc;
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0002FFE4 File Offset: 0x0002EFE4
		internal virtual void Dispose()
		{
			bool flag = this.m_State == RTP_SourceState.Disposed;
			if (!flag)
			{
				this.OnDisposing();
				this.SetState(RTP_SourceState.Disposed);
				this.m_pSession = null;
				this.m_pRtcpEP = null;
				this.m_pRtpEP = null;
				this.Closed = null;
				this.Disposing = null;
				this.StateChanged = null;
			}
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0003003B File Offset: 0x0002F03B
		internal virtual void Close(string closeReason)
		{
			this.m_CloseReason = closeReason;
			this.OnClosed();
			this.Dispose();
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x00030053 File Offset: 0x0002F053
		internal void SetRtcpEP(IPEndPoint ep)
		{
			this.m_pRtcpEP = ep;
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0003005D File Offset: 0x0002F05D
		internal void SetRtpEP(IPEndPoint ep)
		{
			this.m_pRtpEP = ep;
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x00030068 File Offset: 0x0002F068
		internal void SetActivePassive(bool active)
		{
			if (active)
			{
			}
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x00030081 File Offset: 0x0002F081
		internal void SetLastRtcpPacket(DateTime time)
		{
			this.m_LastRtcpPacket = time;
			this.m_LastActivity = time;
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x00030092 File Offset: 0x0002F092
		internal void SetLastRtpPacket(DateTime time)
		{
			this.m_LastRtpPacket = time;
			this.m_LastActivity = time;
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x000300A4 File Offset: 0x0002F0A4
		internal void SetRR(RTCP_Packet_ReportBlock rr)
		{
			bool flag = rr == null;
			if (flag)
			{
				throw new ArgumentNullException("rr");
			}
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x000300C6 File Offset: 0x0002F0C6
		internal void GenerateNewSSRC()
		{
			this.m_SSRC = RTP_Utils.GenerateSSRC();
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x000300D4 File Offset: 0x0002F0D4
		protected void SetState(RTP_SourceState state)
		{
			bool flag = this.m_State == RTP_SourceState.Disposed;
			if (!flag)
			{
				bool flag2 = this.m_State != state;
				if (flag2)
				{
					this.m_State = state;
					this.OnStateChaged();
				}
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06000814 RID: 2068 RVA: 0x00030114 File Offset: 0x0002F114
		public RTP_SourceState State
		{
			get
			{
				return this.m_State;
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000815 RID: 2069 RVA: 0x0003012C File Offset: 0x0002F12C
		public RTP_Session Session
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSession;
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000816 RID: 2070 RVA: 0x00030164 File Offset: 0x0002F164
		public uint SSRC
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SSRC;
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06000817 RID: 2071 RVA: 0x0003019C File Offset: 0x0002F19C
		public IPEndPoint RtcpEP
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRtcpEP;
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06000818 RID: 2072 RVA: 0x000301D4 File Offset: 0x0002F1D4
		public IPEndPoint RtpEP
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRtpEP;
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06000819 RID: 2073
		public abstract bool IsLocal { get; }

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x0003020C File Offset: 0x0002F20C
		public DateTime LastActivity
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LastActivity;
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x0600081B RID: 2075 RVA: 0x00030244 File Offset: 0x0002F244
		public DateTime LastRtcpPacket
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LastRtcpPacket;
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x0600081C RID: 2076 RVA: 0x0003027C File Offset: 0x0002F27C
		public DateTime LastRtpPacket
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LastRtpPacket;
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x0600081D RID: 2077 RVA: 0x000302B4 File Offset: 0x0002F2B4
		public DateTime LastRRTime
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LastRRTime;
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x0600081E RID: 2078 RVA: 0x000302EC File Offset: 0x0002F2EC
		public string CloseReason
		{
			get
			{
				bool flag = this.m_State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_CloseReason;
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x0600081F RID: 2079 RVA: 0x00030324 File Offset: 0x0002F324
		// (set) Token: 0x06000820 RID: 2080 RVA: 0x0003033C File Offset: 0x0002F33C
		public object Tag
		{
			get
			{
				return this.m_pTag;
			}
			set
			{
				this.m_pTag = value;
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06000821 RID: 2081
		internal abstract string CName { get; }

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06000822 RID: 2082 RVA: 0x00030348 File Offset: 0x0002F348
		// (remove) Token: 0x06000823 RID: 2083 RVA: 0x00030380 File Offset: 0x0002F380
		
		public event EventHandler Closed = null;

		// Token: 0x06000824 RID: 2084 RVA: 0x000303B8 File Offset: 0x0002F3B8
		private void OnClosed()
		{
			bool flag = this.Closed != null;
			if (flag)
			{
				this.Closed(this, new EventArgs());
			}
		}

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06000825 RID: 2085 RVA: 0x000303E8 File Offset: 0x0002F3E8
		// (remove) Token: 0x06000826 RID: 2086 RVA: 0x00030420 File Offset: 0x0002F420
		
		public event EventHandler Disposing = null;

		// Token: 0x06000827 RID: 2087 RVA: 0x00030458 File Offset: 0x0002F458
		private void OnDisposing()
		{
			bool flag = this.Disposing != null;
			if (flag)
			{
				this.Disposing(this, new EventArgs());
			}
		}

		// Token: 0x1400003F RID: 63
		// (add) Token: 0x06000828 RID: 2088 RVA: 0x00030488 File Offset: 0x0002F488
		// (remove) Token: 0x06000829 RID: 2089 RVA: 0x000304C0 File Offset: 0x0002F4C0
		
		public event EventHandler StateChanged = null;

		// Token: 0x0600082A RID: 2090 RVA: 0x000304F8 File Offset: 0x0002F4F8
		private void OnStateChaged()
		{
			bool flag = this.StateChanged != null;
			if (flag)
			{
				this.StateChanged(this, new EventArgs());
			}
		}

		// Token: 0x04000388 RID: 904
		private RTP_SourceState m_State = RTP_SourceState.Passive;

		// Token: 0x04000389 RID: 905
		private RTP_Session m_pSession = null;

		// Token: 0x0400038A RID: 906
		private uint m_SSRC = 0U;

		// Token: 0x0400038B RID: 907
		private IPEndPoint m_pRtcpEP = null;

		// Token: 0x0400038C RID: 908
		private IPEndPoint m_pRtpEP = null;

		// Token: 0x0400038D RID: 909
		private DateTime m_LastRtcpPacket = DateTime.MinValue;

		// Token: 0x0400038E RID: 910
		private DateTime m_LastRtpPacket = DateTime.MinValue;

		// Token: 0x0400038F RID: 911
		private DateTime m_LastActivity = DateTime.Now;

		// Token: 0x04000390 RID: 912
		private DateTime m_LastRRTime = DateTime.MinValue;

		// Token: 0x04000391 RID: 913
		private string m_CloseReason = null;

		// Token: 0x04000392 RID: 914
		private object m_pTag = null;
	}
}
