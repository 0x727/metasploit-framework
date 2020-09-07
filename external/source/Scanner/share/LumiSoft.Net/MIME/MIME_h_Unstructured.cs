using System;
using System.Text;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000111 RID: 273
	public class MIME_h_Unstructured : MIME_h
	{
		// Token: 0x06000AA0 RID: 2720 RVA: 0x00040A30 File Offset: 0x0003FA30
		public MIME_h_Unstructured(string name, string value)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'name' value must be specified.", "name");
			}
			bool flag3 = value == null;
			if (flag3)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Name = name;
			this.m_Value = value;
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00040AB8 File Offset: 0x0003FAB8
		private MIME_h_Unstructured()
		{
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00040AE0 File Offset: 0x0003FAE0
		public static MIME_h_Unstructured Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			MIME_h_Unstructured mime_h_Unstructured = new MIME_h_Unstructured();
			string[] array = value.Split(new char[]
			{
				':'
			}, 2);
			bool flag2 = array[0].Trim() == string.Empty;
			if (flag2)
			{
				throw new ParseException("Invalid header field '" + value + "' syntax.");
			}
			mime_h_Unstructured.m_Name = array[0];
			mime_h_Unstructured.m_Value = MIME_Encoding_EncodedWord.DecodeTextS(MIME_Utils.UnfoldHeader((array.Length == 2) ? array[1].TrimStart(new char[0]) : ""));
			mime_h_Unstructured.m_ParseValue = value;
			return mime_h_Unstructured;
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x00040B8C File Offset: 0x0003FB8C
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			bool flag = !reEncode && this.m_ParseValue != null;
			string result;
			if (flag)
			{
				result = this.m_ParseValue;
			}
			else
			{
				bool flag2 = wordEncoder != null;
				if (flag2)
				{
					result = this.m_Name + ": " + wordEncoder.Encode(this.m_Value) + "\r\n";
				}
				else
				{
					result = this.m_Name + ": " + this.m_Value + "\r\n";
				}
			}
			return result;
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06000AA4 RID: 2724 RVA: 0x00040C08 File Offset: 0x0003FC08
		public override bool IsModified
		{
			get
			{
				return this.m_ParseValue == null;
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06000AA5 RID: 2725 RVA: 0x00040C24 File Offset: 0x0003FC24
		public override string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06000AA6 RID: 2726 RVA: 0x00040C3C File Offset: 0x0003FC3C
		// (set) Token: 0x06000AA7 RID: 2727 RVA: 0x00040C54 File Offset: 0x0003FC54
		public string Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_Value = value;
				this.m_ParseValue = null;
			}
		}

		// Token: 0x04000478 RID: 1144
		private string m_ParseValue = null;

		// Token: 0x04000479 RID: 1145
		private string m_Name = "";

		// Token: 0x0400047A RID: 1146
		private string m_Value = "";
	}
}
