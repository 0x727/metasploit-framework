using System;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000EF RID: 239
	public class POP3_ServerMessage
	{
		// Token: 0x0600098D RID: 2445 RVA: 0x00039C99 File Offset: 0x00038C99
		public POP3_ServerMessage(string uid, int size) : this(uid, size, null)
		{
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x00039CA8 File Offset: 0x00038CA8
		public POP3_ServerMessage(string uid, int size, object tag)
		{
			bool flag = uid == null;
			if (flag)
			{
				throw new ArgumentNullException("uid");
			}
			bool flag2 = uid == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'uid' value must be specified.");
			}
			bool flag3 = size < 0;
			if (flag3)
			{
				throw new ArgumentException("Argument 'size' value must be >= 0.");
			}
			this.m_UID = uid;
			this.m_Size = size;
			this.m_pTag = tag;
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x00039D3C File Offset: 0x00038D3C
		internal void SetIsMarkedForDeletion(bool value)
		{
			this.m_IsMarkedForDeletion = value;
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06000990 RID: 2448 RVA: 0x00039D48 File Offset: 0x00038D48
		public string UID
		{
			get
			{
				return this.m_UID;
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06000991 RID: 2449 RVA: 0x00039D60 File Offset: 0x00038D60
		public int Size
		{
			get
			{
				return this.m_Size;
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000992 RID: 2450 RVA: 0x00039D78 File Offset: 0x00038D78
		public bool IsMarkedForDeletion
		{
			get
			{
				return this.m_IsMarkedForDeletion;
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x00039D90 File Offset: 0x00038D90
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x00039DA8 File Offset: 0x00038DA8
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

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000995 RID: 2453 RVA: 0x00039DB4 File Offset: 0x00038DB4
		// (set) Token: 0x06000996 RID: 2454 RVA: 0x00039DCC File Offset: 0x00038DCC
		internal int SequenceNumber
		{
			get
			{
				return this.m_SequenceNumber;
			}
			set
			{
				this.m_SequenceNumber = value;
			}
		}

		// Token: 0x0400043D RID: 1085
		private int m_SequenceNumber = -1;

		// Token: 0x0400043E RID: 1086
		private string m_UID = "";

		// Token: 0x0400043F RID: 1087
		private int m_Size = 0;

		// Token: 0x04000440 RID: 1088
		private bool m_IsMarkedForDeletion = false;

		// Token: 0x04000441 RID: 1089
		private object m_pTag = null;
	}
}
