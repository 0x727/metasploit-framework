using System;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x02000171 RID: 369
	public class Item
	{
		// Token: 0x06000EF4 RID: 3828 RVA: 0x0005D2E8 File Offset: 0x0005C2E8
		internal Item(vCard card, string name, string parameters, string value)
		{
			this.m_pCard = card;
			this.m_Name = name;
			this.m_Parameters = parameters;
			this.m_Value = value;
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x0005D34C File Offset: 0x0005C34C
		public void SetDecodedValue(string value)
		{
			string text = "";
			string[] array = this.m_Parameters.ToLower().Split(new char[]
			{
				';'
			});
			foreach (string text2 in array)
			{
				string[] array3 = text2.Split(new char[]
				{
					'='
				});
				bool flag = array3[0] == "encoding" || array3[0] == "charset";
				if (!flag)
				{
					bool flag2 = text2.Length > 0;
					if (flag2)
					{
						text = text + text2 + ";";
					}
				}
			}
			bool flag3 = this.m_pCard.Version.StartsWith("3");
			if (flag3)
			{
				bool flag4 = !Net_Utils.IsAscii(value);
				if (flag4)
				{
					text += "CHARSET=utf-8";
				}
				this.ParametersString = text;
				this.Value = vCard_Utils.Encode(this.m_pCard.Version, this.m_pCard.Charset, value);
			}
			else
			{
				bool flag5 = this.NeedEncode(value);
				if (flag5)
				{
					text = text + "ENCODING=QUOTED-PRINTABLE;CHARSET=" + this.m_pCard.Charset.WebName;
					this.ParametersString = text;
					this.Value = vCard_Utils.Encode(this.m_pCard.Version, this.m_pCard.Charset, value);
				}
				else
				{
					this.ParametersString = text;
					this.Value = value;
				}
			}
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x0005D4C8 File Offset: 0x0005C4C8
		internal string ToItemString()
		{
			string text = this.m_Value;
			bool foldData = this.m_FoldData;
			if (foldData)
			{
				text = this.FoldData(text);
			}
			bool flag = this.m_Parameters.Length > 0;
			string result;
			if (flag)
			{
				result = string.Concat(new string[]
				{
					this.m_Name,
					";",
					this.m_Parameters,
					":",
					text
				});
			}
			else
			{
				result = this.m_Name + ":" + text;
			}
			return result;
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x0005D550 File Offset: 0x0005C550
		private bool NeedEncode(string value)
		{
			bool flag = !Net_Utils.IsAscii(value);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (char c in value)
				{
					bool flag2 = char.IsControl(c);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x0005D5A8 File Offset: 0x0005C5A8
		private string FoldData(string data)
		{
			bool flag = data.Length > 76;
			string result;
			if (flag)
			{
				int num = 0;
				int num2 = -1;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < data.Length; i++)
				{
					char c = data[i];
					bool flag2 = c == ' ' || c == '\t';
					if (flag2)
					{
						num2 = i;
					}
					bool flag3 = i == data.Length - 1;
					if (flag3)
					{
						stringBuilder.Append(data.Substring(num));
					}
					else
					{
						bool flag4 = i - num >= 76;
						if (flag4)
						{
							bool flag5 = num2 == -1;
							if (flag5)
							{
								num2 = i;
							}
							stringBuilder.Append(data.Substring(num, num2 - num) + "\r\n\t");
							i = num2;
							num2 = -1;
							num = i;
						}
					}
				}
				result = stringBuilder.ToString();
			}
			else
			{
				result = data;
			}
			return result;
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x0005D694 File Offset: 0x0005C694
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06000EFA RID: 3834 RVA: 0x0005D6AC File Offset: 0x0005C6AC
		// (set) Token: 0x06000EFB RID: 3835 RVA: 0x0005D6C4 File Offset: 0x0005C6C4
		public string ParametersString
		{
			get
			{
				return this.m_Parameters;
			}
			set
			{
				this.m_Parameters = value;
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06000EFC RID: 3836 RVA: 0x0005D6D0 File Offset: 0x0005C6D0
		// (set) Token: 0x06000EFD RID: 3837 RVA: 0x0005D6E8 File Offset: 0x0005C6E8
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

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06000EFE RID: 3838 RVA: 0x0005D6F4 File Offset: 0x0005C6F4
		public string DecodedValue
		{
			get
			{
				string text = this.m_Value;
				string text2 = null;
				string text3 = null;
				string[] array = this.m_Parameters.ToLower().Split(new char[]
				{
					';'
				});
				foreach (string text4 in array)
				{
					string[] array3 = text4.Split(new char[]
					{
						'='
					});
					bool flag = array3[0] == "encoding" && array3.Length > 1;
					if (flag)
					{
						text2 = array3[1];
					}
					else
					{
						bool flag2 = array3[0] == "charset" && array3.Length > 1;
						if (flag2)
						{
							text3 = array3[1];
						}
					}
				}
				bool flag3 = text2 != null;
				if (flag3)
				{
					bool flag4 = text2 == "quoted-printable";
					if (flag4)
					{
						text = Encoding.Default.GetString(MIME_Utils.QuotedPrintableDecode(Encoding.Default.GetBytes(text)));
					}
					else
					{
						bool flag5 = text2 == "b" || text2 == "base64";
						if (!flag5)
						{
							throw new Exception("Unknown data encoding '" + text2 + "' !");
						}
						text = Encoding.Default.GetString(Net_Utils.FromBase64(Encoding.Default.GetBytes(text)));
					}
				}
				bool flag6 = text3 != null;
				if (flag6)
				{
					text = Encoding.GetEncoding(text3).GetString(Encoding.Default.GetBytes(text));
				}
				return text;
			}
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06000EFF RID: 3839 RVA: 0x0005D870 File Offset: 0x0005C870
		// (set) Token: 0x06000F00 RID: 3840 RVA: 0x0005D888 File Offset: 0x0005C888
		public bool FoldLongLines
		{
			get
			{
				return this.m_FoldData;
			}
			set
			{
				this.m_FoldData = value;
			}
		}

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06000F01 RID: 3841 RVA: 0x0005D894 File Offset: 0x0005C894
		internal vCard Owner
		{
			get
			{
				return this.m_pCard;
			}
		}

		// Token: 0x04000637 RID: 1591
		private vCard m_pCard = null;

		// Token: 0x04000638 RID: 1592
		private string m_Name = "";

		// Token: 0x04000639 RID: 1593
		private string m_Parameters = "";

		// Token: 0x0400063A RID: 1594
		private string m_Value = "";

		// Token: 0x0400063B RID: 1595
		private bool m_FoldData = true;
	}
}
