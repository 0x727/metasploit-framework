using System;
using System.Collections.Generic;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000180 RID: 384
	public class Mail_t_Group : Mail_t_Address
	{
		// Token: 0x06000F87 RID: 3975 RVA: 0x00060124 File Offset: 0x0005F124
		public Mail_t_Group(string displayName)
		{
			this.m_DisplayName = displayName;
			this.m_pList = new List<Mail_t_Mailbox>();
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x00060150 File Offset: 0x0005F150
		public override string ToString()
		{
			return this.ToString(null);
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0006016C File Offset: 0x0005F16C
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = string.IsNullOrEmpty(this.m_DisplayName);
			if (flag)
			{
				stringBuilder.Append(":");
			}
			else
			{
				bool flag2 = MIME_Encoding_EncodedWord.MustEncode(this.m_DisplayName);
				if (flag2)
				{
					stringBuilder.Append(wordEncoder.Encode(this.m_DisplayName) + ":");
				}
				else
				{
					stringBuilder.Append(TextUtils.QuoteString(this.m_DisplayName) + ":");
				}
			}
			for (int i = 0; i < this.m_pList.Count; i++)
			{
				stringBuilder.Append(this.m_pList[i].ToString(wordEncoder));
				bool flag3 = i < this.m_pList.Count - 1;
				if (flag3)
				{
					stringBuilder.Append(",");
				}
			}
			stringBuilder.Append(";");
			return stringBuilder.ToString();
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06000F8A RID: 3978 RVA: 0x00060264 File Offset: 0x0005F264
		// (set) Token: 0x06000F8B RID: 3979 RVA: 0x0006027C File Offset: 0x0005F27C
		public string DisplayName
		{
			get
			{
				return this.m_DisplayName;
			}
			set
			{
				this.m_DisplayName = value;
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06000F8C RID: 3980 RVA: 0x00060288 File Offset: 0x0005F288
		public List<Mail_t_Mailbox> Members
		{
			get
			{
				return this.m_pList;
			}
		}

		// Token: 0x0400066F RID: 1647
		private string m_DisplayName = null;

		// Token: 0x04000670 RID: 1648
		private List<Mail_t_Mailbox> m_pList = null;
	}
}
