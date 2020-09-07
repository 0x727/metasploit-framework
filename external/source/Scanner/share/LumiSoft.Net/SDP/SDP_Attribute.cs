using System;

namespace LumiSoft.Net.SDP
{
	// Token: 0x020000BB RID: 187
	public class SDP_Attribute
	{
		// Token: 0x06000729 RID: 1833 RVA: 0x0002BFA0 File Offset: 0x0002AFA0
		public SDP_Attribute(string name, string value)
		{
			this.m_Name = name;
			this.Value = value;
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0002BFD0 File Offset: 0x0002AFD0
		public static SDP_Attribute Parse(string aValue)
		{
			StringReader stringReader = new StringReader(aValue);
			stringReader.QuotedReadToDelimiter('=');
			string text = stringReader.QuotedReadToDelimiter(':');
			bool flag = text == null;
			if (flag)
			{
				throw new Exception("SDP message \"a\" field <attribute> name is missing !");
			}
			string name = text;
			string value = "";
			text = stringReader.ReadToEnd();
			bool flag2 = text != null;
			if (flag2)
			{
				value = text;
			}
			return new SDP_Attribute(name, value);
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0002C040 File Offset: 0x0002B040
		public string ToValue()
		{
			bool flag = string.IsNullOrEmpty(this.m_Value);
			string result;
			if (flag)
			{
				result = "a=" + this.m_Name + "\r\n";
			}
			else
			{
				result = string.Concat(new string[]
				{
					"a=",
					this.m_Name,
					":",
					this.m_Value,
					"\r\n"
				});
			}
			return result;
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x0600072C RID: 1836 RVA: 0x0002C0B0 File Offset: 0x0002B0B0
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x0002C0C8 File Offset: 0x0002B0C8
		// (set) Token: 0x0600072E RID: 1838 RVA: 0x0002C0E0 File Offset: 0x0002B0E0
		public string Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		// Token: 0x04000310 RID: 784
		private string m_Name = "";

		// Token: 0x04000311 RID: 785
		private string m_Value = "";
	}
}
