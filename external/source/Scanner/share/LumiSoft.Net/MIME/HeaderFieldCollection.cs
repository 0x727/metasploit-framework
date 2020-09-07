using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.Mime
{
	// Token: 0x0200015F RID: 351
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class HeaderFieldCollection : IEnumerable
	{
		// Token: 0x06000E15 RID: 3605 RVA: 0x00057450 File Offset: 0x00056450
		public HeaderFieldCollection()
		{
			this.m_pHeaderFields = new List<HeaderField>();
		}

		// Token: 0x06000E16 RID: 3606 RVA: 0x0005746C File Offset: 0x0005646C
		public void Add(string fieldName, string value)
		{
			this.m_pHeaderFields.Add(new HeaderField(fieldName, value));
		}

		// Token: 0x06000E17 RID: 3607 RVA: 0x00057482 File Offset: 0x00056482
		public void Add(HeaderField headerField)
		{
			this.m_pHeaderFields.Add(headerField);
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x00057492 File Offset: 0x00056492
		public void Insert(int index, string fieldName, string value)
		{
			this.m_pHeaderFields.Insert(index, new HeaderField(fieldName, value));
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x000574A9 File Offset: 0x000564A9
		public void Remove(int index)
		{
			this.m_pHeaderFields.RemoveAt(index);
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x000574B9 File Offset: 0x000564B9
		public void Remove(HeaderField field)
		{
			this.m_pHeaderFields.Remove(field);
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x000574CC File Offset: 0x000564CC
		public void RemoveAll(string fieldName)
		{
			for (int i = 0; i < this.m_pHeaderFields.Count; i++)
			{
				HeaderField headerField = this.m_pHeaderFields[i];
				bool flag = headerField.Name.ToLower() == fieldName.ToLower();
				if (flag)
				{
					this.m_pHeaderFields.Remove(headerField);
					i--;
				}
			}
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x00057530 File Offset: 0x00056530
		public void Clear()
		{
			this.m_pHeaderFields.Clear();
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x00057540 File Offset: 0x00056540
		public bool Contains(string fieldName)
		{
			foreach (HeaderField headerField in this.m_pHeaderFields)
			{
				bool flag = headerField.Name.ToLower() == fieldName.ToLower();
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x000575B8 File Offset: 0x000565B8
		public bool Contains(HeaderField headerField)
		{
			return this.m_pHeaderFields.Contains(headerField);
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x000575D8 File Offset: 0x000565D8
		public HeaderField GetFirst(string fieldName)
		{
			foreach (HeaderField headerField in this.m_pHeaderFields)
			{
				bool flag = headerField.Name.ToLower() == fieldName.ToLower();
				if (flag)
				{
					return headerField;
				}
			}
			return null;
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x00057650 File Offset: 0x00056650
		public HeaderField[] Get(string fieldName)
		{
			ArrayList arrayList = new ArrayList();
			foreach (HeaderField headerField in this.m_pHeaderFields)
			{
				bool flag = headerField.Name.ToLower() == fieldName.ToLower();
				if (flag)
				{
					arrayList.Add(headerField);
				}
			}
			bool flag2 = arrayList.Count > 0;
			HeaderField[] result;
			if (flag2)
			{
				HeaderField[] array = new HeaderField[arrayList.Count];
				arrayList.CopyTo(array);
				result = array;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x00057700 File Offset: 0x00056700
		public void Parse(string headerString)
		{
			this.Parse(new MemoryStream(Encoding.Default.GetBytes(headerString)));
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x0005771A File Offset: 0x0005671A
		public void Parse(Stream stream)
		{
			this.Parse(new SmartStream(stream, false));
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x0005772C File Offset: 0x0005672C
		public void Parse(SmartStream stream)
		{
			this.m_pHeaderFields.Clear();
			SmartStream.ReadLineAsyncOP readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
			stream.ReadLine(readLineAsyncOP, false);
			bool flag = readLineAsyncOP.Error != null;
			if (flag)
			{
				throw readLineAsyncOP.Error;
			}
			string lineUtf = readLineAsyncOP.LineUtf8;
			while (lineUtf != null)
			{
				bool flag2 = lineUtf == "";
				if (flag2)
				{
					break;
				}
				string text = lineUtf;
				stream.ReadLine(readLineAsyncOP, false);
				bool flag3 = readLineAsyncOP.Error != null;
				if (flag3)
				{
					throw readLineAsyncOP.Error;
				}
				lineUtf = readLineAsyncOP.LineUtf8;
				while (lineUtf != null && (lineUtf.StartsWith("\t") || lineUtf.StartsWith(" ")))
				{
					text += lineUtf;
					stream.ReadLine(readLineAsyncOP, false);
					bool flag4 = readLineAsyncOP.Error != null;
					if (flag4)
					{
						throw readLineAsyncOP.Error;
					}
					lineUtf = readLineAsyncOP.LineUtf8;
				}
				string[] array = text.Split(new char[]
				{
					':'
				}, 2);
				bool flag5 = array.Length == 2;
				if (flag5)
				{
					this.Add(array[0] + ":", array[1].Trim());
				}
			}
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x0005786C File Offset: 0x0005686C
		public string ToHeaderString(string encodingCharSet)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in this)
			{
				HeaderField headerField = (HeaderField)obj;
				stringBuilder.Append(headerField.Name + " " + headerField.EncodedValue + "\r\n");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x000578F4 File Offset: 0x000568F4
		public IEnumerator GetEnumerator()
		{
			return this.m_pHeaderFields.GetEnumerator();
		}

		// Token: 0x170004B0 RID: 1200
		public HeaderField this[int index]
		{
			get
			{
				return this.m_pHeaderFields[index];
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06000E27 RID: 3623 RVA: 0x00057938 File Offset: 0x00056938
		public int Count
		{
			get
			{
				return this.m_pHeaderFields.Count;
			}
		}

		// Token: 0x040005EF RID: 1519
		private List<HeaderField> m_pHeaderFields = null;
	}
}
