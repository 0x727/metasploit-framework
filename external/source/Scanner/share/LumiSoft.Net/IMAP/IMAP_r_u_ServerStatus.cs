using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001FB RID: 507
	public class IMAP_r_u_ServerStatus : IMAP_r_u
	{
		// Token: 0x06001200 RID: 4608 RVA: 0x0006CE7E File Offset: 0x0006BE7E
		public IMAP_r_u_ServerStatus(string responseCode, string responseText) : this(responseCode, null, responseText)
		{
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x0006CE8C File Offset: 0x0006BE8C
		public IMAP_r_u_ServerStatus(string responseCode, IMAP_t_orc optionalResponse, string responseText)
		{
			bool flag = responseCode == null;
			if (flag)
			{
				throw new ArgumentNullException("responseCode");
			}
			bool flag2 = responseCode == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("The argument 'responseCode' value must be specified.", "responseCode");
			}
			this.m_ResponseCode = responseCode;
			this.m_pOptionalResponse = optionalResponse;
			this.m_ResponseText = responseText;
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x0006CF08 File Offset: 0x0006BF08
		public static IMAP_r_u_ServerStatus Parse(string responseLine)
		{
			bool flag = responseLine == null;
			if (flag)
			{
				throw new ArgumentNullException("responseLine");
			}
			string[] array = responseLine.Split(new char[]
			{
				' '
			}, 3);
			string text = array[0];
			string responseCode = array[1];
			IMAP_t_orc optionalResponse = null;
			string responseText = array[2];
			bool flag2 = array[2].StartsWith("[");
			if (flag2)
			{
				StringReader stringReader = new StringReader(array[2]);
				optionalResponse = IMAP_t_orc.Parse(stringReader);
				responseText = stringReader.ReadToEnd();
			}
			return new IMAP_r_u_ServerStatus(responseCode, optionalResponse, responseText);
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0006CF90 File Offset: 0x0006BF90
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* " + this.m_ResponseCode + " ");
			bool flag = this.m_pOptionalResponse != null;
			if (flag)
			{
				stringBuilder.Append("[" + this.m_pOptionalResponse.ToString() + "] ");
			}
			stringBuilder.Append(this.m_ResponseText + "\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06001204 RID: 4612 RVA: 0x0006D014 File Offset: 0x0006C014
		public string ResponseCode
		{
			get
			{
				return this.m_ResponseCode;
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06001205 RID: 4613 RVA: 0x0006D02C File Offset: 0x0006C02C
		public IMAP_t_orc OptionalResponse
		{
			get
			{
				return this.m_pOptionalResponse;
			}
		}

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x06001206 RID: 4614 RVA: 0x0006D044 File Offset: 0x0006C044
		public string ResponseText
		{
			get
			{
				return this.m_ResponseText;
			}
		}

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x06001207 RID: 4615 RVA: 0x0006D05C File Offset: 0x0006C05C
		public bool IsError
		{
			get
			{
				return !this.m_ResponseCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x06001208 RID: 4616 RVA: 0x0006D084 File Offset: 0x0006C084
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

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x06001209 RID: 4617 RVA: 0x0006D0C8 File Offset: 0x0006C0C8
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

		// Token: 0x04000702 RID: 1794
		private string m_ResponseCode = "";

		// Token: 0x04000703 RID: 1795
		private IMAP_t_orc m_pOptionalResponse = null;

		// Token: 0x04000704 RID: 1796
		private string m_ResponseText = "";
	}
}
