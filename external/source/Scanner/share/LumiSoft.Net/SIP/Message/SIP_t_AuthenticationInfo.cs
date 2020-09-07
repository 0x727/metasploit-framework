using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000065 RID: 101
	public class SIP_t_AuthenticationInfo : SIP_t_Value
	{
		// Token: 0x0600034E RID: 846 RVA: 0x00011D99 File Offset: 0x00010D99
		public SIP_t_AuthenticationInfo(string value)
		{
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600034F RID: 847 RVA: 0x00011DD4 File Offset: 0x00010DD4
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00011E04 File Offset: 0x00010E04
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			while (reader.Available > 0L)
			{
				string text = reader.QuotedReadToDelimiter(',');
				bool flag2 = text != null && text.Length > 0;
				if (flag2)
				{
					string[] array = text.Split(new char[]
					{
						'='
					}, 2);
					bool flag3 = array[0].ToLower() == "nextnonce";
					if (flag3)
					{
						this.NextNonce = array[1];
					}
					else
					{
						bool flag4 = array[0].ToLower() == "qop";
						if (flag4)
						{
							this.Qop = array[1];
						}
						else
						{
							bool flag5 = array[0].ToLower() == "rspauth";
							if (flag5)
							{
								this.ResponseAuth = array[1];
							}
							else
							{
								bool flag6 = array[0].ToLower() == "cnonce";
								if (flag6)
								{
									this.CNonce = array[1];
								}
								else
								{
									bool flag7 = array[0].ToLower() == "nc";
									if (!flag7)
									{
										throw new SIP_ParseException("Invalid Authentication-Info value !");
									}
									this.NonceCount = Convert.ToInt32(array[1]);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000351 RID: 849 RVA: 0x00011F4C File Offset: 0x00010F4C
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = this.m_NextNonce != null;
			if (flag)
			{
				stringBuilder.Append("nextnonce=" + this.m_NextNonce);
			}
			bool flag2 = this.m_Qop != null;
			if (flag2)
			{
				bool flag3 = stringBuilder.Length > 0;
				if (flag3)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append("qop=" + this.m_Qop);
			}
			bool flag4 = this.m_ResponseAuth != null;
			if (flag4)
			{
				bool flag5 = stringBuilder.Length > 0;
				if (flag5)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append("rspauth=" + TextUtils.QuoteString(this.m_ResponseAuth));
			}
			bool flag6 = this.m_CNonce != null;
			if (flag6)
			{
				bool flag7 = stringBuilder.Length > 0;
				if (flag7)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append("cnonce=" + this.m_CNonce);
			}
			bool flag8 = this.m_NonceCount != -1;
			if (flag8)
			{
				bool flag9 = stringBuilder.Length > 0;
				if (flag9)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append("nc=" + this.m_NonceCount.ToString("X8"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000352 RID: 850 RVA: 0x000120A8 File Offset: 0x000110A8
		// (set) Token: 0x06000353 RID: 851 RVA: 0x000120C0 File Offset: 0x000110C0
		public string NextNonce
		{
			get
			{
				return this.m_NextNonce;
			}
			set
			{
				this.m_NextNonce = value;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000354 RID: 852 RVA: 0x000120CC File Offset: 0x000110CC
		// (set) Token: 0x06000355 RID: 853 RVA: 0x000120E4 File Offset: 0x000110E4
		public string Qop
		{
			get
			{
				return this.m_Qop;
			}
			set
			{
				this.m_Qop = value;
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000356 RID: 854 RVA: 0x000120F0 File Offset: 0x000110F0
		// (set) Token: 0x06000357 RID: 855 RVA: 0x00012108 File Offset: 0x00011108
		public string ResponseAuth
		{
			get
			{
				return this.m_ResponseAuth;
			}
			set
			{
				this.m_ResponseAuth = value;
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000358 RID: 856 RVA: 0x00012114 File Offset: 0x00011114
		// (set) Token: 0x06000359 RID: 857 RVA: 0x0001212C File Offset: 0x0001112C
		public string CNonce
		{
			get
			{
				return this.m_CNonce;
			}
			set
			{
				this.m_CNonce = value;
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600035A RID: 858 RVA: 0x00012138 File Offset: 0x00011138
		// (set) Token: 0x0600035B RID: 859 RVA: 0x00012150 File Offset: 0x00011150
		public int NonceCount
		{
			get
			{
				return this.m_NonceCount;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					this.m_NonceCount = -1;
				}
				else
				{
					this.m_NonceCount = value;
				}
			}
		}

		// Token: 0x04000136 RID: 310
		private string m_NextNonce = null;

		// Token: 0x04000137 RID: 311
		private string m_Qop = null;

		// Token: 0x04000138 RID: 312
		private string m_ResponseAuth = null;

		// Token: 0x04000139 RID: 313
		private string m_CNonce = null;

		// Token: 0x0400013A RID: 314
		private int m_NonceCount = -1;
	}
}
