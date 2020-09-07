using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x0200010A RID: 266
	public class MIME_h_Collection : IEnumerable
	{
		// Token: 0x06000A3F RID: 2623 RVA: 0x0003EBC8 File Offset: 0x0003DBC8
		public MIME_h_Collection(MIME_h_Provider provider)
		{
			bool flag = provider == null;
			if (flag)
			{
				throw new ArgumentNullException("provider");
			}
			this.m_pProvider = provider;
			this.m_pFields = new List<MIME_h>();
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x0003EC18 File Offset: 0x0003DC18
		public void Insert(int index, MIME_h field)
		{
			bool flag = index < 0 || index > this.m_pFields.Count;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			bool flag2 = field == null;
			if (flag2)
			{
				throw new ArgumentNullException("field");
			}
			this.m_pFields.Insert(index, field);
			this.m_IsModified = true;
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x0003EC74 File Offset: 0x0003DC74
		public MIME_h Add(string field)
		{
			bool flag = field == null;
			if (flag)
			{
				throw new ArgumentNullException("field");
			}
			MIME_h mime_h = this.m_pProvider.Parse(field);
			this.m_pFields.Add(mime_h);
			this.m_IsModified = true;
			return mime_h;
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x0003ECBC File Offset: 0x0003DCBC
		public void Add(MIME_h field)
		{
			bool flag = field == null;
			if (flag)
			{
				throw new ArgumentNullException("field");
			}
			this.m_pFields.Add(field);
			this.m_IsModified = true;
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x0003ECF4 File Offset: 0x0003DCF4
		public void Remove(MIME_h field)
		{
			bool flag = field == null;
			if (flag)
			{
				throw new ArgumentNullException("field");
			}
			this.m_pFields.Remove(field);
			this.m_IsModified = true;
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x0003ED2C File Offset: 0x0003DD2C
		public void RemoveAll(string name)
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
			foreach (MIME_h mime_h in this.m_pFields.ToArray())
			{
				bool flag3 = string.Compare(name, mime_h.Name, true) == 0;
				if (flag3)
				{
					this.m_pFields.Remove(mime_h);
				}
			}
			this.m_IsModified = true;
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x0003EDBE File Offset: 0x0003DDBE
		public void Clear()
		{
			this.m_pFields.Clear();
			this.m_IsModified = true;
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x0003EDD4 File Offset: 0x0003DDD4
		public bool Contains(string name)
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
			foreach (MIME_h mime_h in this.m_pFields.ToArray())
			{
				bool flag3 = string.Compare(name, mime_h.Name, true) == 0;
				if (flag3)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x0003EE5C File Offset: 0x0003DE5C
		public bool Contains(MIME_h field)
		{
			bool flag = field == null;
			if (flag)
			{
				throw new ArgumentNullException("field");
			}
			return this.m_pFields.Contains(field);
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x0003EE90 File Offset: 0x0003DE90
		public MIME_h GetFirst(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			foreach (MIME_h mime_h in this.m_pFields.ToArray())
			{
				bool flag2 = string.Equals(name, mime_h.Name, StringComparison.InvariantCultureIgnoreCase);
				if (flag2)
				{
					return mime_h;
				}
			}
			return null;
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x0003EEF4 File Offset: 0x0003DEF4
		public void ReplaceFirst(MIME_h field)
		{
			bool flag = field == null;
			if (flag)
			{
				throw new ArgumentNullException("field");
			}
			for (int i = 0; i < this.m_pFields.Count; i++)
			{
				bool flag2 = string.Equals(field.Name, this.m_pFields[i].Name, StringComparison.CurrentCultureIgnoreCase);
				if (flag2)
				{
					this.m_pFields.RemoveAt(i);
					this.m_pFields.Insert(i, field);
					break;
				}
			}
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x0003EF74 File Offset: 0x0003DF74
		public MIME_h[] ToArray()
		{
			return this.m_pFields.ToArray();
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x0003EF94 File Offset: 0x0003DF94
		public void ToFile(string fileName, MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			using (FileStream fileStream = File.Create(fileName))
			{
				this.ToStream(fileStream, wordEncoder, parmetersCharset);
			}
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x0003EFE8 File Offset: 0x0003DFE8
		public byte[] ToByte(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.ToStream(memoryStream, wordEncoder, parmetersCharset);
				memoryStream.Position = 0L;
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x0003F038 File Offset: 0x0003E038
		public void ToStream(Stream stream, MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset)
		{
			this.ToStream(stream, wordEncoder, parmetersCharset, false);
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x0003F048 File Offset: 0x0003E048
		public void ToStream(Stream stream, MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncod)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(this.ToString(wordEncoder, parmetersCharset, reEncod));
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x0003F08C File Offset: 0x0003E08C
		public override string ToString()
		{
			return this.ToString(null, null, false);
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x0003F0A8 File Offset: 0x0003E0A8
		public string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset)
		{
			return this.ToString(wordEncoder, parmetersCharset, false);
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x0003F0C4 File Offset: 0x0003E0C4
		public string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MIME_h mime_h in this.m_pFields)
			{
				stringBuilder.Append(mime_h.ToString(wordEncoder, parmetersCharset, reEncode));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x0003F138 File Offset: 0x0003E138
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new SmartStream(new MemoryStream(Encoding.UTF8.GetBytes(value)), true));
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x0003F178 File Offset: 0x0003E178
		public void Parse(SmartStream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.Parse(stream, Encoding.UTF8);
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x0003F1A8 File Offset: 0x0003E1A8
		public void Parse(SmartStream stream, Encoding encoding)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = encoding == null;
			if (flag2)
			{
				throw new ArgumentNullException("encoding");
			}
			StringBuilder stringBuilder = new StringBuilder();
			SmartStream.ReadLineAsyncOP readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[84000], SizeExceededAction.ThrowException);
			for (;;)
			{
				stream.ReadLine(readLineAsyncOP, false);
				bool flag3 = readLineAsyncOP.Error != null;
				if (flag3)
				{
					break;
				}
				bool flag4 = readLineAsyncOP.BytesInBuffer == 0;
				if (flag4)
				{
					goto Block_4;
				}
				bool flag5 = readLineAsyncOP.LineBytesInBuffer == 0;
				if (flag5)
				{
					goto Block_6;
				}
				string @string = encoding.GetString(readLineAsyncOP.Buffer, 0, readLineAsyncOP.BytesInBuffer);
				bool flag6 = stringBuilder.Length == 0;
				if (flag6)
				{
					stringBuilder.Append(@string);
				}
				else
				{
					bool flag7 = char.IsWhiteSpace(@string[0]);
					if (flag7)
					{
						stringBuilder.Append(@string);
					}
					else
					{
						this.Add(stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append(@string);
					}
				}
			}
			throw readLineAsyncOP.Error;
			Block_4:
			bool flag8 = stringBuilder.Length > 0;
			if (flag8)
			{
				this.Add(stringBuilder.ToString());
			}
			this.m_IsModified = false;
			return;
			Block_6:
			bool flag9 = stringBuilder.Length > 0;
			if (flag9)
			{
				this.Add(stringBuilder.ToString());
			}
			this.m_IsModified = false;
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x0003F308 File Offset: 0x0003E308
		public IEnumerator GetEnumerator()
		{
			return this.m_pFields.GetEnumerator();
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06000A56 RID: 2646 RVA: 0x0003F32C File Offset: 0x0003E32C
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
					foreach (MIME_h mime_h in this.m_pFields)
					{
						bool isModified2 = mime_h.IsModified;
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

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06000A57 RID: 2647 RVA: 0x0003F3A4 File Offset: 0x0003E3A4
		public int Count
		{
			get
			{
				return this.m_pFields.Count;
			}
		}

		// Token: 0x1700035F RID: 863
		public MIME_h this[int index]
		{
			get
			{
				bool flag = index < 0 || index >= this.m_pFields.Count;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.m_pFields[index];
			}
		}

		// Token: 0x17000360 RID: 864
		public MIME_h[] this[string name]
		{
			get
			{
				bool flag = name == null;
				if (flag)
				{
					throw new ArgumentNullException("name");
				}
				List<MIME_h> list = new List<MIME_h>();
				foreach (MIME_h mime_h in this.m_pFields.ToArray())
				{
					bool flag2 = string.Compare(name, mime_h.Name, true) == 0;
					if (flag2)
					{
						list.Add(mime_h);
					}
				}
				return list.ToArray();
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06000A5A RID: 2650 RVA: 0x0003F484 File Offset: 0x0003E484
		public MIME_h_Provider FieldsProvider
		{
			get
			{
				return this.m_pProvider;
			}
		}

		// Token: 0x0400045F RID: 1119
		private bool m_IsModified = false;

		// Token: 0x04000460 RID: 1120
		private MIME_h_Provider m_pProvider = null;

		// Token: 0x04000461 RID: 1121
		private List<MIME_h> m_pFields = null;
	}
}
