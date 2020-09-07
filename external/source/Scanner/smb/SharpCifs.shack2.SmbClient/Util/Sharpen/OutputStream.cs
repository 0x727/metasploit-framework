using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000059 RID: 89
	public class OutputStream : IDisposable
	{
		// Token: 0x06000230 RID: 560 RVA: 0x0000A098 File Offset: 0x00008298
		public static implicit operator OutputStream(Stream s)
		{
			return OutputStream.Wrap(s);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000A0B0 File Offset: 0x000082B0
		public static implicit operator Stream(OutputStream s)
		{
			return s.GetWrappedStream();
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000A0C8 File Offset: 0x000082C8
		public virtual void Close()
		{
			bool flag = this.Wrapped != null;
			if (flag)
			{
				this.Wrapped.Close();
			}
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000A0F1 File Offset: 0x000082F1
		public void Dispose()
		{
			this.Close();
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000A0FC File Offset: 0x000082FC
		public virtual void Flush()
		{
			bool flag = this.Wrapped != null;
			if (flag)
			{
				this.Wrapped.Flush();
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000A128 File Offset: 0x00008328
		internal Stream GetWrappedStream()
		{
			return new WrappedSystemStream(this);
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000A140 File Offset: 0x00008340
		internal static OutputStream Wrap(Stream s)
		{
			return new OutputStream
			{
				Wrapped = s
			};
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000A160 File Offset: 0x00008360
		public virtual void Write(int b)
		{
			bool flag = this.Wrapped is WrappedSystemStream;
			if (flag)
			{
				((WrappedSystemStream)this.Wrapped).OutputStream.Write(b);
			}
			else
			{
				bool flag2 = this.Wrapped == null;
				if (flag2)
				{
					throw new NotImplementedException();
				}
				this.Wrapped.WriteByte((byte)b);
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000A1BC File Offset: 0x000083BC
		public virtual void Write(byte[] b)
		{
			this.Write(b, 0, b.Length);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000A1CC File Offset: 0x000083CC
		public virtual void Write(byte[] b, int offset, int len)
		{
			bool flag = this.Wrapped is WrappedSystemStream;
			if (flag)
			{
				((WrappedSystemStream)this.Wrapped).OutputStream.Write(b, offset, len);
			}
			else
			{
				bool flag2 = this.Wrapped != null;
				if (flag2)
				{
					this.Wrapped.Write(b, offset, len);
				}
				else
				{
					for (int i = 0; i < len; i++)
					{
						this.Write((int)b[i + offset]);
					}
				}
			}
		}

		// Token: 0x0400006F RID: 111
		protected Stream Wrapped;
	}
}
