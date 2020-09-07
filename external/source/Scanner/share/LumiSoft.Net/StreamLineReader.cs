using System;
using System.IO;
using System.Text;

namespace LumiSoft.Net
{
	// Token: 0x02000023 RID: 35
	public class StreamLineReader
	{
		// Token: 0x0600010B RID: 267 RVA: 0x00007E70 File Offset: 0x00006E70
		public StreamLineReader(Stream strmSource)
		{
			this.m_StrmSource = strmSource;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00007EC0 File Offset: 0x00006EC0
		public byte[] ReadLine()
		{
			byte[] array = this.m_Buffer;
			int num = 0;
			int i = this.m_StrmSource.ReadByte();
			int num2 = this.m_StrmSource.ReadByte();
			while (i > -1)
			{
				bool flag = i == 13 && (byte)num2 == 10;
				byte[] result;
				if (flag)
				{
					byte[] array2 = new byte[num];
					Array.Copy(array, array2, num);
					result = array2;
				}
				else
				{
					bool flag2 = !this.m_CRLF_LinesOnly && num2 == 10;
					if (!flag2)
					{
						bool flag3 = num == array.Length;
						if (flag3)
						{
							byte[] array3 = new byte[array.Length + this.m_ReadBufferSize];
							Array.Copy(array, array3, array.Length);
							array = array3;
						}
						array[num] = (byte)i;
						num++;
						i = num2;
						num2 = this.m_StrmSource.ReadByte();
						continue;
					}
					byte[] array4 = new byte[num + 1];
					Array.Copy(array, array4, num + 1);
					array4[num] = (byte)i;
					result = array4;
				}
				return result;
			}
			bool flag4 = num > 0;
			if (flag4)
			{
				byte[] array5 = new byte[num];
				Array.Copy(array, array5, num);
				return array5;
			}
			return null;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00007FDC File Offset: 0x00006FDC
		public string ReadLineString()
		{
			byte[] array = this.ReadLine();
			bool flag = array != null;
			string result;
			if (flag)
			{
				bool flag2 = this.m_Encoding == null || this.m_Encoding == "";
				if (flag2)
				{
					result = System.Text.Encoding.Default.GetString(array);
				}
				else
				{
					result = System.Text.Encoding.GetEncoding(this.m_Encoding).GetString(array);
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00008044 File Offset: 0x00007044
		// (set) Token: 0x0600010F RID: 271 RVA: 0x0000805C File Offset: 0x0000705C
		public string Encoding
		{
			get
			{
				return this.m_Encoding;
			}
			set
			{
				System.Text.Encoding.GetEncoding(value);
				this.m_Encoding = value;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00008070 File Offset: 0x00007070
		// (set) Token: 0x06000111 RID: 273 RVA: 0x00008088 File Offset: 0x00007088
		public bool CRLF_LinesOnly
		{
			get
			{
				return this.m_CRLF_LinesOnly;
			}
			set
			{
				this.m_CRLF_LinesOnly = value;
			}
		}

		// Token: 0x0400006F RID: 111
		private Stream m_StrmSource = null;

		// Token: 0x04000070 RID: 112
		private string m_Encoding = "";

		// Token: 0x04000071 RID: 113
		private bool m_CRLF_LinesOnly = true;

		// Token: 0x04000072 RID: 114
		private int m_ReadBufferSize = 1024;

		// Token: 0x04000073 RID: 115
		private byte[] m_Buffer = new byte[1024];
	}
}
