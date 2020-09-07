using System;
using System.Collections.Generic;

namespace LumiSoft.Net.MIME
{
	// Token: 0x0200010D RID: 269
	public class MIME_h_Provider
	{
		// Token: 0x06000A7E RID: 2686 RVA: 0x0003FDD0 File Offset: 0x0003EDD0
		public MIME_h_Provider()
		{
			this.m_pDefaultHeaderField = typeof(MIME_h_Unstructured);
			this.m_pHeadrFields = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);
			this.m_pHeadrFields.Add("Content-Type", typeof(MIME_h_ContentType));
			this.m_pHeadrFields.Add("Content-Disposition", typeof(MIME_h_ContentDisposition));
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x0003FE4C File Offset: 0x0003EE4C
		public MIME_h Parse(string field)
		{
			bool flag = field == null;
			if (flag)
			{
				throw new ArgumentNullException("field");
			}
			bool flag2 = !field.EndsWith("\r\n");
			if (flag2)
			{
				field += "\r\n";
			}
			MIME_h result = null;
			string[] array = field.Split(new char[]
			{
				':'
			}, 2);
			string text = array[0].Trim();
			bool flag3 = text == string.Empty;
			if (flag3)
			{
				throw new ParseException("Invalid header field value '" + field + "'.");
			}
			try
			{
				bool flag4 = this.m_pHeadrFields.ContainsKey(text);
				if (flag4)
				{
					result = (MIME_h)this.m_pHeadrFields[text].GetMethod("Parse").Invoke(null, new object[]
					{
						field
					});
				}
				else
				{
					result = (MIME_h)this.m_pDefaultHeaderField.GetMethod("Parse").Invoke(null, new object[]
					{
						field
					});
				}
			}
			catch (Exception ex)
			{
				result = new MIME_h_Unparsed(field, ex.InnerException);
			}
			return result;
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06000A80 RID: 2688 RVA: 0x0003FF74 File Offset: 0x0003EF74
		// (set) Token: 0x06000A81 RID: 2689 RVA: 0x0003FF8C File Offset: 0x0003EF8C
		public Type DefaultHeaderField
		{
			get
			{
				return this.m_pDefaultHeaderField;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("DefaultHeaderField");
				}
				bool flag2 = !value.GetType().IsSubclassOf(typeof(MIME_h));
				if (flag2)
				{
					throw new ArgumentException("Property 'DefaultHeaderField' value must be based on MIME_h class.");
				}
				this.m_pDefaultHeaderField = value;
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06000A82 RID: 2690 RVA: 0x0003FFE0 File Offset: 0x0003EFE0
		public Dictionary<string, Type> HeaderFields
		{
			get
			{
				return this.m_pHeadrFields;
			}
		}

		// Token: 0x0400046B RID: 1131
		private Type m_pDefaultHeaderField = null;

		// Token: 0x0400046C RID: 1132
		private Dictionary<string, Type> m_pHeadrFields = null;
	}
}
