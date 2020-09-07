using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000079 RID: 121
	public class SIP_t_NameAddress
	{
		// Token: 0x0600045E RID: 1118 RVA: 0x0001572A File Offset: 0x0001472A
		public SIP_t_NameAddress()
		{
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00015746 File Offset: 0x00014746
		public SIP_t_NameAddress(string value)
		{
			this.Parse(value);
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001576C File Offset: 0x0001476C
		public SIP_t_NameAddress(string displayName, AbsoluteUri uri)
		{
			bool flag = uri == null;
			if (flag)
			{
				throw new ArgumentNullException("uri");
			}
			this.DisplayName = displayName;
			this.Uri = uri;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x000157B8 File Offset: 0x000147B8
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x000157E8 File Offset: 0x000147E8
		public void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			reader.ReadToFirstChar();
			bool flag2 = reader.StartsWith("<");
			if (flag2)
			{
				this.m_pUri = AbsoluteUri.Parse(reader.ReadParenthesized());
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (;;)
				{
					stringBuilder.Append(reader.ReadToFirstChar());
					string value = reader.ReadWord();
					bool flag3 = string.IsNullOrEmpty(value);
					if (flag3)
					{
						break;
					}
					stringBuilder.Append(value);
				}
				reader.ReadToFirstChar();
				bool flag4 = reader.StartsWith("<");
				if (flag4)
				{
					this.m_DisplayName = stringBuilder.ToString().Trim();
					this.m_pUri = AbsoluteUri.Parse(reader.ReadParenthesized());
				}
				else
				{
					this.m_pUri = AbsoluteUri.Parse(stringBuilder.ToString());
				}
			}
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x000158C8 File Offset: 0x000148C8
		public string ToStringValue()
		{
			bool flag = string.IsNullOrEmpty(this.m_DisplayName);
			string result;
			if (flag)
			{
				result = "<" + this.m_pUri.ToString() + ">";
			}
			else
			{
				result = TextUtils.QuoteString(this.m_DisplayName) + " <" + this.m_pUri.ToString() + ">";
			}
			return result;
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00015930 File Offset: 0x00014930
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x00015948 File Offset: 0x00014948
		public string DisplayName
		{
			get
			{
				return this.m_DisplayName;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = "";
				}
				this.m_DisplayName = value;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00015970 File Offset: 0x00014970
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x00015988 File Offset: 0x00014988
		public AbsoluteUri Uri
		{
			get
			{
				return this.m_pUri;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_pUri = value;
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x000159B4 File Offset: 0x000149B4
		public bool IsSipOrSipsUri
		{
			get
			{
				return this.IsSipUri || this.IsSecureSipUri;
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x000159D8 File Offset: 0x000149D8
		public bool IsSipUri
		{
			get
			{
				return this.m_pUri.Scheme == "sip";
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x00015A0C File Offset: 0x00014A0C
		public bool IsSecureSipUri
		{
			get
			{
				return this.m_pUri.Scheme == "sips";
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x00015A40 File Offset: 0x00014A40
		public bool IsMailToUri
		{
			get
			{
				return this.m_pUri.Scheme == "mailto";
			}
		}

		// Token: 0x04000156 RID: 342
		private string m_DisplayName = "";

		// Token: 0x04000157 RID: 343
		private AbsoluteUri m_pUri = null;
	}
}
