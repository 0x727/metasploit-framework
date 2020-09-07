using System;
using System.Diagnostics;
using System.IO;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000223 RID: 547
	public class IMAP_e_Append : EventArgs
	{
		// Token: 0x0600137F RID: 4991 RVA: 0x00077BA0 File Offset: 0x00076BA0
		internal IMAP_e_Append(string folder, string[] flags, DateTime date, int size, IMAP_r_ServerStatus response)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag2 = flags == null;
			if (flag2)
			{
				throw new ArgumentNullException("flags");
			}
			bool flag3 = response == null;
			if (flag3)
			{
				throw new ArgumentNullException("response");
			}
			this.m_Folder = folder;
			this.m_pFlags = flags;
			this.m_Date = date;
			this.m_Size = size;
			this.m_pResponse = response;
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06001380 RID: 4992 RVA: 0x00077C4C File Offset: 0x00076C4C
		// (set) Token: 0x06001381 RID: 4993 RVA: 0x00077C64 File Offset: 0x00076C64
		public IMAP_r_ServerStatus Response
		{
			get
			{
				return this.m_pResponse;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_pResponse = value;
			}
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06001382 RID: 4994 RVA: 0x00077C90 File Offset: 0x00076C90
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06001383 RID: 4995 RVA: 0x00077CA8 File Offset: 0x00076CA8
		public string[] Flags
		{
			get
			{
				return this.m_pFlags;
			}
		}

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06001384 RID: 4996 RVA: 0x00077CC0 File Offset: 0x00076CC0
		public DateTime InternalDate
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06001385 RID: 4997 RVA: 0x00077CD8 File Offset: 0x00076CD8
		public int Size
		{
			get
			{
				return this.m_Size;
			}
		}

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x06001386 RID: 4998 RVA: 0x00077CF0 File Offset: 0x00076CF0
		// (set) Token: 0x06001387 RID: 4999 RVA: 0x00077D08 File Offset: 0x00076D08
		public Stream Stream
		{
			get
			{
				return this.m_pStream;
			}
			set
			{
				this.m_pStream = value;
			}
		}

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x06001388 RID: 5000 RVA: 0x00077D14 File Offset: 0x00076D14
		// (remove) Token: 0x06001389 RID: 5001 RVA: 0x00077D4C File Offset: 0x00076D4C
		
		public event EventHandler Completed = null;

		// Token: 0x0600138A RID: 5002 RVA: 0x00077D84 File Offset: 0x00076D84
		internal void OnCompleted()
		{
			bool flag = this.Completed != null;
			if (flag)
			{
				this.Completed(this, new EventArgs());
			}
			this.Completed = null;
		}

		// Token: 0x040007A7 RID: 1959
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x040007A8 RID: 1960
		private string m_Folder = null;

		// Token: 0x040007A9 RID: 1961
		private string[] m_pFlags = null;

		// Token: 0x040007AA RID: 1962
		private DateTime m_Date = DateTime.MinValue;

		// Token: 0x040007AB RID: 1963
		private int m_Size = 0;

		// Token: 0x040007AC RID: 1964
		private Stream m_pStream = null;
	}
}
