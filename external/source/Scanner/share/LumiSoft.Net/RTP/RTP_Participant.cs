using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000CB RID: 203
	public abstract class RTP_Participant
	{
		// Token: 0x060007BB RID: 1979 RVA: 0x0002EA3C File Offset: 0x0002DA3C
		public RTP_Participant(string cname)
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
			this.m_CNAME = cname;
			this.m_pSources = new List<RTP_Source>();
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0002EAC0 File Offset: 0x0002DAC0
		internal void Dispose()
		{
			this.m_pSources = null;
			this.Removed = null;
			this.SourceAdded = null;
			this.SourceRemoved = null;
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x0002EAE0 File Offset: 0x0002DAE0
		internal void EnsureSource(RTP_Source source)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = !this.m_pSources.Contains(source);
			if (flag2)
			{
				this.m_pSources.Add(source);
				this.OnSourceAdded(source);
				source.Disposing += delegate(object sender, EventArgs e)
				{
					bool flag3 = this.m_pSources.Remove(source);
					if (flag3)
					{
						this.OnSourceRemoved(source);
						bool flag4 = this.m_pSources.Count == 0;
						if (flag4)
						{
							this.OnRemoved();
							this.Dispose();
						}
					}
				};
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x060007BE RID: 1982 RVA: 0x0002EB6C File Offset: 0x0002DB6C
		public string CNAME
		{
			get
			{
				return this.m_CNAME;
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x060007BF RID: 1983 RVA: 0x0002EB84 File Offset: 0x0002DB84
		public RTP_Source[] Sources
		{
			get
			{
				return this.m_pSources.ToArray();
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x060007C0 RID: 1984 RVA: 0x0002EBA4 File Offset: 0x0002DBA4
		// (set) Token: 0x060007C1 RID: 1985 RVA: 0x0002EBBC File Offset: 0x0002DBBC
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

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x060007C2 RID: 1986 RVA: 0x0002EBC8 File Offset: 0x0002DBC8
		// (remove) Token: 0x060007C3 RID: 1987 RVA: 0x0002EC00 File Offset: 0x0002DC00
		
		public event EventHandler Removed = null;

		// Token: 0x060007C4 RID: 1988 RVA: 0x0002EC38 File Offset: 0x0002DC38
		private void OnRemoved()
		{
			bool flag = this.Removed != null;
			if (flag)
			{
				this.Removed(this, new EventArgs());
			}
		}

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x060007C5 RID: 1989 RVA: 0x0002EC68 File Offset: 0x0002DC68
		// (remove) Token: 0x060007C6 RID: 1990 RVA: 0x0002ECA0 File Offset: 0x0002DCA0
		
		public event EventHandler<RTP_SourceEventArgs> SourceAdded = null;

		// Token: 0x060007C7 RID: 1991 RVA: 0x0002ECD8 File Offset: 0x0002DCD8
		private void OnSourceAdded(RTP_Source source)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = this.SourceAdded != null;
			if (flag2)
			{
				this.SourceAdded(this, new RTP_SourceEventArgs(source));
			}
		}

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x060007C8 RID: 1992 RVA: 0x0002ED1C File Offset: 0x0002DD1C
		// (remove) Token: 0x060007C9 RID: 1993 RVA: 0x0002ED54 File Offset: 0x0002DD54
		
		public event EventHandler<RTP_SourceEventArgs> SourceRemoved = null;

		// Token: 0x060007CA RID: 1994 RVA: 0x0002ED8C File Offset: 0x0002DD8C
		private void OnSourceRemoved(RTP_Source source)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = this.SourceRemoved != null;
			if (flag2)
			{
				this.SourceRemoved(this, new RTP_SourceEventArgs(source));
			}
		}

		// Token: 0x0400035A RID: 858
		private string m_CNAME = "";

		// Token: 0x0400035B RID: 859
		private List<RTP_Source> m_pSources = null;

		// Token: 0x0400035C RID: 860
		private object m_pTag = null;
	}
}
