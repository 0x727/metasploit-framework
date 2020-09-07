using System;
using System.Diagnostics;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000CC RID: 204
	public class RTP_ReceiveStream
	{
		// Token: 0x060007CB RID: 1995 RVA: 0x0002EDD0 File Offset: 0x0002DDD0
		internal RTP_ReceiveStream(RTP_Session session, RTP_Source ssrc, ushort packetSeqNo)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			bool flag2 = ssrc == null;
			if (flag2)
			{
				throw new ArgumentNullException("ssrc");
			}
			this.m_pSession = session;
			this.m_pSSRC = ssrc;
			this.InitSeq(packetSeqNo);
			this.m_MaxSeqNo = packetSeqNo - 1;
			this.m_Probation = this.MIN_SEQUENTIAL;
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x0002EF0C File Offset: 0x0002DF0C
		internal void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.m_pSession = null;
				this.m_pParticipant = null;
				this.Closed = null;
				this.Timeout = null;
				this.SenderReport = null;
				this.PacketReceived = null;
			}
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x0002EF58 File Offset: 0x0002DF58
		internal void Process(RTP_Packet packet, int size)
		{
			bool flag = packet == null;
			if (flag)
			{
				throw new ArgumentNullException("packet");
			}
			this.m_BytesReceived += (long)size;
			bool flag2 = this.UpdateSeq(packet.SeqNo);
			if (flag2)
			{
				this.OnPacketReceived(packet);
				uint num = RTP_Utils.DateTimeToNTP32(DateTime.Now);
				int num2 = (int)(num - packet.Timestamp);
				int num3 = num2 - this.m_Transit;
				this.m_Transit = num2;
				bool flag3 = num3 < 0;
				if (flag3)
				{
					num3 = -num3;
				}
				this.m_Jitter += 0.0625 * ((double)num3 - this.m_Jitter);
			}
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0002EFFB File Offset: 0x0002DFFB
		private void InitSeq(ushort seqNo)
		{
			this.m_BaseSeq = (uint)seqNo;
			this.m_MaxSeqNo = seqNo;
			this.m_LastBadSeqPlus1 = this.RTP_SEQ_MOD + 1U;
			this.m_SeqNoWrapCount = 0;
			this.m_PacketsReceived = 0L;
			this.m_ReceivedPrior = 0L;
			this.m_ExpectedPrior = 0L;
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0002F03C File Offset: 0x0002E03C
		private bool UpdateSeq(ushort seqNo)
		{
			ushort num = seqNo - this.m_MaxSeqNo;
			bool flag = this.m_Probation > 0;
			bool result;
			if (flag)
			{
				bool flag2 = seqNo == this.m_MaxSeqNo + 1;
				if (flag2)
				{
					this.m_Probation--;
					this.m_MaxSeqNo = seqNo;
					bool flag3 = this.m_Probation == 0;
					if (flag3)
					{
						this.InitSeq(seqNo);
						this.m_PacketsReceived += 1L;
						this.m_pSession.OnNewReceiveStream(this);
						return true;
					}
				}
				else
				{
					this.m_Probation = this.MIN_SEQUENTIAL - 1;
					this.m_MaxSeqNo = seqNo;
				}
				result = false;
			}
			else
			{
				bool flag4 = (int)num < this.MAX_DROPOUT;
				if (flag4)
				{
					bool flag5 = seqNo < this.m_MaxSeqNo;
					if (flag5)
					{
						this.m_SeqNoWrapCount++;
					}
					this.m_MaxSeqNo = seqNo;
				}
				else
				{
					bool flag6 = (ulong)num <= (ulong)this.RTP_SEQ_MOD - (ulong)((long)this.MAX_MISORDER);
					if (flag6)
					{
						bool flag7 = (uint)seqNo == this.m_LastBadSeqPlus1;
						if (!flag7)
						{
							this.m_LastBadSeqPlus1 = (uint)((long)(seqNo + 1) & (long)((ulong)(this.RTP_SEQ_MOD - 1U)));
							return false;
						}
						this.InitSeq(seqNo);
					}
					else
					{
						this.m_PacketsMisorder += 1L;
					}
				}
				this.m_PacketsReceived += 1L;
				result = true;
			}
			return result;
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x0002F19C File Offset: 0x0002E19C
		internal void SetSR(RTCP_Report_Sender report)
		{
			bool flag = report == null;
			if (flag)
			{
				throw new ArgumentNullException("report");
			}
			this.m_LastSRTime = DateTime.Now;
			this.m_pLastSR = report;
			this.OnSenderReport();
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x0002F1D8 File Offset: 0x0002E1D8
		internal RTCP_Packet_ReportBlock CreateReceiverReport()
		{
			uint num = (uint)((uint)this.m_SeqNoWrapCount << (int)(16 + this.m_MaxSeqNo));
			uint num2 = num - this.m_BaseSeq + 1U;
			int num3 = (int)((ulong)num2 - (ulong)this.m_ExpectedPrior);
			this.m_ExpectedPrior = (long)((ulong)num2);
			int num4 = (int)(this.m_PacketsReceived - this.m_ReceivedPrior);
			this.m_ReceivedPrior = this.m_PacketsReceived;
			int num5 = num3 - num4;
			bool flag = num3 == 0 || num5 <= 0;
			int fractionLost;
			if (flag)
			{
				fractionLost = 0;
			}
			else
			{
				fractionLost = (num5 << 8) / num3;
			}
			return new RTCP_Packet_ReportBlock(this.SSRC.SSRC)
			{
				FractionLost = (uint)fractionLost,
				CumulativePacketsLost = (uint)this.PacketsLost,
				ExtendedHighestSeqNo = num,
				Jitter = (uint)this.m_Jitter,
				LastSR = ((this.m_pLastSR == null) ? 0U : ((uint)(this.m_pLastSR.NtpTimestamp >> 8) & 65535U)),
				DelaySinceLastSR = (uint)Math.Max(0.0, (double)this.DelaySinceLastSR / 65.536)
			};
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x060007D2 RID: 2002 RVA: 0x0002F2FC File Offset: 0x0002E2FC
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x060007D3 RID: 2003 RVA: 0x0002F314 File Offset: 0x0002E314
		public RTP_Session Session
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSession;
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x060007D4 RID: 2004 RVA: 0x0002F348 File Offset: 0x0002E348
		public RTP_Source SSRC
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSSRC;
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x060007D5 RID: 2005 RVA: 0x0002F37C File Offset: 0x0002E37C
		public RTP_Participant_Remote Participant
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pParticipant;
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060007D6 RID: 2006 RVA: 0x0002F3B0 File Offset: 0x0002E3B0
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

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x0002F3E4 File Offset: 0x0002E3E4
		public int FirstSeqNo
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (int)this.m_BaseSeq;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x060007D8 RID: 2008 RVA: 0x0002F418 File Offset: 0x0002E418
		public int MaxSeqNo
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (int)this.m_MaxSeqNo;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x0002F44C File Offset: 0x0002E44C
		public long PacketsReceived
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_PacketsReceived;
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x060007DA RID: 2010 RVA: 0x0002F480 File Offset: 0x0002E480
		public long PacketsMisorder
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_PacketsMisorder;
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x0002F4B4 File Offset: 0x0002E4B4
		public long PacketsLost
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				uint num = (uint)(65536 * this.m_SeqNoWrapCount + (int)this.m_MaxSeqNo);
				uint num2 = num - this.m_BaseSeq + 1U;
				return (long)((ulong)num2 - (ulong)this.m_PacketsReceived);
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x060007DC RID: 2012 RVA: 0x0002F510 File Offset: 0x0002E510
		public long BytesReceived
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_BytesReceived;
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x060007DD RID: 2013 RVA: 0x0002F544 File Offset: 0x0002E544
		public double Jitter
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Jitter;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x060007DE RID: 2014 RVA: 0x0002F578 File Offset: 0x0002E578
		public int DelaySinceLastSR
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (int)((this.m_LastSRTime == DateTime.MinValue) ? -1.0 : (DateTime.Now - this.m_LastSRTime).TotalMilliseconds);
			}
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x060007DF RID: 2015 RVA: 0x0002F5DC File Offset: 0x0002E5DC
		public DateTime LastSRTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LastSRTime;
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x060007E0 RID: 2016 RVA: 0x0002F610 File Offset: 0x0002E610
		public RTCP_Report_Sender LastSR
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pLastSR;
			}
		}

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x060007E1 RID: 2017 RVA: 0x0002F644 File Offset: 0x0002E644
		// (remove) Token: 0x060007E2 RID: 2018 RVA: 0x0002F67C File Offset: 0x0002E67C
		
		public event EventHandler Closed = null;

		// Token: 0x060007E3 RID: 2019 RVA: 0x0002F6B4 File Offset: 0x0002E6B4
		private void OnClosed()
		{
			bool flag = this.Closed != null;
			if (flag)
			{
				this.Closed(this, new EventArgs());
			}
		}

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x060007E4 RID: 2020 RVA: 0x0002F6E4 File Offset: 0x0002E6E4
		// (remove) Token: 0x060007E5 RID: 2021 RVA: 0x0002F71C File Offset: 0x0002E71C
		
		public event EventHandler Timeout = null;

		// Token: 0x060007E6 RID: 2022 RVA: 0x0002F754 File Offset: 0x0002E754
		internal void OnTimeout()
		{
			bool flag = this.Timeout != null;
			if (flag)
			{
				this.Timeout(this, new EventArgs());
			}
		}

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x060007E7 RID: 2023 RVA: 0x0002F784 File Offset: 0x0002E784
		// (remove) Token: 0x060007E8 RID: 2024 RVA: 0x0002F7BC File Offset: 0x0002E7BC
		
		public event EventHandler SenderReport = null;

		// Token: 0x060007E9 RID: 2025 RVA: 0x0002F7F4 File Offset: 0x0002E7F4
		private void OnSenderReport()
		{
			bool flag = this.SenderReport != null;
			if (flag)
			{
				this.SenderReport(this, new EventArgs());
			}
		}

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x060007EA RID: 2026 RVA: 0x0002F824 File Offset: 0x0002E824
		// (remove) Token: 0x060007EB RID: 2027 RVA: 0x0002F85C File Offset: 0x0002E85C
		
		public event EventHandler<RTP_PacketEventArgs> PacketReceived = null;

		// Token: 0x060007EC RID: 2028 RVA: 0x0002F894 File Offset: 0x0002E894
		private void OnPacketReceived(RTP_Packet packet)
		{
			bool flag = this.PacketReceived != null;
			if (flag)
			{
				this.PacketReceived(this, new RTP_PacketEventArgs(packet));
			}
		}

		// Token: 0x04000360 RID: 864
		private bool m_IsDisposed = false;

		// Token: 0x04000361 RID: 865
		private RTP_Session m_pSession = null;

		// Token: 0x04000362 RID: 866
		private RTP_Source m_pSSRC = null;

		// Token: 0x04000363 RID: 867
		private RTP_Participant_Remote m_pParticipant = null;

		// Token: 0x04000364 RID: 868
		private int m_SeqNoWrapCount = 0;

		// Token: 0x04000365 RID: 869
		private ushort m_MaxSeqNo = 0;

		// Token: 0x04000366 RID: 870
		private long m_PacketsReceived = 0L;

		// Token: 0x04000367 RID: 871
		private long m_PacketsMisorder = 0L;

		// Token: 0x04000368 RID: 872
		private long m_BytesReceived = 0L;

		// Token: 0x04000369 RID: 873
		private double m_Jitter = 0.0;

		// Token: 0x0400036A RID: 874
		private RTCP_Report_Sender m_pLastSR = null;

		// Token: 0x0400036B RID: 875
		private uint m_BaseSeq = 0U;

		// Token: 0x0400036C RID: 876
		private long m_ReceivedPrior = 0L;

		// Token: 0x0400036D RID: 877
		private long m_ExpectedPrior = 0L;

		// Token: 0x0400036E RID: 878
		private int m_Transit = 0;

		// Token: 0x0400036F RID: 879
		private uint m_LastBadSeqPlus1 = 0U;

		// Token: 0x04000370 RID: 880
		private int m_Probation = 0;

		// Token: 0x04000371 RID: 881
		private DateTime m_LastSRTime = DateTime.MinValue;

		// Token: 0x04000372 RID: 882
		private int MAX_DROPOUT = 3000;

		// Token: 0x04000373 RID: 883
		private int MAX_MISORDER = 100;

		// Token: 0x04000374 RID: 884
		private int MIN_SEQUENTIAL = 2;

		// Token: 0x04000375 RID: 885
		private uint RTP_SEQ_MOD = 65536U;
	}
}
