using System;
using System.Diagnostics;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000CE RID: 206
	public class RTP_SendStream
	{
		// Token: 0x060007EF RID: 2031 RVA: 0x0002F914 File Offset: 0x0002E914
		internal RTP_SendStream(RTP_Source_Local source)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			this.m_pSource = source;
			this.m_SeqNo = new Random().Next(1, 32000);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0002F9B0 File Offset: 0x0002E9B0
		private void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.m_pSource = null;
				this.OnDisposed();
				this.Disposed = null;
				this.Closed = null;
			}
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0002F9EE File Offset: 0x0002E9EE
		public void Close()
		{
			this.Close(null);
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0002F9FC File Offset: 0x0002E9FC
		public void Close(string closeReason)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			this.m_pSource.Close(closeReason);
			this.OnClosed();
			this.Dispose();
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0002FA44 File Offset: 0x0002EA44
		public void Send(RTP_Packet packet)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = packet == null;
			if (flag)
			{
				throw new ArgumentNullException("packet");
			}
			bool flag2 = this.Session.StreamMode == RTP_StreamMode.Inactive || this.Session.StreamMode == RTP_StreamMode.Receive;
			if (!flag2)
			{
				packet.SSRC = this.Source.SSRC;
				packet.SeqNo = this.NextSeqNo();
				packet.PayloadType = this.Session.Payload;
				this.m_RtpBytesSent += (long)this.m_pSource.SendRtpPacket(packet);
				this.m_RtpPacketsSent += 1L;
				this.m_RtpDataBytesSent += (long)packet.Data.Length;
				this.m_LastPacketTime = DateTime.Now;
				this.m_LastPacketRtpTimestamp = packet.Timestamp;
				this.m_RtcpCyclesSinceWeSent = 0;
			}
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0002FB39 File Offset: 0x0002EB39
		internal void RtcpCycle()
		{
			this.m_RtcpCyclesSinceWeSent++;
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0002FB4C File Offset: 0x0002EB4C
		private ushort NextSeqNo()
		{
			bool flag = this.m_SeqNo >= 65535;
			if (flag)
			{
				this.m_SeqNo = 0;
				this.m_SeqNoWrapCount++;
			}
			int seqNo = this.m_SeqNo;
			this.m_SeqNo = seqNo + 1;
			return (ushort)seqNo;
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x060007F6 RID: 2038 RVA: 0x0002FB9C File Offset: 0x0002EB9C
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x060007F7 RID: 2039 RVA: 0x0002FBB4 File Offset: 0x0002EBB4
		public RTP_Session Session
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSource.Session;
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x060007F8 RID: 2040 RVA: 0x0002FBF0 File Offset: 0x0002EBF0
		public RTP_Source Source
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSource;
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x060007F9 RID: 2041 RVA: 0x0002FC24 File Offset: 0x0002EC24
		public int SeqNoWrapCount
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SeqNoWrapCount;
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x060007FA RID: 2042 RVA: 0x0002FC58 File Offset: 0x0002EC58
		public int SeqNo
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SeqNo;
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x060007FB RID: 2043 RVA: 0x0002FC8C File Offset: 0x0002EC8C
		public DateTime LastPacketTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LastPacketTime;
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x060007FC RID: 2044 RVA: 0x0002FCC0 File Offset: 0x0002ECC0
		public uint LastPacketRtpTimestamp
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LastPacketRtpTimestamp;
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x0002FCF4 File Offset: 0x0002ECF4
		public long RtpPacketsSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtpPacketsSent;
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x060007FE RID: 2046 RVA: 0x0002FD28 File Offset: 0x0002ED28
		public long RtpBytesSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtpBytesSent;
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x0002FD5C File Offset: 0x0002ED5C
		public long RtpDataBytesSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtpDataBytesSent;
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06000800 RID: 2048 RVA: 0x0002FD90 File Offset: 0x0002ED90
		internal int RtcpCyclesSinceWeSent
		{
			get
			{
				return this.m_RtcpCyclesSinceWeSent;
			}
		}

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x06000801 RID: 2049 RVA: 0x0002FDA8 File Offset: 0x0002EDA8
		// (remove) Token: 0x06000802 RID: 2050 RVA: 0x0002FDE0 File Offset: 0x0002EDE0
		
		public event EventHandler Disposed = null;

		// Token: 0x06000803 RID: 2051 RVA: 0x0002FE18 File Offset: 0x0002EE18
		private void OnDisposed()
		{
			bool flag = this.Disposed != null;
			if (flag)
			{
				this.Disposed(this, new EventArgs());
			}
		}

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06000804 RID: 2052 RVA: 0x0002FE48 File Offset: 0x0002EE48
		// (remove) Token: 0x06000805 RID: 2053 RVA: 0x0002FE80 File Offset: 0x0002EE80
		
		public event EventHandler Closed = null;

		// Token: 0x06000806 RID: 2054 RVA: 0x0002FEB8 File Offset: 0x0002EEB8
		private void OnClosed()
		{
			bool flag = this.Closed != null;
			if (flag)
			{
				this.Closed(this, new EventArgs());
			}
		}

		// Token: 0x0400037B RID: 891
		private bool m_IsDisposed = false;

		// Token: 0x0400037C RID: 892
		private RTP_Source_Local m_pSource = null;

		// Token: 0x0400037D RID: 893
		private int m_SeqNoWrapCount = 0;

		// Token: 0x0400037E RID: 894
		private int m_SeqNo = 0;

		// Token: 0x0400037F RID: 895
		private DateTime m_LastPacketTime;

		// Token: 0x04000380 RID: 896
		private uint m_LastPacketRtpTimestamp = 0U;

		// Token: 0x04000381 RID: 897
		private long m_RtpPacketsSent = 0L;

		// Token: 0x04000382 RID: 898
		private long m_RtpBytesSent = 0L;

		// Token: 0x04000383 RID: 899
		private long m_RtpDataBytesSent = 0L;

		// Token: 0x04000384 RID: 900
		private int m_RtcpCyclesSinceWeSent = 9999;
	}
}
