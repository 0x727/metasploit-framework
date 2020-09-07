using System;
using System.Diagnostics;

namespace LumiSoft.Net.IMAP.Client
{
	// Token: 0x0200022C RID: 556
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Client_FetchHandler
	{
		// Token: 0x0600141B RID: 5147 RVA: 0x0007E78C File Offset: 0x0007D78C
		internal void SetCurrentSeqNo(int seqNo)
		{
			this.m_CurrentSeqNo = seqNo;
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x0600141C RID: 5148 RVA: 0x0007E798 File Offset: 0x0007D798
		public int CurrentSeqNo
		{
			get
			{
				return this.m_CurrentSeqNo;
			}
		}

		// Token: 0x1400007D RID: 125
		// (add) Token: 0x0600141D RID: 5149 RVA: 0x0007E7B0 File Offset: 0x0007D7B0
		// (remove) Token: 0x0600141E RID: 5150 RVA: 0x0007E7E8 File Offset: 0x0007D7E8
		
		public event EventHandler NextMessage = null;

		// Token: 0x0600141F RID: 5151 RVA: 0x0007E820 File Offset: 0x0007D820
		internal void OnNextMessage()
		{
			bool flag = this.NextMessage != null;
			if (flag)
			{
				this.NextMessage(this, new EventArgs());
			}
		}

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x06001420 RID: 5152 RVA: 0x0007E850 File Offset: 0x0007D850
		// (remove) Token: 0x06001421 RID: 5153 RVA: 0x0007E888 File Offset: 0x0007D888
		
		public event EventHandler<IMAP_Client_Fetch_Body_EArgs> Body = null;

		// Token: 0x06001422 RID: 5154 RVA: 0x0007E8C0 File Offset: 0x0007D8C0
		internal void OnBody(IMAP_Client_Fetch_Body_EArgs eArgs)
		{
			bool flag = this.Body != null;
			if (flag)
			{
				this.Body(this, eArgs);
			}
		}

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x06001423 RID: 5155 RVA: 0x0007E8EC File Offset: 0x0007D8EC
		// (remove) Token: 0x06001424 RID: 5156 RVA: 0x0007E924 File Offset: 0x0007D924
		
		public event EventHandler<EventArgs<IMAP_Envelope>> Envelope = null;

		// Token: 0x06001425 RID: 5157 RVA: 0x0007E95C File Offset: 0x0007D95C
		internal void OnEnvelope(IMAP_Envelope envelope)
		{
			bool flag = this.Envelope != null;
			if (flag)
			{
				this.Envelope(this, new EventArgs<IMAP_Envelope>(envelope));
			}
		}

		// Token: 0x14000080 RID: 128
		// (add) Token: 0x06001426 RID: 5158 RVA: 0x0007E98C File Offset: 0x0007D98C
		// (remove) Token: 0x06001427 RID: 5159 RVA: 0x0007E9C4 File Offset: 0x0007D9C4
		
		public event EventHandler<EventArgs<string[]>> Flags = null;

		// Token: 0x06001428 RID: 5160 RVA: 0x0007E9FC File Offset: 0x0007D9FC
		internal void OnFlags(string[] flags)
		{
			bool flag = this.Flags != null;
			if (flag)
			{
				this.Flags(this, new EventArgs<string[]>(flags));
			}
		}

		// Token: 0x14000081 RID: 129
		// (add) Token: 0x06001429 RID: 5161 RVA: 0x0007EA2C File Offset: 0x0007DA2C
		// (remove) Token: 0x0600142A RID: 5162 RVA: 0x0007EA64 File Offset: 0x0007DA64
		
		public event EventHandler<EventArgs<DateTime>> InternalDate = null;

		// Token: 0x0600142B RID: 5163 RVA: 0x0007EA9C File Offset: 0x0007DA9C
		internal void OnInternalDate(DateTime date)
		{
			bool flag = this.InternalDate != null;
			if (flag)
			{
				this.InternalDate(this, new EventArgs<DateTime>(date));
			}
		}

		// Token: 0x14000082 RID: 130
		// (add) Token: 0x0600142C RID: 5164 RVA: 0x0007EACC File Offset: 0x0007DACC
		// (remove) Token: 0x0600142D RID: 5165 RVA: 0x0007EB04 File Offset: 0x0007DB04
		
		public event EventHandler<IMAP_Client_Fetch_Rfc822_EArgs> Rfc822 = null;

		// Token: 0x0600142E RID: 5166 RVA: 0x0007EB3C File Offset: 0x0007DB3C
		internal void OnRfc822(IMAP_Client_Fetch_Rfc822_EArgs eArgs)
		{
			bool flag = this.Rfc822 != null;
			if (flag)
			{
				this.Rfc822(this, eArgs);
			}
		}

		// Token: 0x14000083 RID: 131
		// (add) Token: 0x0600142F RID: 5167 RVA: 0x0007EB68 File Offset: 0x0007DB68
		// (remove) Token: 0x06001430 RID: 5168 RVA: 0x0007EBA0 File Offset: 0x0007DBA0
		
		public event EventHandler<EventArgs<string>> Rfc822Header = null;

		// Token: 0x06001431 RID: 5169 RVA: 0x0007EBD8 File Offset: 0x0007DBD8
		internal void OnRfc822Header(string header)
		{
			bool flag = this.Rfc822Header != null;
			if (flag)
			{
				this.Rfc822Header(this, new EventArgs<string>(header));
			}
		}

		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06001432 RID: 5170 RVA: 0x0007EC08 File Offset: 0x0007DC08
		// (remove) Token: 0x06001433 RID: 5171 RVA: 0x0007EC40 File Offset: 0x0007DC40
		
		public event EventHandler<EventArgs<int>> Rfc822Size = null;

		// Token: 0x06001434 RID: 5172 RVA: 0x0007EC78 File Offset: 0x0007DC78
		internal void OnSize(int size)
		{
			bool flag = this.Rfc822Size != null;
			if (flag)
			{
				this.Rfc822Size(this, new EventArgs<int>(size));
			}
		}

		// Token: 0x14000085 RID: 133
		// (add) Token: 0x06001435 RID: 5173 RVA: 0x0007ECA8 File Offset: 0x0007DCA8
		// (remove) Token: 0x06001436 RID: 5174 RVA: 0x0007ECE0 File Offset: 0x0007DCE0
		
		public event EventHandler<EventArgs<string>> Rfc822Text = null;

		// Token: 0x06001437 RID: 5175 RVA: 0x0007ED18 File Offset: 0x0007DD18
		internal void OnRfc822Text(string text)
		{
			bool flag = this.Rfc822Text != null;
			if (flag)
			{
				this.Rfc822Text(this, new EventArgs<string>(text));
			}
		}

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x06001438 RID: 5176 RVA: 0x0007ED48 File Offset: 0x0007DD48
		// (remove) Token: 0x06001439 RID: 5177 RVA: 0x0007ED80 File Offset: 0x0007DD80
		
		public event EventHandler<EventArgs<long>> UID = null;

		// Token: 0x0600143A RID: 5178 RVA: 0x0007EDB8 File Offset: 0x0007DDB8
		internal void OnUID(long uid)
		{
			bool flag = this.UID != null;
			if (flag)
			{
				this.UID(this, new EventArgs<long>(uid));
			}
		}

		// Token: 0x14000087 RID: 135
		// (add) Token: 0x0600143B RID: 5179 RVA: 0x0007EDE8 File Offset: 0x0007DDE8
		// (remove) Token: 0x0600143C RID: 5180 RVA: 0x0007EE20 File Offset: 0x0007DE20
		
		public event EventHandler<EventArgs<ulong>> X_GM_MSGID = null;

		// Token: 0x0600143D RID: 5181 RVA: 0x0007EE58 File Offset: 0x0007DE58
		internal void OnX_GM_MSGID(ulong msgID)
		{
			bool flag = this.X_GM_MSGID != null;
			if (flag)
			{
				this.X_GM_MSGID(this, new EventArgs<ulong>(msgID));
			}
		}

		// Token: 0x14000088 RID: 136
		// (add) Token: 0x0600143E RID: 5182 RVA: 0x0007EE88 File Offset: 0x0007DE88
		// (remove) Token: 0x0600143F RID: 5183 RVA: 0x0007EEC0 File Offset: 0x0007DEC0
		
		public event EventHandler<EventArgs<ulong>> X_GM_THRID = null;

		// Token: 0x06001440 RID: 5184 RVA: 0x0007EEF8 File Offset: 0x0007DEF8
		internal void OnX_GM_THRID(ulong threadID)
		{
			bool flag = this.X_GM_THRID != null;
			if (flag)
			{
				this.X_GM_THRID(this, new EventArgs<ulong>(threadID));
			}
		}

		// Token: 0x040007D3 RID: 2003
		private int m_CurrentSeqNo = -1;
	}
}
