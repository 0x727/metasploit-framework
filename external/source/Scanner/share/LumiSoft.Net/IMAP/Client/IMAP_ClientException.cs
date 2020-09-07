using System;

namespace LumiSoft.Net.IMAP.Client
{
	// Token: 0x02000230 RID: 560
	public class IMAP_ClientException : Exception
	{
		// Token: 0x06001463 RID: 5219 RVA: 0x0007F474 File Offset: 0x0007E474
		public IMAP_ClientException(IMAP_r_ServerStatus response) : base(response.ToString())
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pResponse = response;
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x0007F4B4 File Offset: 0x0007E4B4
		public IMAP_ClientException(string responseLine) : base(responseLine)
		{
			bool flag = responseLine == null;
			if (flag)
			{
				throw new ArgumentNullException("responseLine");
			}
			this.m_pResponse = IMAP_r_ServerStatus.Parse(responseLine);
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x0007F4F4 File Offset: 0x0007E4F4
		public IMAP_ClientException(string responseCode, string responseText) : base(responseCode + " " + responseText)
		{
			bool flag = responseCode == null;
			if (flag)
			{
				throw new ArgumentNullException("responseCode");
			}
			bool flag2 = responseCode == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'responseCode' value must be specified.", "responseCode");
			}
			bool flag3 = responseText == null;
			if (flag3)
			{
				throw new ArgumentNullException("responseText");
			}
			bool flag4 = responseText == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("Argument 'responseText' value must be specified.", "responseText");
			}
			this.m_pResponse = IMAP_r_ServerStatus.Parse(responseCode + " " + responseText);
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06001466 RID: 5222 RVA: 0x0007F59C File Offset: 0x0007E59C
		public IMAP_r_ServerStatus Response
		{
			get
			{
				return this.m_pResponse;
			}
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06001467 RID: 5223 RVA: 0x0007F5B4 File Offset: 0x0007E5B4
		public string StatusCode
		{
			get
			{
				return this.m_pResponse.ResponseCode;
			}
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06001468 RID: 5224 RVA: 0x0007F5D4 File Offset: 0x0007E5D4
		public string ResponseText
		{
			get
			{
				return this.m_pResponse.ResponseText;
			}
		}

		// Token: 0x040007EF RID: 2031
		private IMAP_r_ServerStatus m_pResponse = null;
	}
}
