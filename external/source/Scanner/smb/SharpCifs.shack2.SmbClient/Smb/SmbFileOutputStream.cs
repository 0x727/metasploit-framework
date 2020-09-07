using System;
using System.IO;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000AE RID: 174
	public class SmbFileOutputStream : OutputStream
	{
		// Token: 0x06000564 RID: 1380 RVA: 0x0001C619 File Offset: 0x0001A819
		public SmbFileOutputStream(string url) : this(url, false)
		{
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0001C625 File Offset: 0x0001A825
		public SmbFileOutputStream(SmbFile file) : this(file, false)
		{
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x0001C631 File Offset: 0x0001A831
		public SmbFileOutputStream(string url, bool append) : this(new SmbFile(url), append)
		{
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0001C642 File Offset: 0x0001A842
		public SmbFileOutputStream(SmbFile file, bool append) : this(file, append, append ? 22 : 82)
		{
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x0001C657 File Offset: 0x0001A857
		public SmbFileOutputStream(string url, int shareAccess) : this(new SmbFile(url, string.Empty, null, shareAccess), false)
		{
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x0001C670 File Offset: 0x0001A870
		internal SmbFileOutputStream(SmbFile file, bool append, int openFlags)
		{
			this._file = file;
			this._append = append;
			this._openFlags = openFlags;
			this._access = (int)((uint)openFlags >> 16 & 65535U);
			if (append)
			{
				try
				{
					this._fp = file.Length();
				}
				catch (SmbAuthException ex)
				{
					throw;
				}
				catch (SmbException)
				{
					this._fp = 0L;
				}
			}
			bool flag = file is SmbNamedPipe && file.Unc.StartsWith("\\pipe\\");
			if (flag)
			{
				file.Unc = Runtime.Substring(file.Unc, 5);
				file.Send(new TransWaitNamedPipe("\\pipe" + file.Unc), new TransWaitNamedPipeResponse());
			}
			file.Open(openFlags, this._access | SmbConstants.FileWriteData, 128, 0);
			this._openFlags &= -81;
			this._writeSize = file.Tree.Session.transport.SndBufSize - 70;
			this._useNtSmbs = file.Tree.Session.transport.HasCapability(SmbConstants.CapNtSmbs);
			bool useNtSmbs = this._useNtSmbs;
			if (useNtSmbs)
			{
				this._reqx = new SmbComWriteAndX();
				this._rspx = new SmbComWriteAndXResponse();
			}
			else
			{
				this._req = new SmbComWrite();
				this._rsp = new SmbComWriteResponse();
			}
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0001C7F4 File Offset: 0x0001A9F4
		public override void Close()
		{
			this._file.Close();
			this._tmp = null;
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001C80A File Offset: 0x0001AA0A
		public override void Write(int b)
		{
			this._tmp[0] = (byte)b;
			this.Write(this._tmp, 0, 1);
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0000A1BC File Offset: 0x000083BC
		public override void Write(byte[] b)
		{
			this.Write(b, 0, b.Length);
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x0001C828 File Offset: 0x0001AA28
		public virtual bool IsOpen()
		{
			return this._file.IsOpen();
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x0001C848 File Offset: 0x0001AA48
		internal virtual void EnsureOpen()
		{
			bool flag = !this._file.IsOpen();
			if (flag)
			{
				this._file.Open(this._openFlags, this._access | SmbConstants.FileWriteData, 128, 0);
				bool append = this._append;
				if (append)
				{
					this._fp = this._file.Length();
				}
			}
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0001C8AC File Offset: 0x0001AAAC
		public override void Write(byte[] b, int off, int len)
		{
			bool flag = !this._file.IsOpen() && this._file is SmbNamedPipe;
			if (flag)
			{
				this._file.Send(new TransWaitNamedPipe("\\pipe" + this._file.Unc), new TransWaitNamedPipeResponse());
			}
			this.WriteDirect(b, off, len, 0);
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x0001C914 File Offset: 0x0001AB14
		public virtual void WriteDirect(byte[] b, int off, int len, int flags)
		{
			bool flag = len <= 0;
			if (!flag)
			{
				bool flag2 = this._tmp == null;
				if (flag2)
				{
					throw new IOException("Bad file descriptor");
				}
				this.EnsureOpen();
				do
				{
					int num = (len > this._writeSize) ? this._writeSize : len;
					bool useNtSmbs = this._useNtSmbs;
					if (useNtSmbs)
					{
						this._reqx.SetParam(this._file.Fid, this._fp, len - num, b, off, num);
						bool flag3 = (flags & 1) != 0;
						if (flag3)
						{
							this._reqx.SetParam(this._file.Fid, this._fp, len, b, off, num);
							this._reqx.WriteMode = 8;
						}
						else
						{
							this._reqx.WriteMode = 0;
						}
						this._file.Send(this._reqx, this._rspx);
						this._fp += this._rspx.Count;
						len -= (int)this._rspx.Count;
						off += (int)this._rspx.Count;
					}
					else
					{
						this._req.SetParam(this._file.Fid, this._fp, len - num, b, off, num);
						this._fp += this._rsp.Count;
						len -= (int)this._rsp.Count;
						off += (int)this._rsp.Count;
						this._file.Send(this._req, this._rsp);
					}
				}
				while (len > 0);
			}
		}

		// Token: 0x04000327 RID: 807
		private SmbFile _file;

		// Token: 0x04000328 RID: 808
		private bool _append;

		// Token: 0x04000329 RID: 809
		private bool _useNtSmbs;

		// Token: 0x0400032A RID: 810
		private int _openFlags;

		// Token: 0x0400032B RID: 811
		private int _access;

		// Token: 0x0400032C RID: 812
		private int _writeSize;

		// Token: 0x0400032D RID: 813
		private long _fp;

		// Token: 0x0400032E RID: 814
		private byte[] _tmp = new byte[1];

		// Token: 0x0400032F RID: 815
		private SmbComWriteAndX _reqx;

		// Token: 0x04000330 RID: 816
		private SmbComWriteAndXResponse _rspx;

		// Token: 0x04000331 RID: 817
		private SmbComWrite _req;

		// Token: 0x04000332 RID: 818
		private SmbComWriteResponse _rsp;
	}
}
