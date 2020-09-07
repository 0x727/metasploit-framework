using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200006C RID: 108
	internal class WrappedSystemStream : Stream
	{
		// Token: 0x0600030F RID: 783 RVA: 0x0000C594 File Offset: 0x0000A794
		public WrappedSystemStream(InputStream ist)
		{
			this._ist = ist;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000C5A5 File Offset: 0x0000A7A5
		public WrappedSystemStream(OutputStream ost)
		{
			this._ost = ost;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000C5B8 File Offset: 0x0000A7B8
		public InputStream InputStream
		{
			get
			{
				return this._ist;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000312 RID: 786 RVA: 0x0000C5D0 File Offset: 0x0000A7D0
		public OutputStream OutputStream
		{
			get
			{
				return this._ost;
			}
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0000C5E8 File Offset: 0x0000A7E8
		public override void Close()
		{
			bool flag = this._ist != null;
			if (flag)
			{
				this._ist.Close();
			}
			bool flag2 = this._ost != null;
			if (flag2)
			{
				this._ost.Close();
			}
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0000C62C File Offset: 0x0000A82C
		public override void Flush()
		{
			this._ost.Flush();
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000C63C File Offset: 0x0000A83C
		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = this._ist.Read(buffer, offset, count);
			bool flag = num != -1;
			int result;
			if (flag)
			{
				this._position += num;
				result = num;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0000C67C File Offset: 0x0000A87C
		public override int ReadByte()
		{
			int num = this._ist.Read();
			bool flag = num != -1;
			if (flag)
			{
				this._position++;
			}
			return num;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000C6B4 File Offset: 0x0000A8B4
		public override long Seek(long offset, SeekOrigin origin)
		{
			bool flag = origin == SeekOrigin.Begin;
			if (flag)
			{
				this.Position = offset;
			}
			else
			{
				bool flag2 = origin == SeekOrigin.Current;
				if (flag2)
				{
					this.Position += offset;
				}
				else
				{
					bool flag3 = origin == SeekOrigin.End;
					if (flag3)
					{
						this.Position = this.Length + offset;
					}
				}
			}
			return this.Position;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00006440 File Offset: 0x00004640
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000C70E File Offset: 0x0000A90E
		public override void Write(byte[] buffer, int offset, int count)
		{
			this._ost.Write(buffer, offset, count);
			this._position += count;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0000C72E File Offset: 0x0000A92E
		public override void WriteByte(byte value)
		{
			this._ost.Write((int)value);
			this._position++;
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0000C74C File Offset: 0x0000A94C
		public override bool CanRead
		{
			get
			{
				return this._ist != null;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0000C768 File Offset: 0x0000A968
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0000C77C File Offset: 0x0000A97C
		public override bool CanWrite
		{
			get
			{
				return this._ost != null;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600031E RID: 798 RVA: 0x0000C798 File Offset: 0x0000A998
		public override long Length
		{
			get
			{
				return this._ist.Length;
			}
		}

		// Token: 0x0600031F RID: 799 RVA: 0x0000C7B5 File Offset: 0x0000A9B5
		internal void OnMark(int nb)
		{
			this._markedPosition = this._position;
			this._ist.Mark(nb);
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000320 RID: 800 RVA: 0x0000C7D4 File Offset: 0x0000A9D4
		// (set) Token: 0x06000321 RID: 801 RVA: 0x0000C818 File Offset: 0x0000AA18
		public override long Position
		{
			get
			{
				bool flag = this._ist != null && this._ist.CanSeek();
				long result;
				if (flag)
				{
					result = this._ist.Position;
				}
				else
				{
					result = (long)this._position;
				}
				return result;
			}
			set
			{
				bool flag = value == (long)this._position;
				if (!flag)
				{
					bool flag2 = value == (long)this._markedPosition;
					if (flag2)
					{
						this._ist.Reset();
					}
					else
					{
						bool flag3 = this._ist != null && this._ist.CanSeek();
						if (!flag3)
						{
							throw new NotSupportedException();
						}
						this._ist.Position = value;
					}
				}
			}
		}

		// Token: 0x04000097 RID: 151
		private InputStream _ist;

		// Token: 0x04000098 RID: 152
		private OutputStream _ost;

		// Token: 0x04000099 RID: 153
		private int _position;

		// Token: 0x0400009A RID: 154
		private int _markedPosition;
	}
}
