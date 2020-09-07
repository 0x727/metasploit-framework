using System;
using System.Text;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000226 RID: 550
	public class IMAP_MessageInfo
	{
		// Token: 0x06001392 RID: 5010 RVA: 0x00077EFC File Offset: 0x00076EFC
		public IMAP_MessageInfo(string id, long uid, string[] flags, int size, DateTime internalDate)
		{
			bool flag = id == null;
			if (flag)
			{
				throw new ArgumentNullException("id");
			}
			bool flag2 = id == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'id' value must be specified.", "id");
			}
			bool flag3 = uid < 1L;
			if (flag3)
			{
				throw new ArgumentException("Argument 'uid' value must be >= 1.", "uid");
			}
			bool flag4 = flags == null;
			if (flag4)
			{
				throw new ArgumentNullException("flags");
			}
			this.m_ID = id;
			this.m_UID = uid;
			this.m_pFlags = flags;
			this.m_Size = size;
			this.m_InternalDate = internalDate;
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x00077FBC File Offset: 0x00076FBC
		public bool ContainsFlag(string flag)
		{
			bool flag2 = flag == null;
			if (flag2)
			{
				throw new ArgumentNullException("flag");
			}
			foreach (string a in this.m_pFlags)
			{
				bool flag3 = string.Equals(a, flag, StringComparison.InvariantCultureIgnoreCase);
				if (flag3)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x00078018 File Offset: 0x00077018
		internal string FlagsToImapString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			for (int i = 0; i < this.m_pFlags.Length; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append("\\" + this.m_pFlags[i]);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x00078098 File Offset: 0x00077098
		internal void UpdateFlags(IMAP_Flags_SetType setType, string[] flags)
		{
			bool flag = flags == null;
			if (flag)
			{
				throw new ArgumentNullException("flags");
			}
			bool flag2 = setType == IMAP_Flags_SetType.Add;
			if (flag2)
			{
				this.m_pFlags = IMAP_Utils.MessageFlagsAdd(this.m_pFlags, flags);
			}
			else
			{
				bool flag3 = setType == IMAP_Flags_SetType.Remove;
				if (flag3)
				{
					this.m_pFlags = IMAP_Utils.MessageFlagsRemove(this.m_pFlags, flags);
				}
				else
				{
					this.m_pFlags = flags;
				}
			}
		}

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x06001396 RID: 5014 RVA: 0x00078100 File Offset: 0x00077100
		public string ID
		{
			get
			{
				return this.m_ID;
			}
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x06001397 RID: 5015 RVA: 0x00078118 File Offset: 0x00077118
		public long UID
		{
			get
			{
				return this.m_UID;
			}
		}

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06001398 RID: 5016 RVA: 0x00078130 File Offset: 0x00077130
		public string[] Flags
		{
			get
			{
				return this.m_pFlags;
			}
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06001399 RID: 5017 RVA: 0x00078148 File Offset: 0x00077148
		public int Size
		{
			get
			{
				return this.m_Size;
			}
		}

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x0600139A RID: 5018 RVA: 0x00078160 File Offset: 0x00077160
		public DateTime InternalDate
		{
			get
			{
				return this.m_InternalDate;
			}
		}

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x0600139B RID: 5019 RVA: 0x00078178 File Offset: 0x00077178
		// (set) Token: 0x0600139C RID: 5020 RVA: 0x00078190 File Offset: 0x00077190
		internal int SeqNo
		{
			get
			{
				return this.m_SeqNo;
			}
			set
			{
				this.m_SeqNo = value;
			}
		}

		// Token: 0x040007B7 RID: 1975
		private string m_ID = null;

		// Token: 0x040007B8 RID: 1976
		private long m_UID = 0L;

		// Token: 0x040007B9 RID: 1977
		private string[] m_pFlags = null;

		// Token: 0x040007BA RID: 1978
		private int m_Size = 0;

		// Token: 0x040007BB RID: 1979
		private DateTime m_InternalDate;

		// Token: 0x040007BC RID: 1980
		private int m_SeqNo = 1;
	}
}
