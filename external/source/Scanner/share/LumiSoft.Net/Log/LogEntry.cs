using System;
using System.Net;
using System.Security.Principal;

namespace LumiSoft.Net.Log
{
	// Token: 0x02000036 RID: 54
	public class LogEntry
	{
		// Token: 0x060001ED RID: 493 RVA: 0x0000C7E4 File Offset: 0x0000B7E4
		public LogEntry(LogEntryType type, string id, long size, string text)
		{
			this.m_Type = type;
			this.m_ID = id;
			this.m_Size = size;
			this.m_Text = text;
			this.m_Time = DateTime.Now;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000C86C File Offset: 0x0000B86C
		public LogEntry(LogEntryType type, string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEP, IPEndPoint remoteEP, byte[] data)
		{
			this.m_Type = type;
			this.m_ID = id;
			this.m_pUserIdentity = userIdentity;
			this.m_Size = size;
			this.m_Text = text;
			this.m_pLocalEP = localEP;
			this.m_pRemoteEP = remoteEP;
			this.m_pData = data;
			this.m_Time = DateTime.Now;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000C914 File Offset: 0x0000B914
		public LogEntry(LogEntryType type, string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEP, IPEndPoint remoteEP, Exception exception)
		{
			this.m_Type = type;
			this.m_ID = id;
			this.m_pUserIdentity = userIdentity;
			this.m_Size = size;
			this.m_Text = text;
			this.m_pLocalEP = localEP;
			this.m_pRemoteEP = remoteEP;
			this.m_pException = exception;
			this.m_Time = DateTime.Now;
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x0000C9BC File Offset: 0x0000B9BC
		public LogEntryType EntryType
		{
			get
			{
				return this.m_Type;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000C9D4 File Offset: 0x0000B9D4
		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x0000C9EC File Offset: 0x0000B9EC
		public DateTime Time
		{
			get
			{
				return this.m_Time;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x0000CA04 File Offset: 0x0000BA04
		public GenericIdentity UserIdentity
		{
			get
			{
				return this.m_pUserIdentity;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000CA1C File Offset: 0x0000BA1C
		public long Size
		{
			get
			{
				return this.m_Size;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x0000CA34 File Offset: 0x0000BA34
		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x0000CA4C File Offset: 0x0000BA4C
		public Exception Exception
		{
			get
			{
				return this.m_pException;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000CA64 File Offset: 0x0000BA64
		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.m_pLocalEP;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x0000CA7C File Offset: 0x0000BA7C
		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_pRemoteEP;
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x0000CA94 File Offset: 0x0000BA94
		public byte[] Data
		{
			get
			{
				return this.m_pData;
			}
		}

		// Token: 0x040000D2 RID: 210
		private LogEntryType m_Type = LogEntryType.Text;

		// Token: 0x040000D3 RID: 211
		private string m_ID = "";

		// Token: 0x040000D4 RID: 212
		private DateTime m_Time;

		// Token: 0x040000D5 RID: 213
		private GenericIdentity m_pUserIdentity = null;

		// Token: 0x040000D6 RID: 214
		private long m_Size = 0L;

		// Token: 0x040000D7 RID: 215
		private string m_Text = "";

		// Token: 0x040000D8 RID: 216
		private Exception m_pException = null;

		// Token: 0x040000D9 RID: 217
		private IPEndPoint m_pLocalEP = null;

		// Token: 0x040000DA RID: 218
		private IPEndPoint m_pRemoteEP = null;

		// Token: 0x040000DB RID: 219
		private byte[] m_pData = null;
	}
}
