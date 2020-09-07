using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000C7 RID: 199
	public class RTP_MultimediaSession : IDisposable
	{
		// Token: 0x06000796 RID: 1942 RVA: 0x0002DF00 File Offset: 0x0002CF00
		public RTP_MultimediaSession(string cname)
		{
			bool flag = cname == null;
			if (flag)
			{
				throw new ArgumentNullException("cname");
			}
			bool flag2 = cname == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'cname' value must be specified.");
			}
			this.m_pLocalParticipant = new RTP_Participant_Local(cname);
			this.m_pSessions = new List<RTP_Session>();
			this.m_pParticipants = new Dictionary<string, RTP_Participant_Remote>();
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x0002DF98 File Offset: 0x0002CF98
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				foreach (RTP_Session rtp_Session in this.m_pSessions.ToArray())
				{
					rtp_Session.Dispose();
				}
				this.m_IsDisposed = true;
				this.m_pLocalParticipant = null;
				this.m_pSessions = null;
				this.m_pParticipants = null;
				this.NewParticipant = null;
				this.Error = null;
			}
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x0002E008 File Offset: 0x0002D008
		public void Close(string closeReason)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			foreach (RTP_Session rtp_Session in this.m_pSessions.ToArray())
			{
				rtp_Session.Close(closeReason);
			}
			this.Dispose();
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x0002E064 File Offset: 0x0002D064
		public void Start()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x0002E090 File Offset: 0x0002D090
		public void Stop()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x0002E0BC File Offset: 0x0002D0BC
		public RTP_Session CreateSession(RTP_Address localEP, RTP_Clock clock)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = localEP == null;
			if (flag)
			{
				throw new ArgumentNullException("localEP");
			}
			bool flag2 = clock == null;
			if (flag2)
			{
				throw new ArgumentNullException("clock");
			}
			RTP_Session rtp_Session = new RTP_Session(this, localEP, clock);
			rtp_Session.Disposed += delegate(object s, EventArgs e)
			{
				this.m_pSessions.Remove((RTP_Session)s);
			};
			this.m_pSessions.Add(rtp_Session);
			this.OnSessionCreated(rtp_Session);
			return rtp_Session;
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x0002E148 File Offset: 0x0002D148
		internal RTP_Participant_Remote GetOrCreateParticipant(string cname)
		{
			bool flag = cname == null;
			if (flag)
			{
				throw new ArgumentNullException("cname");
			}
			bool flag2 = cname == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'cname' value must be specified.");
			}
			Dictionary<string, RTP_Participant_Remote> pParticipants = this.m_pParticipants;
			bool flag3 = false;
			RTP_Participant_Remote participant2;
			try
			{
				Monitor.Enter(pParticipants, ref flag3);
				RTP_Participant_Remote participant = null;
				bool flag4 = !this.m_pParticipants.TryGetValue(cname, out participant);
				if (flag4)
				{
					participant = new RTP_Participant_Remote(cname);
					participant.Removed += delegate(object sender, EventArgs e)
					{
						this.m_pParticipants.Remove(participant.CNAME);
					};
					this.m_pParticipants.Add(cname, participant);
					this.OnNewParticipant(participant);
				}
				participant2 = participant;
			}
			finally
			{
				if (flag3)
				{
					Monitor.Exit(pParticipants);
				}
			}
			return participant2;
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x0002E240 File Offset: 0x0002D240
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x0600079E RID: 1950 RVA: 0x0002E258 File Offset: 0x0002D258
		public RTP_Session[] Sessions
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSessions.ToArray();
			}
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x0600079F RID: 1951 RVA: 0x0002E294 File Offset: 0x0002D294
		public RTP_Participant_Local LocalParticipant
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pLocalParticipant;
			}
		}

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x060007A0 RID: 1952 RVA: 0x0002E2C8 File Offset: 0x0002D2C8
		public RTP_Participant_Remote[] RemoteParticipants
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				Dictionary<string, RTP_Participant_Remote> pParticipants = this.m_pParticipants;
				RTP_Participant_Remote[] result;
				lock (pParticipants)
				{
					RTP_Participant_Remote[] array = new RTP_Participant_Remote[this.m_pParticipants.Count];
					this.m_pParticipants.Values.CopyTo(array, 0);
					result = array;
				}
				return result;
			}
		}

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x060007A1 RID: 1953 RVA: 0x0002E34C File Offset: 0x0002D34C
		// (remove) Token: 0x060007A2 RID: 1954 RVA: 0x0002E384 File Offset: 0x0002D384
		
		public event EventHandler<EventArgs<RTP_Session>> SessionCreated = null;

		// Token: 0x060007A3 RID: 1955 RVA: 0x0002E3BC File Offset: 0x0002D3BC
		private void OnSessionCreated(RTP_Session session)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			bool flag2 = this.SessionCreated != null;
			if (flag2)
			{
				this.SessionCreated(this, new EventArgs<RTP_Session>(session));
			}
		}

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x060007A4 RID: 1956 RVA: 0x0002E400 File Offset: 0x0002D400
		// (remove) Token: 0x060007A5 RID: 1957 RVA: 0x0002E438 File Offset: 0x0002D438
		
		public event EventHandler<RTP_ParticipantEventArgs> NewParticipant = null;

		// Token: 0x060007A6 RID: 1958 RVA: 0x0002E470 File Offset: 0x0002D470
		private void OnNewParticipant(RTP_Participant_Remote participant)
		{
			bool flag = this.NewParticipant != null;
			if (flag)
			{
				this.NewParticipant(this, new RTP_ParticipantEventArgs(participant));
			}
		}

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x060007A7 RID: 1959 RVA: 0x0002E4A0 File Offset: 0x0002D4A0
		// (remove) Token: 0x060007A8 RID: 1960 RVA: 0x0002E4D8 File Offset: 0x0002D4D8
		
		public event EventHandler<ExceptionEventArgs> Error = null;

		// Token: 0x060007A9 RID: 1961 RVA: 0x0002E510 File Offset: 0x0002D510
		internal void OnError(Exception exception)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, new ExceptionEventArgs(exception));
			}
		}

		// Token: 0x0400034A RID: 842
		private bool m_IsDisposed = false;

		// Token: 0x0400034B RID: 843
		private RTP_Participant_Local m_pLocalParticipant = null;

		// Token: 0x0400034C RID: 844
		private List<RTP_Session> m_pSessions = null;

		// Token: 0x0400034D RID: 845
		private Dictionary<string, RTP_Participant_Remote> m_pParticipants = null;
	}
}
