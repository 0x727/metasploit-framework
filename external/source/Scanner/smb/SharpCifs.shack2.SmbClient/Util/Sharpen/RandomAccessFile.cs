using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200005F RID: 95
	public class RandomAccessFile
	{
		// Token: 0x06000286 RID: 646 RVA: 0x0000AEA0 File Offset: 0x000090A0
		public RandomAccessFile(FilePath file, string mode) : this(file.GetPath(), mode)
		{
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000AEB4 File Offset: 0x000090B4
		public RandomAccessFile(string file, string mode)
		{
			bool flag = mode.IndexOf('w') != -1;
			if (flag)
			{
				this._stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			}
			else
			{
				this._stream = new FileStream(file, FileMode.Open, FileAccess.Read);
			}
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000AEF9 File Offset: 0x000090F9
		public void Close()
		{
			this._stream.Close();
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000AF08 File Offset: 0x00009108
		public long GetFilePointer()
		{
			return this._stream.Position;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000AF28 File Offset: 0x00009128
		public long Length()
		{
			return this._stream.Length;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000AF48 File Offset: 0x00009148
		public int Read(byte[] buffer)
		{
			int num = this._stream.Read(buffer, 0, buffer.Length);
			return (num > 0) ? num : -1;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000AF74 File Offset: 0x00009174
		public int Read(byte[] buffer, int start, int size)
		{
			return this._stream.Read(buffer, start, size);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0000AF94 File Offset: 0x00009194
		public void ReadFully(byte[] buffer, int start, int size)
		{
			while (size > 0)
			{
				int num = this._stream.Read(buffer, start, size);
				bool flag = num == 0;
				if (flag)
				{
					throw new EofException();
				}
				size -= num;
				start += num;
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000AFD6 File Offset: 0x000091D6
		public void Seek(long pos)
		{
			this._stream.Position = pos;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000AFE6 File Offset: 0x000091E6
		public void SetLength(long len)
		{
			this._stream.SetLength(len);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000AFF6 File Offset: 0x000091F6
		public void Write(int value)
		{
			this._stream.Write(BitConverter.GetBytes(value), 0, 4);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000B00D File Offset: 0x0000920D
		public void Write(byte[] buffer)
		{
			this._stream.Write(buffer, 0, buffer.Length);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000B021 File Offset: 0x00009221
		public void Write(byte[] buffer, int start, int size)
		{
			this._stream.Write(buffer, start, size);
		}

		// Token: 0x0400007C RID: 124
		private FileStream _stream;
	}
}
