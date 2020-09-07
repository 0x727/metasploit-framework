using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000CA RID: 202
	public class RTP_ParticipantEventArgs : EventArgs
	{
		// Token: 0x060007B9 RID: 1977 RVA: 0x0002E9EC File Offset: 0x0002D9EC
		public RTP_ParticipantEventArgs(RTP_Participant_Remote participant)
		{
			bool flag = participant == null;
			if (flag)
			{
				throw new ArgumentNullException("participant");
			}
			this.m_pParticipant = participant;
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x060007BA RID: 1978 RVA: 0x0002EA24 File Offset: 0x0002DA24
		public RTP_Participant_Remote Participant
		{
			get
			{
				return this.m_pParticipant;
			}
		}

		// Token: 0x04000359 RID: 857
		private RTP_Participant_Remote m_pParticipant = null;
	}
}
