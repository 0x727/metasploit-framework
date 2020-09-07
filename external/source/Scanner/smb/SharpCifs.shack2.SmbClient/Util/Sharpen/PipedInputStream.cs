using System;
using System.Threading;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200005B RID: 91
	internal class PipedInputStream : InputStream
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600023E RID: 574 RVA: 0x0000A280 File Offset: 0x00008480
		// (set) Token: 0x0600023F RID: 575 RVA: 0x0000A298 File Offset: 0x00008498
		public int In
		{
			get
			{
				return this._start;
			}
			set
			{
				this._start = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000240 RID: 576 RVA: 0x0000A2A4 File Offset: 0x000084A4
		// (set) Token: 0x06000241 RID: 577 RVA: 0x0000A2BC File Offset: 0x000084BC
		public int Out
		{
			get
			{
				return this._end;
			}
			set
			{
				this._end = value;
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000A2C6 File Offset: 0x000084C6
		public PipedInputStream()
		{
			this._thisLock = new object();
			this._dataEvent = new ManualResetEvent(false);
			this.Buffer = new byte[1025];
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000A2FE File Offset: 0x000084FE
		public PipedInputStream(PipedOutputStream os) : this()
		{
			os.Attach(this);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000A310 File Offset: 0x00008510
		public override void Close()
		{
			object thisLock = this._thisLock;
			lock (thisLock)
			{
				this._closed = true;
				this._dataEvent.Set();
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000A364 File Offset: 0x00008564
		public override int Available()
		{
			object thisLock = this._thisLock;
			int result;
			lock (thisLock)
			{
				bool flag2 = this._start <= this._end;
				if (flag2)
				{
					result = this._end - this._start;
				}
				else
				{
					result = this.Buffer.Length - this._start + this._end;
				}
			}
			return result;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000A3E0 File Offset: 0x000085E0
		public override int Read()
		{
			bool flag = this._oneBuffer == null;
			if (flag)
			{
				this._oneBuffer = new byte[1];
			}
			bool flag2 = this.Read(this._oneBuffer, 0, 1) == -1;
			int result;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				result = (int)this._oneBuffer[0];
			}
			return result;
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000A42C File Offset: 0x0000862C
		public override int Read(byte[] b, int offset, int len)
		{
			int num = 0;
			do
			{
				this._dataEvent.WaitOne();
				object thisLock = this._thisLock;
				lock (thisLock)
				{
					bool flag2 = this._closed && this.Available() == 0;
					if (flag2)
					{
						return -1;
					}
					bool flag3 = this._start < this._end;
					if (flag3)
					{
						num = Math.Min(len, this._end - this._start);
						Array.Copy(this.Buffer, this._start, b, offset, num);
						this._start += num;
					}
					else
					{
						bool flag4 = this._start > this._end;
						if (flag4)
						{
							num = Math.Min(len, this.Buffer.Length - this._start);
							Array.Copy(this.Buffer, this._start, b, offset, num);
							len -= num;
							this._start = (this._start + num) % this.Buffer.Length;
							bool flag5 = len > 0;
							if (flag5)
							{
								int num2 = Math.Min(len, this._end);
								Array.Copy(this.Buffer, 0, b, offset + num, num2);
								this._start += num2;
								num += num2;
							}
						}
					}
					bool flag6 = this._start == this._end && !this._closed;
					if (flag6)
					{
						this._dataEvent.Reset();
					}
					Monitor.PulseAll(this._thisLock);
				}
			}
			while (num == 0);
			return num;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000A5EC File Offset: 0x000087EC
		private int Allocate(int len)
		{
			int result;
			while ((result = this.TryAllocate(len)) == 0)
			{
				try
				{
					Monitor.Wait(this._thisLock);
				}
				catch
				{
					this._closed = true;
					this._dataEvent.Set();
					throw;
				}
			}
			return result;
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000A64C File Offset: 0x0000884C
		private int TryAllocate(int len)
		{
			bool flag = this._start <= this._end;
			int num;
			if (flag)
			{
				num = this.Buffer.Length - this._end + this._start;
			}
			else
			{
				num = this._start - this._end;
			}
			bool flag2 = num <= len;
			if (flag2)
			{
				bool flag3 = !this._allowGrow;
				if (flag3)
				{
					return (num > 0) ? (num - 1) : 0;
				}
				int num2 = len - num + 1;
				byte[] array = new byte[this.Buffer.Length + num2];
				bool flag4 = this._start <= this._end;
				if (flag4)
				{
					Array.Copy(this.Buffer, this._start, array, this._start, this._end - this._start);
				}
				else
				{
					Array.Copy(this.Buffer, 0, array, 0, this._end);
					Array.Copy(this.Buffer, this._start, array, this._start + num2, this.Buffer.Length - this._start);
					this._start += num2;
				}
				this.Buffer = array;
			}
			return len;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000A784 File Offset: 0x00008984
		internal void Write(int b)
		{
			object thisLock = this._thisLock;
			lock (thisLock)
			{
				this.Allocate(1);
				this.Buffer[this._end] = (byte)b;
				this._end = (this._end + 1) % this.Buffer.Length;
				this._dataEvent.Set();
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000A800 File Offset: 0x00008A00
		internal void Write(byte[] b, int offset, int len)
		{
			do
			{
				object thisLock = this._thisLock;
				lock (thisLock)
				{
					int num = this.Allocate(len);
					int num2 = Math.Min(this.Buffer.Length - this._end, num);
					Array.Copy(b, offset, this.Buffer, this._end, num2);
					this._end = (this._end + num2) % this.Buffer.Length;
					bool flag2 = num2 < num;
					if (flag2)
					{
						Array.Copy(b, offset + num2, this.Buffer, 0, num - num2);
						this._end += num - num2;
					}
					this._dataEvent.Set();
					len -= num;
					offset += num;
				}
			}
			while (len > 0);
		}

		// Token: 0x04000070 RID: 112
		private byte[] _oneBuffer;

		// Token: 0x04000071 RID: 113
		public const int PipeSize = 1024;

		// Token: 0x04000072 RID: 114
		protected byte[] Buffer;

		// Token: 0x04000073 RID: 115
		private bool _closed;

		// Token: 0x04000074 RID: 116
		private ManualResetEvent _dataEvent;

		// Token: 0x04000075 RID: 117
		private int _end;

		// Token: 0x04000076 RID: 118
		private int _start;

		// Token: 0x04000077 RID: 119
		private object _thisLock;

		// Token: 0x04000078 RID: 120
		private bool _allowGrow = false;
	}
}
