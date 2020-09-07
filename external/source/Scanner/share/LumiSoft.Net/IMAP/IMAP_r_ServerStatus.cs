using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000203 RID: 515
	public class IMAP_r_ServerStatus : IMAP_r
	{
		// Token: 0x06001235 RID: 4661 RVA: 0x0006DD8C File Offset: 0x0006CD8C
		public IMAP_r_ServerStatus(string commandTag, string responseCode, string responseText) : this(commandTag, responseCode, null, responseText)
		{
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x0006DD9C File Offset: 0x0006CD9C
		public IMAP_r_ServerStatus(string commandTag, string responseCode, IMAP_t_orc optionalResponse, string responseText)
		{
			this.m_CommandTag = "";
			this.m_ResponseCode = "";
			this.m_pOptionalResponse = null;
			this.m_ResponseText = "";
			//base..ctor();
			bool flag = commandTag == null;
			if (flag)
			{
				throw new ArgumentNullException("commandTag");
			}
			bool flag2 = commandTag == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("The argument 'commandTag' value must be specified.", "commandTag");
			}
			bool flag3 = responseCode == null;
			if (flag3)
			{
				throw new ArgumentNullException("responseCode");
			}
			bool flag4 = responseCode == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("The argument 'responseCode' value must be specified.", "responseCode");
			}
			this.m_CommandTag = commandTag;
			this.m_ResponseCode = responseCode;
			this.m_pOptionalResponse = optionalResponse;
			this.m_ResponseText = responseText;
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x0006DE5E File Offset: 0x0006CE5E
		internal IMAP_r_ServerStatus(string responseCode, string responseText)
		{
			this.m_CommandTag = "";
			this.m_ResponseCode = "";
			this.m_pOptionalResponse = null;
			this.m_ResponseText = "";
			//base..ctor();
			this.m_ResponseCode = responseCode;
			this.m_ResponseText = responseText;
		}

		// Token: 0x06001238 RID: 4664 RVA: 0x0006DEA0 File Offset: 0x0006CEA0
		public static IMAP_r_ServerStatus Parse(string responseLine)
		{
			bool flag = responseLine == null;
			if (flag)
			{
				throw new ArgumentNullException("responseLine");
			}
			bool flag2 = responseLine.StartsWith("+");
			IMAP_r_ServerStatus result;
			if (flag2)
			{
				string[] array = responseLine.Split(new char[]
				{
					' '
				}, 2);
				string responseText = (array.Length == 2) ? array[1] : null;
				result = new IMAP_r_ServerStatus("+", "+", responseText);
			}
			else
			{
				string[] array2 = responseLine.Split(new char[]
				{
					' '
				}, 3);
				string commandTag = array2[0];
				string responseCode = array2[1];
				IMAP_t_orc optionalResponse = null;
				string responseText2 = array2[2];
				bool flag3 = array2[2].StartsWith("[");
				if (flag3)
				{
					StringReader stringReader = new StringReader(array2[2]);
					optionalResponse = IMAP_t_orc.Parse(stringReader);
					responseText2 = stringReader.ReadToEnd();
				}
				result = new IMAP_r_ServerStatus(commandTag, responseCode, optionalResponse, responseText2);
			}
			return result;
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x0006DF78 File Offset: 0x0006CF78
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = !string.IsNullOrEmpty(this.m_CommandTag);
			if (flag)
			{
				stringBuilder.Append(this.m_CommandTag + " ");
			}
			stringBuilder.Append(this.m_ResponseCode + " ");
			bool flag2 = this.m_pOptionalResponse != null;
			if (flag2)
			{
				stringBuilder.Append("[" + this.m_pOptionalResponse.ToString() + "] ");
			}
			stringBuilder.Append(this.m_ResponseText + "\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x0600123A RID: 4666 RVA: 0x0006E020 File Offset: 0x0006D020
		public string CommandTag
		{
			get
			{
				return this.m_CommandTag;
			}
		}

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x0006E038 File Offset: 0x0006D038
		public string ResponseCode
		{
			get
			{
				return this.m_ResponseCode;
			}
		}

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x0600123C RID: 4668 RVA: 0x0006E050 File Offset: 0x0006D050
		public IMAP_t_orc OptionalResponse
		{
			get
			{
				return this.m_pOptionalResponse;
			}
		}

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x0006E068 File Offset: 0x0006D068
		public string ResponseText
		{
			get
			{
				return this.m_ResponseText;
			}
		}

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x0006E080 File Offset: 0x0006D080
		public bool IsError
		{
			get
			{
				bool flag = this.m_ResponseCode.Equals("NO", StringComparison.InvariantCultureIgnoreCase);
				bool result;
				if (flag)
				{
					result = true;
				}
				else
				{
					bool flag2 = this.m_ResponseCode.Equals("BAD", StringComparison.InvariantCultureIgnoreCase);
					result = flag2;
				}
				return result;
			}
		}

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x0006E0C8 File Offset: 0x0006D0C8
		public bool IsContinue
		{
			get
			{
				return this.m_ResponseCode.Equals("+", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x0006E0EC File Offset: 0x0006D0EC
		[Obsolete("Use property OptionalResponse instead.")]
		public string OptionalResponseCode
		{
			get
			{
				bool flag = this.m_pOptionalResponse == null;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this.m_pOptionalResponse.ToString().Split(new char[]
					{
						' '
					})[0];
				}
				return result;
			}
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x06001241 RID: 4673 RVA: 0x0006E130 File Offset: 0x0006D130
		[Obsolete("Use property OptionalResponse instead.")]
		public string OptionalResponseArgs
		{
			get
			{
				bool flag = this.m_pOptionalResponse == null;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					string[] array = this.m_pOptionalResponse.ToString().Split(new char[]
					{
						' '
					}, 2);
					result = ((array.Length == 2) ? array[1] : "");
				}
				return result;
			}
		}

		// Token: 0x04000715 RID: 1813
		private string m_CommandTag;

		// Token: 0x04000716 RID: 1814
		private string m_ResponseCode;

		// Token: 0x04000717 RID: 1815
		private IMAP_t_orc m_pOptionalResponse;

		// Token: 0x04000718 RID: 1816
		private string m_ResponseText;
	}
}
