using System;
using System.Text;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000110 RID: 272
	public class MIME_h_Unparsed : MIME_h
	{
		// Token: 0x06000A99 RID: 2713 RVA: 0x00040910 File Offset: 0x0003F910
		internal MIME_h_Unparsed(string value, Exception exception)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			string[] array = value.Split(new char[]
			{
				':'
			}, 2);
			bool flag2 = array.Length != 2;
			if (flag2)
			{
				throw new ParseException("Invalid Content-Type: header field value '" + value + "'.");
			}
			this.m_Name = array[0];
			this.m_Value = array[1].Trim();
			this.m_ParseValue = value;
			this.m_pException = exception;
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x000409B1 File Offset: 0x0003F9B1
		public static MIME_h_Unparsed Parse(string value)
		{
			throw new InvalidOperationException();
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x000409BC File Offset: 0x0003F9BC
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			return this.m_ParseValue;
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06000A9C RID: 2716 RVA: 0x000409D4 File Offset: 0x0003F9D4
		public override bool IsModified
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06000A9D RID: 2717 RVA: 0x000409E8 File Offset: 0x0003F9E8
		public override string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06000A9E RID: 2718 RVA: 0x00040A00 File Offset: 0x0003FA00
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06000A9F RID: 2719 RVA: 0x00040A18 File Offset: 0x0003FA18
		public Exception Exception
		{
			get
			{
				return this.m_pException;
			}
		}

		// Token: 0x04000474 RID: 1140
		private string m_ParseValue = null;

		// Token: 0x04000475 RID: 1141
		private string m_Name = null;

		// Token: 0x04000476 RID: 1142
		private string m_Value = null;

		// Token: 0x04000477 RID: 1143
		private Exception m_pException = null;
	}
}
