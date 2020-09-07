using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LumiSoft.Net.MIME
{
	// Token: 0x0200010F RID: 271
	public class MIME_h_ParameterCollection : IEnumerable
	{
		// Token: 0x06000A88 RID: 2696 RVA: 0x000400A8 File Offset: 0x0003F0A8
		public MIME_h_ParameterCollection(MIME_h owner)
		{
			bool flag = owner == null;
			if (flag)
			{
				throw new ArgumentNullException("owner");
			}
			this.m_pOwner = owner;
			this.m_pParameters = new Dictionary<string, MIME_h_Parameter>(StringComparer.CurrentCultureIgnoreCase);
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x00040104 File Offset: 0x0003F104
		public void Remove(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = this.m_pParameters.Remove(name);
			if (flag2)
			{
				this.m_IsModified = true;
			}
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x0004013F File Offset: 0x0003F13F
		public void Clear()
		{
			this.m_pParameters.Clear();
			this.m_IsModified = true;
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x00040158 File Offset: 0x0003F158
		public MIME_h_Parameter[] ToArray()
		{
			MIME_h_Parameter[] array = new MIME_h_Parameter[this.m_pParameters.Count];
			this.m_pParameters.Values.CopyTo(array, 0);
			return array;
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x00040190 File Offset: 0x0003F190
		public override string ToString()
		{
			return this.ToString(null);
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x000401AC File Offset: 0x0003F1AC
		public string ToString(Encoding charset)
		{
			bool flag = charset == null;
			if (flag)
			{
				charset = Encoding.Default;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MIME_h_Parameter mime_h_Parameter in this.ToArray())
			{
				bool flag2 = string.IsNullOrEmpty(mime_h_Parameter.Value);
				if (flag2)
				{
					stringBuilder.Append(";\r\n\t" + mime_h_Parameter.Name);
				}
				else
				{
					bool flag3 = (charset == null || Net_Utils.IsAscii(mime_h_Parameter.Value)) && mime_h_Parameter.Value.Length < 76;
					if (flag3)
					{
						stringBuilder.Append(";\r\n\t" + mime_h_Parameter.Name + "=" + TextUtils.QuoteString(mime_h_Parameter.Value));
					}
					else
					{
						bool encodeRfc = this.m_EncodeRfc2047;
						if (encodeRfc)
						{
							stringBuilder.Append(";\r\n\t" + mime_h_Parameter.Name + "=" + TextUtils.QuoteString(MIME_Encoding_EncodedWord.EncodeS(MIME_EncodedWordEncoding.B, Encoding.UTF8, false, mime_h_Parameter.Value)));
						}
						else
						{
							byte[] bytes = charset.GetBytes(mime_h_Parameter.Value);
							List<string> list = new List<string>();
							int num = 0;
							char[] array2 = new char[50];
							foreach (byte b in bytes)
							{
								bool flag4 = num >= 47;
								if (flag4)
								{
									list.Add(new string(array2, 0, num));
									num = 0;
								}
								bool flag5 = MIME_Reader.IsAttributeChar((char)b);
								if (flag5)
								{
									array2[num++] = (char)b;
								}
								else
								{
									array2[num++] = '%';
									array2[num++] = (b >> 4).ToString("X")[0];
									array2[num++] = ((int)(b & 15)).ToString("X")[0];
								}
							}
							bool flag6 = num > 0;
							if (flag6)
							{
								list.Add(new string(array2, 0, num));
							}
							for (int k = 0; k < list.Count; k++)
							{
								bool flag7 = charset != null && k == 0;
								if (flag7)
								{
									stringBuilder.Append(string.Concat(new string[]
									{
										";\r\n\t",
										mime_h_Parameter.Name,
										"*",
										k.ToString(),
										"*=",
										charset.WebName,
										"''",
										list[k]
									}));
								}
								else
								{
									stringBuilder.Append(string.Concat(new string[]
									{
										";\r\n\t",
										mime_h_Parameter.Name,
										"*",
										k.ToString(),
										"*=",
										list[k]
									}));
								}
							}
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x000404B4 File Offset: 0x0003F4B4
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new MIME_Reader(value));
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x000404E4 File Offset: 0x0003F4E4
		public void Parse(MIME_Reader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			KeyValueCollection<string, MIME_h_ParameterCollection._ParameterBuilder> keyValueCollection = new KeyValueCollection<string, MIME_h_ParameterCollection._ParameterBuilder>();
			string[] array = TextUtils.SplitQuotedString(reader.ToEnd(), ';');
			foreach (string text in array)
			{
				bool flag2 = string.IsNullOrEmpty(text);
				if (!flag2)
				{
					string[] array3 = text.Trim().Split(new char[]
					{
						'='
					}, 2);
					string text2 = array3[0].Trim();
					string value = null;
					bool flag3 = array3.Length == 2;
					if (flag3)
					{
						value = TextUtils.UnQuoteString(MIME_Utils.UnfoldHeader(array3[1].Trim()));
					}
					string[] array4 = text2.Split(new char[]
					{
						'*'
					});
					int index = 0;
					bool encoded = array4.Length == 3;
					bool flag4 = array4.Length >= 2;
					if (flag4)
					{
						try
						{
							index = Convert.ToInt32(array4[1]);
						}
						catch
						{
						}
					}
					bool flag5 = array4.Length < 2 && keyValueCollection.ContainsKey(array4[0]);
					if (!flag5)
					{
						bool flag6 = !keyValueCollection.ContainsKey(array4[0]);
						if (flag6)
						{
							keyValueCollection.Add(array4[0], new MIME_h_ParameterCollection._ParameterBuilder(array4[0]));
						}
						keyValueCollection[array4[0]].AddPart(index, encoded, value);
					}
				}
			}
			foreach (object obj in keyValueCollection)
			{
				MIME_h_ParameterCollection._ParameterBuilder parameterBuilder = (MIME_h_ParameterCollection._ParameterBuilder)obj;
				this.m_pParameters.Add(parameterBuilder.Name, parameterBuilder.GetParamter());
			}
			this.m_IsModified = false;
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x000406B8 File Offset: 0x0003F6B8
		private static string DecodeExtOctet(string text, Encoding charset)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			bool flag2 = charset == null;
			if (flag2)
			{
				throw new ArgumentNullException("charset");
			}
			int count = 0;
			byte[] array = new byte[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				bool flag3 = text[i] == '%';
				if (flag3)
				{
					array[count++] = byte.Parse(text[i + 1].ToString() + text[i + 2].ToString(), NumberStyles.HexNumber);
					i += 2;
				}
				else
				{
					array[count++] = (byte)text[i];
				}
			}
			return charset.GetString(array, 0, count);
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x00040798 File Offset: 0x0003F798
		public IEnumerator GetEnumerator()
		{
			return this.m_pParameters.Values.GetEnumerator();
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06000A92 RID: 2706 RVA: 0x000407C0 File Offset: 0x0003F7C0
		public bool IsModified
		{
			get
			{
				bool isModified = this.m_IsModified;
				bool result;
				if (isModified)
				{
					result = true;
				}
				else
				{
					foreach (MIME_h_Parameter mime_h_Parameter in this.ToArray())
					{
						bool isModified2 = mime_h_Parameter.IsModified;
						if (isModified2)
						{
							return true;
						}
					}
					result = false;
				}
				return result;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06000A93 RID: 2707 RVA: 0x00040814 File Offset: 0x0003F814
		public MIME_h Owner
		{
			get
			{
				return this.m_pOwner;
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06000A94 RID: 2708 RVA: 0x0004082C File Offset: 0x0003F82C
		public int Count
		{
			get
			{
				return this.m_pParameters.Count;
			}
		}

		// Token: 0x1700037D RID: 893
		public string this[string name]
		{
			get
			{
				bool flag = name == null;
				if (flag)
				{
					throw new ArgumentNullException("name");
				}
				MIME_h_Parameter mime_h_Parameter = null;
				bool flag2 = this.m_pParameters.TryGetValue(name, out mime_h_Parameter);
				string result;
				if (flag2)
				{
					result = mime_h_Parameter.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = name == null;
				if (flag)
				{
					throw new ArgumentNullException("name");
				}
				MIME_h_Parameter mime_h_Parameter = null;
				bool flag2 = this.m_pParameters.TryGetValue(name, out mime_h_Parameter);
				if (flag2)
				{
					mime_h_Parameter.Value = value;
				}
				else
				{
					this.m_pParameters.Add(name, new MIME_h_Parameter(name, value));
				}
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06000A97 RID: 2711 RVA: 0x000408EC File Offset: 0x0003F8EC
		// (set) Token: 0x06000A98 RID: 2712 RVA: 0x00040904 File Offset: 0x0003F904
		public bool EncodeRfc2047
		{
			get
			{
				return this.m_EncodeRfc2047;
			}
			set
			{
				this.m_EncodeRfc2047 = value;
			}
		}

		// Token: 0x04000470 RID: 1136
		private bool m_IsModified = false;

		// Token: 0x04000471 RID: 1137
		private MIME_h m_pOwner = null;

		// Token: 0x04000472 RID: 1138
		private Dictionary<string, MIME_h_Parameter> m_pParameters = null;

		// Token: 0x04000473 RID: 1139
		private bool m_EncodeRfc2047 = false;

		// Token: 0x020002C1 RID: 705
		public class _ParameterBuilder
		{
			// Token: 0x0600184E RID: 6222 RVA: 0x000966A8 File Offset: 0x000956A8
			public _ParameterBuilder(string name)
			{
				bool flag = name == null;
				if (flag)
				{
					throw new ArgumentNullException("name");
				}
				this.m_Name = name;
				this.m_pParts = new SortedList<int, string>();
			}

			// Token: 0x0600184F RID: 6223 RVA: 0x000966F8 File Offset: 0x000956F8
			public void AddPart(int index, bool encoded, string value)
			{
				bool flag = encoded && index == 0;
				if (flag)
				{
					string[] array = value.Split(new char[]
					{
						'\''
					});
					this.m_pEncoding = Encoding.GetEncoding(string.IsNullOrEmpty(array[0]) ? "us-ascii" : array[0]);
					value = array[2];
				}
				this.m_pParts[index] = value;
			}

			// Token: 0x06001850 RID: 6224 RVA: 0x0009675C File Offset: 0x0009575C
			public MIME_h_Parameter GetParamter()
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<int, string> keyValuePair in this.m_pParts)
				{
					stringBuilder.Append(keyValuePair.Value);
				}
				bool flag = this.m_pEncoding != null;
				MIME_h_Parameter result;
				if (flag)
				{
					result = new MIME_h_Parameter(this.m_Name, MIME_h_ParameterCollection.DecodeExtOctet(stringBuilder.ToString(), this.m_pEncoding));
				}
				else
				{
					result = new MIME_h_Parameter(this.m_Name, stringBuilder.ToString());
				}
				return result;
			}

			// Token: 0x170007E9 RID: 2025
			// (get) Token: 0x06001851 RID: 6225 RVA: 0x00096804 File Offset: 0x00095804
			public string Name
			{
				get
				{
					return this.m_Name;
				}
			}

			// Token: 0x04000A3C RID: 2620
			private string m_Name = null;

			// Token: 0x04000A3D RID: 2621
			private SortedList<int, string> m_pParts = null;

			// Token: 0x04000A3E RID: 2622
			private Encoding m_pEncoding = null;
		}
	}
}
