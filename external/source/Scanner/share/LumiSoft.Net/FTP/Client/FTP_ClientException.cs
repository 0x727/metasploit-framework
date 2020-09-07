using System;

namespace LumiSoft.Net.FTP.Client
{
	// Token: 0x02000234 RID: 564
	public class FTP_ClientException : Exception
	{
		// Token: 0x06001475 RID: 5237 RVA: 0x0007F8B0 File Offset: 0x0007E8B0
		public FTP_ClientException(string responseLine) : base(responseLine)
		{
			bool flag = responseLine == null;
			if (flag)
			{
				throw new ArgumentNullException("responseLine");
			}
			string[] array = responseLine.Split(new char[]
			{
				' '
			}, 2);
			try
			{
				this.m_StatusCode = Convert.ToInt32(array[0]);
			}
			catch
			{
			}
			bool flag2 = array.Length == 2;
			if (flag2)
			{
				this.m_ResponseText = array[1];
			}
		}

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06001476 RID: 5238 RVA: 0x0007F940 File Offset: 0x0007E940
		public int StatusCode
		{
			get
			{
				return this.m_StatusCode;
			}
		}

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06001477 RID: 5239 RVA: 0x0007F958 File Offset: 0x0007E958
		public string ResponseText
		{
			get
			{
				return this.m_ResponseText;
			}
		}

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06001478 RID: 5240 RVA: 0x0007F970 File Offset: 0x0007E970
		public bool IsPermanentError
		{
			get
			{
				return this.m_StatusCode >= 500 && this.m_StatusCode <= 599;
			}
		}

		// Token: 0x040007FA RID: 2042
		private int m_StatusCode = 500;

		// Token: 0x040007FB RID: 2043
		private string m_ResponseText = "";
	}
}
