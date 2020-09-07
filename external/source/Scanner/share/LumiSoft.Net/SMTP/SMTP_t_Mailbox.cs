using System;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.SMTP
{
	// Token: 0x02000133 RID: 307
	public class SMTP_t_Mailbox
	{
		// Token: 0x06000C2D RID: 3117 RVA: 0x0004AD1C File Offset: 0x00049D1C
		public SMTP_t_Mailbox(string localPart, string domain)
		{
			bool flag = localPart == null;
			if (flag)
			{
				throw new ArgumentNullException("localPart");
			}
			bool flag2 = domain == null;
			if (flag2)
			{
				throw new ArgumentNullException("domain");
			}
			this.m_LocalPart = localPart;
			this.m_Domain = domain;
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x0004AD78 File Offset: 0x00049D78
		public override string ToString()
		{
			bool flag = MIME_Reader.IsDotAtom(this.m_LocalPart);
			string result;
			if (flag)
			{
				result = this.m_LocalPart + "@" + (Net_Utils.IsIPAddress(this.m_Domain) ? ("[" + this.m_Domain + "]") : this.m_Domain);
			}
			else
			{
				result = TextUtils.QuoteString(this.m_LocalPart) + "@" + (Net_Utils.IsIPAddress(this.m_Domain) ? ("[" + this.m_Domain + "]") : this.m_Domain);
			}
			return result;
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06000C2F RID: 3119 RVA: 0x0004AE18 File Offset: 0x00049E18
		public string LocalPart
		{
			get
			{
				return this.m_LocalPart;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06000C30 RID: 3120 RVA: 0x0004AE30 File Offset: 0x00049E30
		public string Domain
		{
			get
			{
				return this.m_Domain;
			}
		}

		// Token: 0x0400050C RID: 1292
		private string m_LocalPart = null;

		// Token: 0x0400050D RID: 1293
		private string m_Domain = null;
	}
}
