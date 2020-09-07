using System;
using System.Net;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000E0 RID: 224
	public class RTP_Source_Local : RTP_Source
	{
		// Token: 0x060008FE RID: 2302 RVA: 0x000360D8 File Offset: 0x000350D8
		internal RTP_Source_Local(RTP_Session session, uint ssrc, IPEndPoint rtcpEP, IPEndPoint rtpEP) : base(session, ssrc)
		{
			bool flag = rtcpEP == null;
			if (flag)
			{
				throw new ArgumentNullException("rtcpEP");
			}
			bool flag2 = rtpEP == null;
			if (flag2)
			{
				throw new ArgumentNullException("rtpEP");
			}
			base.SetRtcpEP(rtcpEP);
			base.SetRtpEP(rtpEP);
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x00036130 File Offset: 0x00035130
		public void SendApplicationPacket(RTCP_Packet_APP packet)
		{
			bool flag = base.State == RTP_SourceState.Disposed;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag2 = packet == null;
			if (flag2)
			{
				throw new ArgumentNullException("packet");
			}
			packet.Source = base.SSRC;
			RTCP_CompoundPacket rtcp_CompoundPacket = new RTCP_CompoundPacket();
			RTCP_Packet_RR rtcp_Packet_RR = new RTCP_Packet_RR();
			rtcp_Packet_RR.SSRC = base.SSRC;
			rtcp_CompoundPacket.Packets.Add(packet);
			base.Session.SendRtcpPacket(rtcp_CompoundPacket);
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x000361B4 File Offset: 0x000351B4
		internal override void Close(string closeReason)
		{
			bool flag = base.State == RTP_SourceState.Disposed;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			RTCP_CompoundPacket rtcp_CompoundPacket = new RTCP_CompoundPacket();
			RTCP_Packet_RR rtcp_Packet_RR = new RTCP_Packet_RR();
			rtcp_Packet_RR.SSRC = base.SSRC;
			rtcp_CompoundPacket.Packets.Add(rtcp_Packet_RR);
			RTCP_Packet_BYE rtcp_Packet_BYE = new RTCP_Packet_BYE();
			rtcp_Packet_BYE.Sources = new uint[]
			{
				base.SSRC
			};
			bool flag2 = !string.IsNullOrEmpty(closeReason);
			if (flag2)
			{
				rtcp_Packet_BYE.LeavingReason = closeReason;
			}
			rtcp_CompoundPacket.Packets.Add(rtcp_Packet_BYE);
			base.Session.SendRtcpPacket(rtcp_CompoundPacket);
			base.Close(closeReason);
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00036260 File Offset: 0x00035260
		internal void CreateStream()
		{
			bool flag = this.m_pStream != null;
			if (flag)
			{
				throw new InvalidOperationException("Stream is already created.");
			}
			this.m_pStream = new RTP_SendStream(this);
			this.m_pStream.Disposed += delegate(object s, EventArgs e)
			{
				this.m_pStream = null;
				this.Dispose();
			};
			base.SetState(RTP_SourceState.Active);
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x000362B4 File Offset: 0x000352B4
		internal int SendRtpPacket(RTP_Packet packet)
		{
			bool flag = packet == null;
			if (flag)
			{
				throw new ArgumentNullException("packet");
			}
			bool flag2 = this.m_pStream == null;
			if (flag2)
			{
				throw new InvalidOperationException("RTP stream is not created by CreateStream method.");
			}
			base.SetLastRtpPacket(DateTime.Now);
			base.SetState(RTP_SourceState.Active);
			return base.Session.SendRtpPacket(this.m_pStream, packet);
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06000903 RID: 2307 RVA: 0x0003631C File Offset: 0x0003531C
		public override bool IsLocal
		{
			get
			{
				bool flag = base.State == RTP_SourceState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return true;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06000904 RID: 2308 RVA: 0x00036350 File Offset: 0x00035350
		public RTP_Participant_Local Participant
		{
			get
			{
				return base.Session.Session.LocalParticipant;
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06000905 RID: 2309 RVA: 0x00036374 File Offset: 0x00035374
		public RTP_SendStream Stream
		{
			get
			{
				return this.m_pStream;
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06000906 RID: 2310 RVA: 0x0003638C File Offset: 0x0003538C
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

		// Token: 0x04000410 RID: 1040
		private RTP_SendStream m_pStream = null;
	}
}
