using System;

namespace LumiSoft.Net.Mime
{
	// Token: 0x0200015E RID: 350
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class HeaderField
	{
		// Token: 0x06000E0E RID: 3598 RVA: 0x000572F0 File Offset: 0x000562F0
		public HeaderField()
		{
		}

		// Token: 0x06000E0F RID: 3599 RVA: 0x00057310 File Offset: 0x00056310
		public HeaderField(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06000E10 RID: 3600 RVA: 0x00057340 File Offset: 0x00056340
		// (set) Token: 0x06000E11 RID: 3601 RVA: 0x00057358 File Offset: 0x00056358
		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				bool flag = value == "";
				if (flag)
				{
					throw new Exception("Header Field name can't be empty !");
				}
				bool flag2 = !value.EndsWith(":");
				if (flag2)
				{
					value += ":";
				}
				foreach (char c in value.Substring(0, value.Length - 1))
				{
					bool flag3 = c < '!' || c > '~';
					if (flag3)
					{
						throw new Exception("Invalid field name '" + value + "'. A field name MUST be composed of printable US-ASCII characters (i.e.,characters that have values between 33 and 126, inclusive),except\tcolon.");
					}
				}
				this.m_Name = value;
			}
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06000E12 RID: 3602 RVA: 0x00057404 File Offset: 0x00056404
		// (set) Token: 0x06000E13 RID: 3603 RVA: 0x00057421 File Offset: 0x00056421
		public string Value
		{
			get
			{
				return MimeUtils.DecodeWords(this.m_Value);
			}
			set
			{
				this.m_Value = Core.CanonicalEncode(value, "utf-8");
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06000E14 RID: 3604 RVA: 0x00057438 File Offset: 0x00056438
		internal string EncodedValue
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040005ED RID: 1517
		private string m_Name = "";

		// Token: 0x040005EE RID: 1518
		private string m_Value = "";
	}
}
