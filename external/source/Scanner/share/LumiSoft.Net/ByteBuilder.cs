using System;
using System.Text;

namespace LumiSoft.Net
{
	// Token: 0x02000003 RID: 3
	public class ByteBuilder
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00001050
		public ByteBuilder()
		{
			this.m_pBuffer = new byte[this.m_BlockSize];
			this.m_pCharset = Encoding.UTF8;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020A4 File Offset: 0x000010A4
		public void Append(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Append(this.m_pCharset.GetBytes(value));
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020DC File Offset: 0x000010DC
		public void Append(Encoding charset, string value)
		{
			bool flag = charset == null;
			if (flag)
			{
				throw new ArgumentNullException("charset");
			}
			bool flag2 = value == null;
			if (flag2)
			{
				throw new ArgumentNullException("value");
			}
			this.Append(charset.GetBytes(value));
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002120 File Offset: 0x00001120
		public void Append(byte[] value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Append(value, 0, value.Length);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002150 File Offset: 0x00001150
		public void Append(byte[] value, int offset, int count)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			while (this.m_pBuffer.Length - this.m_Count < count)
			{
				byte[] array = new byte[this.m_pBuffer.Length + this.m_BlockSize];
				Array.Copy(this.m_pBuffer, array, this.m_Count);
				this.m_pBuffer = array;
			}
			Array.Copy(value, offset, this.m_pBuffer, this.m_Count, count);
			this.m_Count += value.Length;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021E0 File Offset: 0x000011E0
		public byte[] ToByte()
		{
			byte[] array = new byte[this.m_Count];
			Array.Copy(this.m_pBuffer, array, this.m_Count);
			return array;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002214 File Offset: 0x00001214
		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000222C File Offset: 0x0000122C
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002244 File Offset: 0x00001244
		public Encoding Charset
		{
			get
			{
				return this.m_pCharset;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_pCharset = value;
			}
		}

		// Token: 0x04000006 RID: 6
		private int m_BlockSize = 1024;

		// Token: 0x04000007 RID: 7
		private byte[] m_pBuffer = null;

		// Token: 0x04000008 RID: 8
		private int m_Count = 0;

		// Token: 0x04000009 RID: 9
		private Encoding m_pCharset = null;
	}
}
