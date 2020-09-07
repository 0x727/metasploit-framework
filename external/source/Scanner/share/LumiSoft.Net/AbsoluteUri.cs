using System;

namespace LumiSoft.Net
{
	// Token: 0x02000016 RID: 22
	public class AbsoluteUri
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00004260 File Offset: 0x00003260
		internal AbsoluteUri()
		{
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004280 File Offset: 0x00003280
		public static AbsoluteUri Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			bool flag2 = value == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'value' value must be specified.");
			}
			string[] array = value.Split(new char[]
			{
				':'
			}, 2);
			//bool flag3 = array[0].ToLower() == "sip" || array[0].ToLower() == "sips";
			AbsoluteUri result;
			//if (flag3)
			//{
			//	SIP_Uri sip_Uri = new SIP_Uri();
			//	sip_Uri.ParseInternal(value);
			//	result = sip_Uri;
			//}
			//else
			{
				AbsoluteUri absoluteUri = new AbsoluteUri();
				absoluteUri.ParseInternal(value);
				result = absoluteUri;
			}
			return result;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004330 File Offset: 0x00003330
		protected virtual void ParseInternal(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			string[] array = value.Split(new char[]
			{
				':'
			}, 1);
			this.m_Scheme = array[0].ToLower();
			bool flag2 = array.Length == 2;
			if (flag2)
			{
				this.m_Value = array[1];
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004388 File Offset: 0x00003388
		public override string ToString()
		{
			return this.m_Scheme + ":" + this.m_Value;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600006C RID: 108 RVA: 0x000043B0 File Offset: 0x000033B0
		public virtual string Scheme
		{
			get
			{
				return this.m_Scheme;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600006D RID: 109 RVA: 0x000043C8 File Offset: 0x000033C8
		public string Value
		{
			get
			{
				return this.ToString().Split(new char[]
				{
					':'
				}, 2)[1];
			}
		}

		// Token: 0x04000037 RID: 55
		private string m_Scheme = "";

		// Token: 0x04000038 RID: 56
		private string m_Value = "";
	}
}
