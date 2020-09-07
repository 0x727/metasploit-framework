using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200004A RID: 74
	public class InputStream : IDisposable
	{
		// Token: 0x060001CC RID: 460 RVA: 0x00008CEC File Offset: 0x00006EEC
		public static implicit operator InputStream(Stream s)
		{
			return InputStream.Wrap(s);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00008D04 File Offset: 0x00006F04
		public static implicit operator Stream(InputStream s)
		{
			return s.GetWrappedStream();
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00008D1C File Offset: 0x00006F1C
		public virtual int Available()
		{
			bool flag = this.Wrapped is WrappedSystemStream;
			int result;
			if (flag)
			{
				result = ((WrappedSystemStream)this.Wrapped).InputStream.Available();
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00008D5C File Offset: 0x00006F5C
		public virtual void Close()
		{
			bool flag = this.Wrapped != null;
			if (flag)
			{
				this.Wrapped.Close();
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00008D85 File Offset: 0x00006F85
		public void Dispose()
		{
			this.Close();
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00008D90 File Offset: 0x00006F90
		internal Stream GetWrappedStream()
		{
			return new WrappedSystemStream(this);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00008DA8 File Offset: 0x00006FA8
		public virtual void Mark(int readlimit)
		{
			bool flag = this.Wrapped is WrappedSystemStream;
			if (flag)
			{
				((WrappedSystemStream)this.Wrapped).InputStream.Mark(readlimit);
			}
			else
			{
				bool flag2 = this.BaseStream is WrappedSystemStream;
				if (flag2)
				{
					((WrappedSystemStream)this.BaseStream).OnMark(readlimit);
				}
				bool flag3 = this.Wrapped != null;
				if (flag3)
				{
					this._mark = this.Wrapped.Position;
				}
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00008E28 File Offset: 0x00007028
		public virtual bool MarkSupported()
		{
			bool flag = this.Wrapped is WrappedSystemStream;
			bool result;
			if (flag)
			{
				result = ((WrappedSystemStream)this.Wrapped).InputStream.MarkSupported();
			}
			else
			{
				result = (this.Wrapped != null && this.Wrapped.CanSeek);
			}
			return result;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00008E7C File Offset: 0x0000707C
		public virtual int Read()
		{
			bool flag = this.Wrapped == null;
			if (flag)
			{
				throw new NotImplementedException();
			}
			return this.Wrapped.ReadByte();
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00008EB0 File Offset: 0x000070B0
		public virtual int Read(byte[] buf)
		{
			return this.Read(buf, 0, buf.Length);
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00008ED0 File Offset: 0x000070D0
		public virtual int Read(byte[] b, int off, int len)
		{
			bool flag = this.Wrapped is WrappedSystemStream;
			int result;
			if (flag)
			{
				result = ((WrappedSystemStream)this.Wrapped).InputStream.Read(b, off, len);
			}
			else
			{
				bool flag2 = this.Wrapped != null;
				if (flag2)
				{
					int num = this.Wrapped.Read(b, off, len);
					result = ((num <= 0) ? -1 : num);
				}
				else
				{
					int i;
					for (i = 0; i < len; i++)
					{
						int num2 = this.Read();
						bool flag3 = num2 == -1;
						if (flag3)
						{
							return -1;
						}
						b[off + i] = (byte)num2;
					}
					result = i;
				}
			}
			return result;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00008F70 File Offset: 0x00007170
		public virtual void Reset()
		{
			bool flag = this.Wrapped is WrappedSystemStream;
			if (flag)
			{
				((WrappedSystemStream)this.Wrapped).InputStream.Reset();
			}
			else
			{
				bool flag2 = this.Wrapped == null;
				if (flag2)
				{
					throw new IOException();
				}
				this.Wrapped.Position = this._mark;
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00008FD0 File Offset: 0x000071D0
		public virtual long Skip(long cnt)
		{
			bool flag = this.Wrapped is WrappedSystemStream;
			long result;
			if (flag)
			{
				result = ((WrappedSystemStream)this.Wrapped).InputStream.Skip(cnt);
			}
			else
			{
				long num;
				for (num = cnt; num > 0L; num -= 1L)
				{
					bool flag2 = this.Read() == -1;
					if (flag2)
					{
						return cnt - num;
					}
				}
				result = cnt - num;
			}
			return result;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000903C File Offset: 0x0000723C
		internal bool CanSeek()
		{
			bool flag = this.Wrapped != null;
			return flag && this.Wrapped.CanSeek;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000906C File Offset: 0x0000726C
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000909C File Offset: 0x0000729C
		internal long Position
		{
			get
			{
				bool flag = this.Wrapped != null;
				if (flag)
				{
					return this.Wrapped.Position;
				}
				throw new NotSupportedException();
			}
			set
			{
				bool flag = this.Wrapped != null;
				if (flag)
				{
					this.Wrapped.Position = value;
					return;
				}
				throw new NotSupportedException();
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060001DC RID: 476 RVA: 0x000090CC File Offset: 0x000072CC
		public virtual long Length
		{
			get
			{
				bool flag = this.Wrapped != null;
				if (flag)
				{
					return this.Wrapped.Length;
				}
				throw new NotSupportedException();
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00009100 File Offset: 0x00007300
		internal static InputStream Wrap(Stream s)
		{
			return new InputStream
			{
				Wrapped = s
			};
		}

		// Token: 0x0400005E RID: 94
		private long _mark;

		// Token: 0x0400005F RID: 95
		protected Stream Wrapped;

		// Token: 0x04000060 RID: 96
		protected Stream BaseStream;
	}
}
