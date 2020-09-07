using System;

namespace LumiSoft.Net.POP3.Client
{
	// Token: 0x020000E4 RID: 228
	public class POP3_ClientException : Exception
	{
		// Token: 0x0600092A RID: 2346 RVA: 0x000371A4 File Offset: 0x000361A4
		public POP3_ClientException(string responseLine) : base(responseLine)
		{
			bool flag = responseLine == null;
			if (flag)
			{
				throw new ArgumentNullException("responseLine");
			}
			string[] array = responseLine.Split(new char[0], 2);
			this.m_StatusCode = array[0];
			bool flag2 = array.Length == 2;
			if (flag2)
			{
				this.m_ResponseText = array[1];
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x0600092B RID: 2347 RVA: 0x00037210 File Offset: 0x00036210
		public string StatusCode
		{
			get
			{
				return this.m_StatusCode;
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x0600092C RID: 2348 RVA: 0x00037228 File Offset: 0x00036228
		public string ResponseText
		{
			get
			{
				return this.m_ResponseText;
			}
		}

		// Token: 0x0400041E RID: 1054
		private string m_StatusCode = "";

		// Token: 0x0400041F RID: 1055
		private string m_ResponseText = "";
	}
}
