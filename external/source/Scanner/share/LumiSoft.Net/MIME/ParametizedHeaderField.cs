using System;
using System.Collections;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000169 RID: 361
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class ParametizedHeaderField
	{
		// Token: 0x06000EBD RID: 3773 RVA: 0x0005C1E4 File Offset: 0x0005B1E4
		public ParametizedHeaderField(HeaderField headerField)
		{
			this.m_pHeaderField = headerField;
			this.m_pParameters = new HeaderFieldParameterCollection(this);
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x0005C210 File Offset: 0x0005B210
		internal Hashtable ParseParameters()
		{
			string[] array = TextUtils.SplitQuotedString(this.m_pHeaderField.EncodedValue, ';');
			Hashtable hashtable = new Hashtable();
			for (int i = 1; i < array.Length; i++)
			{
				string[] array2 = array[i].Trim().Split(new char[]
				{
					'='
				}, 2);
				bool flag = !hashtable.ContainsKey(array2[0].ToLower());
				if (flag)
				{
					bool flag2 = array2.Length == 2;
					if (flag2)
					{
						string text = array2[1];
						bool flag3 = text.StartsWith("\"");
						if (flag3)
						{
							text = TextUtils.UnQuoteString(array2[1]);
						}
						hashtable.Add(array2[0].ToLower(), text);
					}
					else
					{
						hashtable.Add(array2[0].ToLower(), "");
					}
				}
			}
			return hashtable;
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x0005C2E8 File Offset: 0x0005B2E8
		internal void StoreParameters(string value, Hashtable parameters)
		{
			string text = value;
			foreach (object obj in parameters)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				text = string.Concat(new object[]
				{
					text,
					";\t",
					dictionaryEntry.Key,
					"=\"",
					dictionaryEntry.Value,
					"\""
				});
			}
			this.m_pHeaderField.Value = text;
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06000EC0 RID: 3776 RVA: 0x0005C388 File Offset: 0x0005B388
		public string Name
		{
			get
			{
				return this.m_pHeaderField.Name;
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06000EC1 RID: 3777 RVA: 0x0005C3A8 File Offset: 0x0005B3A8
		// (set) Token: 0x06000EC2 RID: 3778 RVA: 0x0005C3CE File Offset: 0x0005B3CE
		public string Value
		{
			get
			{
				return TextUtils.SplitQuotedString(this.m_pHeaderField.Value, ';')[0];
			}
			set
			{
				this.StoreParameters(value, this.ParseParameters());
			}
		}

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x0005C3E0 File Offset: 0x0005B3E0
		public HeaderFieldParameterCollection Parameters
		{
			get
			{
				return this.m_pParameters;
			}
		}

		// Token: 0x04000617 RID: 1559
		private HeaderField m_pHeaderField = null;

		// Token: 0x04000618 RID: 1560
		private HeaderFieldParameterCollection m_pParameters = null;
	}
}
