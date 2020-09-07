using System;
using System.IO;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000C0 RID: 192
	internal class TransactNamedPipeInputStream : SmbFileInputStream
	{
		// Token: 0x06000637 RID: 1591 RVA: 0x00021E6C File Offset: 0x0002006C
		internal TransactNamedPipeInputStream(SmbNamedPipe pipe) : base(pipe, (pipe.PipeType & -65281) | 32)
		{
			this._dcePipe = ((pipe.PipeType & 1536) != 1536);
			this.Lock = new object();
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00021EC8 File Offset: 0x000200C8
		public override int Read()
		{
			int result = -1;
			object @lock = this.Lock;
			lock (@lock)
			{
				try
				{
					while (this._used == 0)
					{
						Runtime.Wait(this.Lock);
					}
				}
				catch (Exception ex)
				{
					throw new IOException(ex.Message);
				}
				result = (int)(this._pipeBuf[this._begIdx] & byte.MaxValue);
				this._begIdx = (this._begIdx + 1) % this._pipeBuf.Length;
			}
			return result;
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00021F78 File Offset: 0x00020178
		public override int Read(byte[] b)
		{
			return this.Read(b, 0, b.Length);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x00021F98 File Offset: 0x00020198
		public override int Read(byte[] b, int off, int len)
		{
			int num = -1;
			bool flag = len <= 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				object @lock = this.Lock;
				lock (@lock)
				{
					try
					{
						while (this._used == 0)
						{
							Runtime.Wait(this.Lock);
						}
					}
					catch (Exception ex)
					{
						throw new IOException(ex.Message);
					}
					int num2 = this._pipeBuf.Length - this._begIdx;
					num = ((len > this._used) ? this._used : len);
					bool flag3 = this._used > num2 && num > num2;
					if (flag3)
					{
						Array.Copy(this._pipeBuf, this._begIdx, b, off, num2);
						off += num2;
						Array.Copy(this._pipeBuf, 0, b, off, num - num2);
					}
					else
					{
						Array.Copy(this._pipeBuf, this._begIdx, b, off, num);
					}
					this._used -= num;
					this._begIdx = (this._begIdx + num) % this._pipeBuf.Length;
				}
				result = num;
			}
			return result;
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x000220DC File Offset: 0x000202DC
		public override int Available()
		{
			bool flag = this.File.Log.Level >= 3;
			if (flag)
			{
				this.File.Log.WriteLine("Named Pipe available() does not apply to TRANSACT Named Pipes");
			}
			return 0;
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00022124 File Offset: 0x00020324
		internal virtual int Receive(byte[] b, int off, int len)
		{
			bool flag = len > this._pipeBuf.Length - this._used;
			int num2;
			if (flag)
			{
				int num = this._pipeBuf.Length * 2;
				bool flag2 = len > num - this._used;
				if (flag2)
				{
					num = len + this._used;
				}
				byte[] pipeBuf = this._pipeBuf;
				this._pipeBuf = new byte[num];
				num2 = pipeBuf.Length - this._begIdx;
				bool flag3 = this._used > num2;
				if (flag3)
				{
					Array.Copy(pipeBuf, this._begIdx, this._pipeBuf, 0, num2);
					Array.Copy(pipeBuf, 0, this._pipeBuf, num2, this._used - num2);
				}
				else
				{
					Array.Copy(pipeBuf, this._begIdx, this._pipeBuf, 0, this._used);
				}
				this._begIdx = 0;
				this._nxtIdx = this._used;
			}
			num2 = this._pipeBuf.Length - this._nxtIdx;
			bool flag4 = len > num2;
			if (flag4)
			{
				Array.Copy(b, off, this._pipeBuf, this._nxtIdx, num2);
				off += num2;
				Array.Copy(b, off, this._pipeBuf, 0, len - num2);
			}
			else
			{
				Array.Copy(b, off, this._pipeBuf, this._nxtIdx, len);
			}
			this._nxtIdx = (this._nxtIdx + len) % this._pipeBuf.Length;
			this._used += len;
			return len;
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00022290 File Offset: 0x00020490
		public virtual int Dce_read(byte[] b, int off, int len)
		{
			return base.Read(b, off, len);
		}

		// Token: 0x040003BF RID: 959
		private const int InitPipeSize = 4096;

		// Token: 0x040003C0 RID: 960
		private byte[] _pipeBuf = new byte[4096];

		// Token: 0x040003C1 RID: 961
		private int _begIdx;

		// Token: 0x040003C2 RID: 962
		private int _nxtIdx;

		// Token: 0x040003C3 RID: 963
		private int _used;

		// Token: 0x040003C4 RID: 964
		private bool _dcePipe;

		// Token: 0x040003C5 RID: 965
		internal object Lock;
	}
}
