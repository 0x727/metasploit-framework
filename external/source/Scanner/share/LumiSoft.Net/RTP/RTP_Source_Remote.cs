using System;
using System.Diagnostics;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000E1 RID: 225
	public class RTP_Source_Remote : RTP_Source
	{
		// Token: 0x06000908 RID: 2312 RVA: 0x000363CD File Offset: 0x000353CD
		internal RTP_Source_Remote(RTP_Session session, uint ssrc) : base(session, ssrc)
		{
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x000363F0 File Offset: 0x000353F0
		internal override void Dispose()
		{
			this.m_pParticipant = null;
			bool flag = this.m_pStream != null;
			if (flag)
			{
				this.m_pStream.Dispose();
			}
			this.ApplicationPacket = null;
			base.Dispose();
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00036430 File Offset: 0x00035430
		internal void SetParticipant(RTP_Participant_Remote participant)
		{
			bool flag = participant == null;
			if (flag)
			{
				throw new ArgumentNullException("participant");
			}
			this.m_pParticipant = participant;
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x0003645C File Offset: 0x0003545C
		internal void OnRtpPacketReceived(RTP_Packet packet, int size)
		{
			bool flag = packet == null;
			if (flag)
			{
				throw new ArgumentNullException("packet");
			}
			base.SetLastRtpPacket(DateTime.Now);
			bool flag2 = this.m_pStream == null;
			if (flag2)
			{
				this.m_pStream = new RTP_ReceiveStream(base.Session, this, packet.SeqNo);
				base.SetState(RTP_SourceState.Active);
			}
			this.m_pStream.Process(packet, size);
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x000364C8 File Offset: 0x000354C8
		internal void OnSenderReport(RTCP_Report_Sender report)
		{
			bool flag = report == null;
			if (flag)
			{
				throw new ArgumentNullException("report");
			}
			bool flag2 = this.m_pStream != null;
			if (flag2)
			{
				this.m_pStream.SetSR(report);
			}
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x00036508 File Offset: 0x00035508
		internal void OnAppPacket(RTCP_Packet_APP packet)
		{
			bool flag = packet == null;
			if (flag)
			{
				throw new ArgumentNullException("packet");
			}
			this.OnApplicationPacket(packet);
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x0600090E RID: 2318 RVA: 0x00036534 File Offset: 0x00035534
		public override bool IsLocal
		{
			get
			{
				bool flag = base.State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return false;
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x0600090F RID: 2319 RVA: 0x00036568 File Offset: 0x00035568
		public RTP_Participant_Remote Participant
		{
			get
			{
				bool flag = base.State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pParticipant;
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06000910 RID: 2320 RVA: 0x000365A0 File Offset: 0x000355A0
		public RTP_ReceiveStream Stream
		{
			get
			{
				bool flag = base.State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pStream;
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06000911 RID: 2321 RVA: 0x000365D8 File Offset: 0x000355D8
		internal override string CName
		{
			get
			{
				bool flag = this.Participant != null;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this.Participant.CNAME;
				}
				return result;
			}
		}

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x06000912 RID: 2322 RVA: 0x00036608 File Offset: 0x00035608
		// (remove) Token: 0x06000913 RID: 2323 RVA: 0x00036640 File Offset: 0x00035640
		
		public event EventHandler<EventArgs<RTCP_Packet_APP>> ApplicationPacket = null;

		// Token: 0x06000914 RID: 2324 RVA: 0x00036678 File Offset: 0x00035678
		private void OnApplicationPacket(RTCP_Packet_APP packet)
		{
			bool flag = packet == null;
			if (flag)
			{
				throw new ArgumentNullException("packet");
			}
			bool flag2 = this.ApplicationPacket != null;
			if (flag2)
			{
				this.ApplicationPacket(this, new EventArgs<RTCP_Packet_APP>(packet));
			}
		}

		// Token: 0x04000411 RID: 1041
		private RTP_Participant_Remote m_pParticipant = null;

		// Token: 0x04000412 RID: 1042
		private RTP_ReceiveStream m_pStream = null;
	}
}
