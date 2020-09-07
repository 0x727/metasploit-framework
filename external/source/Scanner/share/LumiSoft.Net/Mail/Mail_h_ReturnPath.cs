using System;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x0200017D RID: 381
	public class Mail_h_ReturnPath : MIME_h
	{
		// Token: 0x06000F71 RID: 3953 RVA: 0x0005FAB0 File Offset: 0x0005EAB0
		public Mail_h_ReturnPath(string address)
		{
			this.m_Address = address;
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x0005FAD0 File Offset: 0x0005EAD0
		public static Mail_h_ReturnPath Parse(string value)
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
				throw new ParseException("Invalid header field value '" + value + "'.");
			}
			Mail_h_ReturnPath mail_h_ReturnPath = new Mail_h_ReturnPath(null);
			MIME_Reader mime_Reader = new MIME_Reader(array[1].Trim());
			mime_Reader.ToFirstChar();
			bool flag3 = !mime_Reader.StartsWith("<");
			if (flag3)
			{
				mail_h_ReturnPath.m_Address = mime_Reader.ToEnd();
			}
			else
			{
				mail_h_ReturnPath.m_Address = mime_Reader.ReadParenthesized();
			}
			return mail_h_ReturnPath;
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x0005FB80 File Offset: 0x0005EB80
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			bool flag = string.IsNullOrEmpty(this.m_Address);
			string result;
			if (flag)
			{
				result = "Return-Path: <>\r\n";
			}
			else
			{
				result = "Return-Path: <" + this.m_Address + ">\r\n";
			}
			return result;
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06000F74 RID: 3956 RVA: 0x0005FBC0 File Offset: 0x0005EBC0
		public override bool IsModified
		{
			get
			{
				return this.m_IsModified;
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06000F75 RID: 3957 RVA: 0x0005FBD8 File Offset: 0x0005EBD8
		public override string Name
		{
			get
			{
				return "Return-Path";
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06000F76 RID: 3958 RVA: 0x0005FBF0 File Offset: 0x0005EBF0
		public string Address
		{
			get
			{
				return this.m_Address;
			}
		}

		// Token: 0x0400066B RID: 1643
		private bool m_IsModified = false;

		// Token: 0x0400066C RID: 1644
		private string m_Address = null;
	}
}
