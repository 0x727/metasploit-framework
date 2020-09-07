using System;

namespace LumiSoft.Net.IO
{
	// Token: 0x0200011C RID: 284
	public class FifoBuffer
	{
		// Token: 0x06000B2A RID: 2858 RVA: 0x00044BA8 File Offset: 0x00043BA8
		public FifoBuffer(int maxSize)
		{
			bool flag = maxSize < 1;
			if (flag)
			{
				throw new ArgumentException("Argument 'maxSize' value must be >= 1.");
			}
			this.m_pBuffer = new byte[maxSize];
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x00044C00 File Offset: 0x00043C00
		public int Read(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' value must be >= 0.");
			}
			bool flag4 = offset + count > buffer.Length;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' is bigger than than argument 'buffer' can store.");
			}
			object pLock = this.m_pLock;
			int result;
			lock (pLock)
			{
				int num = Math.Min(count, this.m_WriteOffset - this.m_ReadOffset);
				bool flag6 = num > 0;
				if (flag6)
				{
					Array.Copy(this.m_pBuffer, this.m_ReadOffset, buffer, offset, num);
					this.m_ReadOffset += num;
				}
				result = num;
			}
			return result;
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x00044CF4 File Offset: 0x00043CF4
		public void Write(byte[] buffer, int offset, int count, bool ignoreBufferFull)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
			}
			bool flag3 = count < 0 || count + offset > buffer.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			object pLock = this.m_pLock;
			lock (pLock)
			{
				int num = this.m_pBuffer.Length - this.m_WriteOffset;
				bool flag5 = num < count;
				if (flag5)
				{
					this.TrimStart();
					num = this.m_pBuffer.Length - this.m_WriteOffset;
					bool flag6 = num >= count;
					if (flag6)
					{
						Array.Copy(buffer, offset, this.m_pBuffer, this.m_WriteOffset, count);
						this.m_WriteOffset += count;
					}
					else
					{
						bool flag7 = !ignoreBufferFull;
						if (flag7)
						{
							throw new DataSizeExceededException();
						}
					}
				}
				else
				{
					Array.Copy(buffer, offset, this.m_pBuffer, this.m_WriteOffset, count);
					this.m_WriteOffset += count;
				}
			}
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x00044E28 File Offset: 0x00043E28
		public void Clear()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				this.m_ReadOffset = 0;
				this.m_WriteOffset = 0;
			}
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x00044E78 File Offset: 0x00043E78
		private void TrimStart()
		{
			bool flag = this.m_ReadOffset > 0;
			if (flag)
			{
				byte[] array = new byte[this.Available];
				Array.Copy(this.m_pBuffer, this.m_ReadOffset, array, 0, array.Length);
				Array.Copy(array, this.m_pBuffer, array.Length);
				this.m_ReadOffset = 0;
				this.m_WriteOffset = array.Length;
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06000B2F RID: 2863 RVA: 0x00044ED8 File Offset: 0x00043ED8
		public int MaxSize
		{
			get
			{
				return this.m_pBuffer.Length;
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06000B30 RID: 2864 RVA: 0x00044EF4 File Offset: 0x00043EF4
		public int Available
		{
			get
			{
				return this.m_WriteOffset - this.m_ReadOffset;
			}
		}

		// Token: 0x04000495 RID: 1173
		private object m_pLock = new object();

		// Token: 0x04000496 RID: 1174
		private byte[] m_pBuffer = null;

		// Token: 0x04000497 RID: 1175
		private int m_ReadOffset = 0;

		// Token: 0x04000498 RID: 1176
		private int m_WriteOffset = 0;
	}
}
